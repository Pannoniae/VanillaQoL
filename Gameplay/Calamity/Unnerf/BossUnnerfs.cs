using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class DefenseDamageUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.defenseDamage;
    }

    public override void OnModLoad() {
        CalamityMod.CalamityMod.ExternalFlag_DisableDefenseDamage = true;
    }

    public override void Unload() {
        CalamityMod.CalamityMod.ExternalFlag_DisableDefenseDamage = false;
    }
}
