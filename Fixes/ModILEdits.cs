using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MagicStorage.UI.States;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Fixes;

public class ModILEdits {
    public static void load() {
        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            if (VanillaQoL.instance.hasRecipeBrowser) {
                RecipeBrowserLogic.load();
            }

            if (VanillaQoL.instance.hasMagicStorage) {
                MagicStorageLogic.load();
            }
        }
    }

    public static void unload() {
        // nothing?
    }

    public static List<Condition> filterConditions(List<Condition> original) {
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
        var recipeInfoType1 = recipeBrowserModAssembly.GetType("RecipeBrowser.UIRecipeInfo");
        var recipeInfoType2 = recipeBrowserModAssembly.GetType("RecipeBrowser.UIRecipeInfoRightAligned");
        var recipeBrowserMethod1 =
            recipeInfoType1.GetMethod("DrawSelf", BindingFlags.Instance | BindingFlags.NonPublic);
        var recipeBrowserMethod2 =
            recipeInfoType2.GetMethod("DrawSelf", BindingFlags.Instance | BindingFlags.NonPublic);
        MonoModHooks.Modify(recipeBrowserMethod1, removeHiddenConditions);
        MonoModHooks.Modify(recipeBrowserMethod2, removeHiddenConditions);
    }

    // [112 39 - 112 56]
    //IL_027b: ldfld        class [System.Collections]System.Collections.Generic.List`1<class [tModLoader]Terraria.Condition> [tModLoader]Terraria.Recipe::Conditions
    private static void removeHiddenConditions(ILContext il) {
        var ilCursor = new ILCursor(il);
        // match the ldfld before the loop, modify the list
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Recipe>("Conditions"))) {
            // call our filter method
            ilCursor.Emit<ModILEdits>(OpCodes.Call, "filterConditions");
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
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(4))) {
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