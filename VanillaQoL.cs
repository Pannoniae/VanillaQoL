using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Fixes;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;

    public bool hasThorium;
    public bool hasCalamity;
    public bool hasCensus;

    public override void Load() {
        instance = this;
        hasThorium = ModLoader.HasMod("ThoriumMod");
        hasCalamity = ModLoader.HasMod("CalamityMod");
        hasCensus = ModLoader.HasMod("Census");
        ILEdits.load();

    }

    public override void Unload() {
        ILEdits.unload();
        Utils.completelyWipeClass(typeof(ILEdits));
        instance = null!;
        if (LanguagePatch.loaded) {
            LanguagePatch.unload();
        }
    }

    public override void PostSetupContent() {
        LanguagePatch.modifyKey("Mods.ThoriumMod.Conditions.DonatorItemToggled", " ");
        LanguagePatch.modifyKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery", " ");
    }
}