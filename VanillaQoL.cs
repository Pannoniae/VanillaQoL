using System.IO;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;
using VanillaQoL.Fixes;
using VanillaQoL.Gameplay;

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

    public override void Load() {
        instance = this;
        hasThorium = ModLoader.HasMod("ThoriumMod");
        hasCalamity = ModLoader.HasMod("CalamityMod");
        hasCensus = ModLoader.HasMod("Census");
        hasRecipeBrowser = ModLoader.HasMod("RecipeBrowser");
        hasMagicStorage = ModLoader.HasMod("MagicStorage");
        hasCheatSheet = ModLoader.HasMod("CheatSheet");
        hasHEROsMod = ModLoader.HasMod("HEROsMod");
        ILEdits.load();
        ModILEdits.load();
    }

    public override void Unload() {
        if (LanguagePatch.loaded) {
            LanguagePatch.unload();
        }
        ILEdits.unload();

        // IL patch static lambdas are leaking memory, wipe them
        Utils.completelyWipeClass(typeof(ILEdits));
        Utils.completelyWipeClass(typeof(ModILEdits));
        Utils.completelyWipeClass(typeof(RecipeBrowserLogic));
        Utils.completelyWipeClass(typeof(MagicStorageLogic));
        // Func<bool> is a static lambda, this would leak as well
        Utils.completelyWipeClass(typeof(LanguagePatch));

        // memory leak fix
        //if (QoLConfig.Instance.fixMemoryLeaks) {
        //    if (instance.hasHEROsMod) {

        //    }
        //}


        instance = null!;
    }

    public override void PostSetupContent() {
        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggled");
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery");
        }
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        QoLSharedMapSystem.instance.HandlePacket(reader, whoAmI);
    }
}