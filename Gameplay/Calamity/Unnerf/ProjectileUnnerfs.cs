using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class IFrameUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.immunityFrames;
    }

    public override void OnModLoad() {
        var x = CalamityTweakTables.stripProjectileRules("IDStaticIFrameRule", "LocalIFrameRule",
            "SingleHitImmunityRule");
        VanillaQoL.instance.Logger.Info($"Changed {x} projectiles to vanilla iframes...");
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class ProjectileStatUnnerfs : ModSystem {
    private static CalamityUnnerfConfig cfg => CalamityUnnerfConfig.Instance;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.projPiercing || cfg.projArmourPen || cfg.projSpeed ||
                                                 cfg.projLifetime || cfg.projScale || cfg.projYoyoStats ||
                                                 cfg.projCollision || cfg.projTrueMelee);
    }

    public override void OnModLoad() {
        if (cfg.projPiercing) {
            strip("piercing", "PiercingDeltaRule", "PiercingExactRule");
        }

        if (cfg.projArmourPen) {
            strip("armour penetration", "ArmorPenetrationDeltaRule", "ArmorPenetrationExactRule");
        }

        if (cfg.projSpeed) {
            strip("speed", "ExtraUpdatesDeltaRule", "ExtraUpdatesExactRule", "MaxUpdatesExactRule");
        }

        if (cfg.projLifetime) {
            strip("lifetime", "TimeLeftDeltaRule", "TimeLeftExactRule");
        }

        if (cfg.projScale) {
            strip("hitbox size", "ScaleDeltaRule", "ScaleExactRule", "ScaleRatioRule");
        }

        if (cfg.projYoyoStats) {
            strip("yo-yo stats", "YoyoLifetimeRule", "YoyoRangeRule", "YoyoTopSpeedRule");
        }

        if (cfg.projCollision) {
            strip("water and tile behaviour", "IgnoreWaterRule", "TileCollideRule");
        }

        if (cfg.projTrueMelee) {
            strip("damage class", "TrueMeleeRule", "TrueMeleeNoSpeedRule");
        }
    }

    private static void strip(string what, params string[] rules) {
        var changed = CalamityTweakTables.stripProjectileRules(rules);
        VanillaQoL.instance.Logger.Info($"Reverted {changed} projectiles back to their vanilla {what}.");
    }
}
