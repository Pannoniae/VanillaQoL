using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class IFrameUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.immunityFrames;
    }

    public override void OnModLoad() {
        var x = CalamityTweakTables.stripProjectileRules("IDStaticIFrameRule", "LocalIFrameRule");
        VanillaQoL.instance.Logger.Info($"Changed {x} projectiles to vanilla iframes...");
    }
}
