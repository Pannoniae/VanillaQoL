using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.CalPlayer;
using MagicStorage.UI.States;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.IL;

public class ModILEdits {
    public static void load() {
        try {
            if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
                if (VanillaQoL.instance.hasRecipeBrowser) {
                    RecipeBrowserLogic.load();
                }

                if (VanillaQoL.instance.hasMagicStorage) {
                    MagicStorageLogic.load();
                }
            }
        }
        catch (Exception e) {
            VanillaQoL.instance.Logger.Error(
                $"Couldn't load Magic Storage and Recipe Browser integration! Error message: {e}");
        }
    }

    public static void unload() {
        // nothing?
    }

    public static IEnumerable<Condition> filterConditions(IEnumerable<Condition> original) {
        return original.Where(c => !isHidden(c.Description));
    }
    
    public static List<Condition> filterConditionsL(List<Condition> original) {
        return original.Where(c => !isHidden(c.Description)).ToList();
    }

    private static bool isHidden(LocalizedText loc) {
        return LanguagePatch._overrides.ContainsKey(loc.Key) && LanguagePatch._overrides[loc.Key].hidden;
    }
}

[JITWhenModsEnabled("RecipeBrowser")]
public static class RecipeBrowserLogic {
    public static void load() {
        // yey, more internal classes. fuck this, reflection hackery time
        var recipeBrowserModAssembly = ModLoader.GetMod("RecipeBrowser").Code;
        try {
            var recipeInfoType1 = recipeBrowserModAssembly.GetType("RecipeBrowser.UIRecipeInfo");
            var recipeInfoType2 = recipeBrowserModAssembly.GetType("RecipeBrowser.UIRecipeInfoRightAligned");
            var recipeBrowserMethod1 =
                recipeInfoType1!.GetMethod("DrawSelf", BindingFlags.Instance | BindingFlags.NonPublic);
            var recipeBrowserMethod2 =
                recipeInfoType2!.GetMethod("DrawSelf", BindingFlags.Instance | BindingFlags.NonPublic);
            MonoModHooks.Modify(recipeBrowserMethod1, removeHiddenConditions);
            MonoModHooks.Modify(recipeBrowserMethod2, removeHiddenConditions);
        }
        catch (Exception e) {
            VanillaQoL.instance.Logger.Warn($"Couldn't inject into RecipeBrowser to remove hidden conditions! {e}");
        }
    }

    // [112 39 - 112 56]
    //IL_027b: ldfld        class [System.Collections]System.Collections.Generic.List`1<class [tModLoader]Terraria.Condition> [tModLoader]Terraria.Recipe::Conditions
    private static void removeHiddenConditions(ILContext il) {
        var ilCursor = new ILCursor(il);
        // match the ldfld before the loop, modify the list
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Recipe>("Conditions"))) {
            // call our filter method
            ilCursor.Emit<ModILEdits>(OpCodes.Call, "filterConditionsL");
            // afterwards the list is on the stack as we wanted

            VanillaQoL.instance.Logger.Info(
                "Patched RecipeBrowser UIRecipeInfo for hiding invisible recipe conditions!");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate recipe condition check in RecipeBrowser");
        }
    }
}

[JITWhenModsEnabled("MagicStorage")]
public static class MagicStorageLogic {
    // thank you very much for not making everything internal, I love you<3
    public static void load() {
        var magicStorageType = typeof(CraftingUIState);
        var magicStorageMethod =
            magicStorageType.GetMethod("UpdateRecipeText", BindingFlags.Instance | BindingFlags.NonPublic);

        MonoModHooks.Modify(magicStorageMethod, removeHiddenConditions);
    }

    private static void removeHiddenConditions(ILContext il) {
        var ilCursor = new ILCursor(il);
        // this time we patch the ldloc.s before the loop, to be nicer and less fragile, no need to hack that much
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(3))) {
            // call our filter method
            ilCursor.Emit<ModILEdits>(OpCodes.Call, "filterConditions");
            // afterwards the list is on the stack as we wanted

            VanillaQoL.instance.Logger.Info(
                "Patched MagicStorage UpdateRecipeText for hiding invisible recipe conditions!");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate recipe condition check in MagicStorage");
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public static class CalamityLogic {
    // thank you very much for not making everything internal, I love you<3
    public static void load() { /*

        The good part is that none of this shit matters anymore. The fake accessory slot is gonezo from Calamity.

        var type = typeof(CelestialOnion);
        var isEnabledMethod =
            type.GetMethod("IsEnabled", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        MonoModHooks.Modify(isEnabledMethod, remove8thSlot);*/
    }

    private static void remove8thSlot(ILContext il) {
        // we don't want the slot at all :)
        var ilCursor = new ILCursor(il);
        ilCursor.Emit(OpCodes.Ldc_I4_0);
        ilCursor.Emit(OpCodes.Ret);
    }
}

// a nested auto-generated class refers to calamity stuff. we need to patch the tml JITing process so it doesn't fucking crash on startup
// because this attribute apparently doesn't apply to nested classes lmao
[JITWhenModsEnabled("CalamityMod")]
public static class CalamityLogic2 {
    // thank you very much for not making everything internal, I love you<3
    public static void load() {
        var type = typeof(CalamityPlayer);
        var deadMethod = type.GetMethod("UpdateDead", BindingFlags.Instance | BindingFlags.Public);
        var deadMethod2 = type.GetMethod("KillPlayer", BindingFlags.Instance | BindingFlags.Public);

        MonoModHooks.Modify(deadMethod, removePlayerRespawn);
        MonoModHooks.Modify(deadMethod2, removePlayerRespawn2);
    }

    // [2433 7 - 2433 118]
    // IL_0b51: ldsfld       bool CalamityMod.CalPlayer.CalamityPlayer::areThereAnyDamnBosses
    // IL_0b56: brtrue.s     IL_0b5f
    private static void removePlayerRespawn(ILContext il) {
        var ilCursor = new ILCursor(il);
        // catch incoming jumps
        if (ilCursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<CalamityPlayer>("areThereAnyDamnBosses"))) {
            ilCursor.EmitRet();
        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match player respawn logic in CalamityPlayer.UpdateDead! (areThereAnyDamnBosses)");
        }
    }

    // [7191 7 - 7191 37]
    // IL_04e2: ldarg.0      // this
    // IL_04e3: call         instance class [tModLoader]Terraria.Player [tModLoader]Terraria.ModLoader.ModPlayer::get_Player()
    // IL_04e8: ldc.i4       600 // 0x00000258
    // IL_04ed: stfld        int32 [tModLoader]Terraria.Player::respawnTimer
    private static void removePlayerRespawn2(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg0(),
                i => i.MatchCall<ModPlayer>("get_Player"),
                i => i.MatchLdcI4(600),
                i => i.MatchStfld<Player>("respawnTimer"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match player respawn logic in CalamityPlayer.UpdateDead! (areThereAnyDamnBosses)");
        }

        // IL_0580: stfld        int32 [tModLoader]Terraria.Player::respawnTimer
        if (!ilCursor.TryFindNext(out var c, i => i.MatchStfld<Player>("respawnTimer"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match jump after player respawn logic in CalamityPlayer.UpdateDead! (Player.respawnTimer)");
        }
        // TryFindNext returns before, go to after
        ilCursor.EmitBr(c[0].Next!.Next);
    }
}

[JITWhenModsEnabled("CalamityMod")]
public static class CalamityLogic3 {
    public static void addBuff(int buff) {
        CalamityLists.persistentBuffList.Add(buff);
    }
}
