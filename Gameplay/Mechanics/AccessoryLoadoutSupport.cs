using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace VanillaQoL.Gameplay;

public class AccessoryLoadoutSupport : ModPlayer {
    public Type modAccessorySlots = typeof(ModAccessorySlotPlayer);
    public ModAccessorySlotPlayer modAccessorySlotsPlayer = null!;

    private Dictionary<string, int> slots = null!;

    // we have the actual fields here
    public Item[] exAccessorySlot = null!;
    public Item[] exDyesAccessory = null!;
    public bool[] exHideAccessory = null!;

    public Item[] origExAccessorySlot = null!;
    public Item[] origExDyesAccessory = null!;
    public bool[] origExHideAccessory = null!;

    public int loadout;

    public static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;


    // exAccessorySlot contains 2 * count items, extraCount times.
    // // offset of exAccessorySlot is extraCount * 2
    public int accessoryIndex(int iLoadout, int index, bool vanity) {
        var count = slots.Count;
        // slotsCount if vanity, else 0
        var vanityOffset = vanity ? count : 0;
        // get the array segment corresponding to the loadout
        var offset = iLoadout * count * 2  + index + vanityOffset;
        return offset;
    }

    public void ResetAndSizeAccessoryArrays() {

        var count = slots.Count;
        var extraCount = 3;
        exAccessorySlot = new Item[2 * count * extraCount];
        exDyesAccessory = new Item[count * extraCount];
        exHideAccessory = new bool[count * extraCount];
        for (int i = 0; i < extraCount; i++) {
            var offset = i * count;
            for (int index = 0; index < count; ++index) {
                exDyesAccessory[offset + index] = new Item();
                exHideAccessory[offset + index] = false;
                exAccessorySlot[(offset + index) * 2] = new Item();
                exAccessorySlot[(offset + index) * 2 + 1] = new Item();
            }
        }

        //VanillaQoL.instance.Logger.Warn($"Sync: {count}, {modAccessorySlotsPlayer.SlotCount}");

        // original fields
        modAccessorySlots.GetMethod("ResetAndSizeAccessoryArrays", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(modAccessorySlotsPlayer, Array.Empty<object?>());
        origExAccessorySlot =
            (Item[])modAccessorySlots.GetField("exAccessorySlot", flags)!.GetValue(modAccessorySlotsPlayer)!;
        origExDyesAccessory =
            (Item[])modAccessorySlots.GetField("exDyesAccessory", flags)!.GetValue(modAccessorySlotsPlayer)!;
        origExHideAccessory =
            (bool[])modAccessorySlots.GetField("exHideAccessory", flags)!.GetValue(modAccessorySlotsPlayer)!;
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.accessoryLoadoutSupport;
    }

    public override void Load() {
        // hook save/load
        // just overwrite methods, if they change the loading, the mod is fucked anyway
        MonoModHooks.Modify(modAccessorySlots.GetMethod("SaveData"), saveDataHook);
        MonoModHooks.Modify(modAccessorySlots.GetMethod("LoadData"), loadDataHook);

        // hook loadout change
        IL_Player.TrySwitchingLoadout += loadoutPatch;
        IL_Player.ctor += playerInitPatch;
    }

    public override void UpdateAutopause() {
        syncArrays();
    }

    public override void PreUpdate() {
        syncArrays();
    }

    // make vanilla arrays be in sync with our arrays
    public void syncArrays() {
        var count = modAccessorySlotsPlayer.SlotCount;
        var offset = count * loadout;
        for (int i = 0; i < count; i++) {
            exAccessorySlot[accessoryIndex(loadout, i, false)] = origExAccessorySlot[i];
            exAccessorySlot[accessoryIndex(loadout, i, true)] = origExAccessorySlot[i + count];

            exDyesAccessory[offset + i] = origExDyesAccessory[i];

            exHideAccessory[offset + i] = origExHideAccessory[i];
        }
    }

    public void playerInitPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // IL_0758: ldarg.0      // this
        // IL_0759: call         void Terraria.ModLoader.PlayerLoader::SetupPlayer(class Terraria.Player)
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(),
                i => i.MatchCall(typeof(PlayerLoader), "SetupPlayer"))) {
            ilCursor.EmitLdarg0();
            ilCursor.Emit<AccessoryLoadoutSupport>(OpCodes.Call, "playerInitStatic");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to find the SetupPlayer hook in Player constructor!");
        }
    }

    public static void playerInitStatic(Player player) {
        player.GetModPlayer<AccessoryLoadoutSupport>().playerInit();
    }


    //public override void Initialize() {
    // we cheat a bit to load slightly later than tML's ModPlayer ;)
    public void playerInit() {
        modAccessorySlotsPlayer = Player.GetModPlayer<ModAccessorySlotPlayer>();
        //VanillaQoL.instance.Logger.Info(
        //    $"Loaded, {modAccessorySlotsPlayer.SlotCount}, {modAccessorySlotsPlayer.LoadedSlotCount}");


        slots = (Dictionary<string, int>)modAccessorySlots.GetField("slots", flags)!
            .GetValue(modAccessorySlotsPlayer)!;

        // we have the array but we store all 3 loadouts. we swap them in according to the current loadout
        ResetAndSizeAccessoryArrays();
    }

    public override void Unload() {
        IL_Player.TrySwitchingLoadout -= loadoutPatch;
        IL_Player.ctor -= playerInitPatch;
    }


    public void loadoutPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // patch after stfld
        // IL_0083: stfld        int32 Terraria.Player::CurrentLoadoutIndex
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchStfld<Player>("CurrentLoadoutIndex"))) {
            ilCursor.EmitLdarg0();
            ilCursor.Emit<AccessoryLoadoutSupport>(OpCodes.Call, "switchLoadout");
        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Failed to find the loadout switch in TrySwitchingLoadout! (Player.CurrentLoadoutIndex)");
        }
    }

    public static void switchLoadout(Player player) {
        var instance = player.GetModPlayer<AccessoryLoadoutSupport>();
        // don't do anything if this is not the correct player
        if (player.whoAmI == instance.Player.whoAmI) {
            // 0-based
            instance.loadout = instance.Player.CurrentLoadoutIndex;
            // swap the arrays
            var count = instance.modAccessorySlotsPlayer.SlotCount;
            // offset is 0 when first loadout, SlotCount when second, SlotCount * 2 when third
            //VanillaQoL.instance.Logger.Info($"Swapping loadouts! {count}, {instance.loadout}");
            var offset = instance.loadout * count;
            //VanillaQoL.instance.Logger.Warn("Loaded slots:");
            //foreach (var kv in instance.slots) {
            //    VanillaQoL.instance.Logger.Warn($"{kv.Key}, {kv.Value}");
            //}
            for (int i = 0; i < count; i++) {
                //VanillaQoL.instance.Logger.Warn($"{i}, {offset}, {count}");
                instance.origExAccessorySlot[i] = instance.exAccessorySlot[instance.accessoryIndex(instance.loadout, i, false)];
                instance.origExAccessorySlot[i + count] = instance.exAccessorySlot[instance.accessoryIndex(instance.loadout, i, true)];

                instance.origExDyesAccessory[i] = instance.exDyesAccessory[offset + i];

                instance.origExHideAccessory[i] = instance.exHideAccessory[offset + i];
            }
        }
    }

    public void saveDataHook(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.Emit<AccessoryLoadoutSupport>(OpCodes.Call, "saveData");
        ilCursor.EmitRet();
    }

    public static void saveData(ModAccessorySlotPlayer player, TagCompound tag) {
        var instance = player.Player.GetModPlayer<AccessoryLoadoutSupport>();
        //VanillaQoL.instance.Logger.Warn($"Save: {instance.exAccessorySlot.Length}");
        //for (int i = 0; i < instance.exAccessorySlot.Length; i++) {
        //    VanillaQoL.instance.Logger.Warn(instance.exAccessorySlot[i]);
        //}

        tag["order"] = instance.slots.Keys.ToList();
        tag["items"] = instance.exAccessorySlot.Select(ItemIO.Save).ToList<TagCompound>();
        tag["dyes"] = instance.exDyesAccessory.Select(ItemIO.Save).ToList<TagCompound>();
        tag["visible"] = instance.exHideAccessory.ToList();
    }

    public void loadDataHook(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.Emit<AccessoryLoadoutSupport>(OpCodes.Call, "loadData");
        ilCursor.EmitRet();
    }

    public static void loadData(ModAccessorySlotPlayer player, TagCompound tag) {
        var instance = player.Player.GetModPlayer<AccessoryLoadoutSupport>();
        List<string> list1 = tag.GetList<string>("order").ToList();
        foreach (var key in list1) {
            if (!instance.slots.ContainsKey(key))
                instance.slots.Add(key, instance.slots.Count);
        }

        // proper sizing (3x original)
        instance.ResetAndSizeAccessoryArrays();

        // load the saved data (for all 3)
        List<Item> list2 = tag.GetList<TagCompound>("items")
            .Select(ItemIO.Load).ToList<Item>();
        List<Item> list3 = tag.GetList<TagCompound>("dyes")
            .Select(ItemIO.Load).ToList<Item>();
        List<bool> list4 = tag.GetList<bool>("visible").ToList();
        // only load 3 times if we actually have it saved.
        var max = list3.Count > list1.Count ? 3 : 1;
        for (int i = 0; i < max; i++) {
            // do it three times
            for (int index = 0; index < list1.Count; ++index) {
                var offset = i * list1.Count;
                // get the slot in the slots (1x)
                int slot = instance.slots[list1[index]];
                instance.exDyesAccessory[offset + slot] = list3[offset + index];
                instance.exHideAccessory[offset + slot] = list4[offset + index];
                instance.exAccessorySlot[offset * 2 + slot] = list2[offset * 2 + index];
                // this is the vanity item
                instance.exAccessorySlot[offset * 2 + slot + instance.modAccessorySlotsPlayer.SlotCount] =
                    list2[offset * 2 + index + list1.Count];
            }
        }

        // copy into tML arrays
        switchLoadout(instance.Player);
    }
}