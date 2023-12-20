using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MagicStorage;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;
using VanillaQoL.API;
using VanillaQoL.Config;
using VanillaQoL.Gameplay;
using VanillaQoL.IL;
using VanillaQoL.Items;
using VanillaQoL.Shared;
using VanillaQoL.UI;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;

    public bool hasThorium;
    public bool hasCalamity;
    public bool hasCensus;
    public bool hasRecipeBrowser;
    public bool hasMagicStorage;
    public bool hasCheatSheet;
    public bool hasHEROsMod;
    public bool hasCalamityQoL;

    public override uint ExtraPlayerBuffSlots =>
        (uint)QoLConfig.Instance.moreBuffSlots;

    public VanillaQoL() {
        instance = this;
        PreJITFilter = new Filter();
        ModLeakFix.addHandler();
    }

    public override void Load() {
        // store modnames
        List<string> list = new List<string>();
        for (var i = 0; i < ModLoader.Mods.Length; i++) {
            var m = ModLoader.Mods[i];
            if (m.Name != "ModLoader" && m.Name != "VanillaQoL") list.Add(m.Name);
        }

        ModLeakFix.modNames = list;


        hasThorium = ModLoader.HasMod("ThoriumMod");
        hasCalamity = ModLoader.HasMod("CalamityMod");
        hasCensus = ModLoader.HasMod("Census");
        hasRecipeBrowser = ModLoader.HasMod("RecipeBrowser");
        hasMagicStorage = ModLoader.HasMod("MagicStorage");
        hasCheatSheet = ModLoader.HasMod("CheatSheet");
        hasHEROsMod = ModLoader.HasMod("HEROsMod");
        hasCalamityQoL = ModLoader.HasMod("CalamityQOL");
        ILEdits.load();
        ModILEdits.load();
    }

    public override void Unload() {
        // unregister
        var type = typeof(ChatManager);
        var handlers = type.GetField("_handlers", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ConcurrentDictionary<string, ITagHandler> _handlers =
            (ConcurrentDictionary<string, ITagHandler>)handlers.GetValue(null)!;

        _handlers["npc"] = null!;

        // unload
        if (LanguagePatch.loaded) {
            LanguagePatch.unload();
        }

        //IL_Player.TileInteractionsUse -= SliceOfCake.sliceOfCakePatch;

        ILEdits.unload();
        GlobalFeatures.clear();

        ModLeakFix.ilUnload();


        instance = null!;
    }

    public override void PostSetupContent() {
        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggled");
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery");
        }

        // load chat tags
        // since recipe browser's broken chat tags are loaded in Load(), we do it later to overwrite it:))
        ChatManager.Register<NPCTagHandler>("npc");
    }

    // use at load time
    public static bool isCalamityLoaded() {
        return ModLoader.HasMod("CalamityMod");
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        if (QoLConfig.Instance.mapSharing) {
            QoLSharedMapSystem.instance.HandlePacket(reader, whoAmI);
        }
        else {
            // we know the base stream is a MemoryStream
            var stream = (MemoryStream)reader.BaseStream;
            while (stream.ReadByte() != -1) {
                // read
            }
        }
    }

    public class Filter : PreJITFilter {
        public override bool ShouldJIT(MemberInfo member) {
            // if it's a type, check subtypes as well
            // SORRY FOR THE SPAGHETTI
            return member.DeclaringType?.GetCustomAttributes<MemberJitAttribute>()
                .All(a => a.ShouldJIT(member)) ?? member.GetCustomAttributes<MemberJitAttribute>()
                .All(a => a.ShouldJIT(member));
        }
    }
}

[NoJIT]
public static class ModLeakFix {

    public static List<string> modNames = null!;

    public static void addHandler() {
        var typeCaching = typeof(AssemblyManager).Assembly.GetType("Terraria.ModLoader.Core.TypeCaching");
        var ev = typeCaching!.GetEvent("OnClear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ev.AddEventHandler(null, ilUnload);
    }

    public static void ilUnload() {
        // unload *all* the IL edits
        // NEVER unload IL edits in ILoadable, it runs later than this method

        // wipe
        // IL patch static lambdas are leaking memory, wipe them
        Utils.completelyWipeClass(typeof(ILEdits));
        Utils.completelyWipeClass(typeof(ModILEdits));
        Utils.completelyWipeClass(typeof(LanguagePatch));
        Utils.completelyWipeClass(typeof(RecipeBrowserLogic));
        Utils.completelyWipeClass(typeof(MagicStorageLogic));
        Utils.completelyWipeClass(typeof(CalamityLogic));
        Utils.completelyWipeClass(typeof(CalamityLogic2));
        Utils.completelyWipeClass(typeof(QoLSharedMapSystem));
        Utils.completelyWipeClass(typeof(LockOn));
        Utils.completelyWipeClass(typeof(DisableTownSlimes));
        Utils.completelyWipeClass(typeof(NurseHealing));
        Utils.completelyWipeClass(typeof(AccessoryLoadoutSupport));
        Utils.completelyWipeClass(typeof(AccessorySlotUnlock));
        Utils.completelyWipeClass(typeof(SliceOfCake));
        Utils.completelyWipeClass(typeof(Explosives));
        Utils.completelyWipeClass(typeof(DrillRework));
        Utils.completelyWipeClass(typeof(RespawningRework));
        Utils.completelyWipeClass(typeof(NPCShops));
        Utils.completelyWipeClass(typeof(GlobalFeatures));
        Utils.completelyWipeClass(typeof(PrefixRarity));

        // Func<bool> is a static lambda, this would leak as well
        // memory leak fix

        // clear *all* mod classes
        unload();
    }
    public static void unload() {
        //foreach (var modName in modNames) {
           // var assemblies = AssemblyManager.GetModAssemblies("MagicStorage");
            //foreach (var assembly in assemblies) {
                //foreach (var type in AssemblyManager.GetLoadableTypes(assembly)) {
                    //Utils.completelyWipeClass(type);
                //}
            //}
        //}
        var typeCaching = typeof(AssemblyManager).Assembly.GetType("Terraria.ModLoader.Core.TypeCaching");
        var ev = typeCaching!.GetEvent("OnClear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ev.RemoveEventHandler(null, ilUnload);
    }
}