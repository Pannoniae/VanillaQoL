using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity rewrites the stats of a few hundred vanilla items through its item tweak table. Unlike everything else in
 * here these aren't all nerfs  and many of the rules have an exact number rather than a delta, so there's no actual way to tell which is which without vanilla's
 * numbers to compare against without looking at each one of them.... I don't have the patience for all that stuff ngl.
 *
 * So we don't try. Every toggle here is a wholesale "put this stat back to vanilla", buffs included, and they're all
 * off by default because this isn't an unnerf.
 */
[JITWhenModsEnabled("CalamityMod")]
public class ItemStatUnnerfs : ModSystem {
    private static CalamityUnnerfConfig cfg => CalamityUnnerfConfig.Instance;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.itemDamage || cfg.itemCrit || cfg.itemDefence || cfg.itemMana ||
                                                 cfg.itemSpeed || cfg.itemShootSpeed || cfg.itemKnockback ||
                                                 cfg.itemScale || cfg.itemValue || cfg.itemToolPower ||
                                                 cfg.itemBehaviour);
    }

    public override void OnModLoad() {
        if (cfg.itemDamage) {
            strip("damage", "DamageDeltaRule", "DamageExactRule", "DamageRatioRule");
        }

        if (cfg.itemCrit) {
            strip("crit chance", "CritChanceDeltaRule", "CritChanceExactRule");
        }

        if (cfg.itemDefence) {
            strip("defence", "DefenseDeltaRule", "DefenseExactRule");
        }

        if (cfg.itemMana) {
            strip("mana cost", "ManaDeltaRule", "ManaExactRule", "ManaRatioRule");
        }
        if (cfg.itemSpeed) {
            strip("attack speed",
                "AttackSpeedExactRule", "AttackSpeedRatioRule",
                "UseDeltaRule", "UseExactRule", "UseRatioRule",
                "UseTimeDeltaRule", "UseTimeExactRule", "UseTimeRatioRule",
                "UseAnimationDeltaRule", "UseAnimationExactRule", "UseAnimationRatioRule",
                "ReuseDelayDeltaRule", "ReuseDelayExactRule", "ReuseDelayRatioRule");
        }

        if (cfg.itemShootSpeed) {
            strip("shoot speed", "ShootSpeedDeltaRule", "ShootSpeedExactRule", "ShootSpeedRatioRule");
        }

        if (cfg.itemKnockback) {
            strip("knockback", "KnockbackDeltaRule", "KnockbackExactRule", "KnockbackRatioRule");
        }

        if (cfg.itemScale) {
            strip("hitbox size", "ScaleDeltaRule", "ScaleExactRule", "ScaleRatioRule");
        }

        if (cfg.itemValue) {
            strip("sell price", "ValueRule");
        }

        if (cfg.itemToolPower) {
            strip("tool power", "AxePowerRule", "HammerPowerRule", "PickPowerRule",
                "TileBoostDeltaRule", "TileBoostExactRule");
        }

        if (cfg.itemBehaviour) {
            strip("behaviour", "ConsumableRule", "MaxStackRule", "MeleeSettingsRule", "UseTurnRule");
        }
    }

    private static void strip(string what, params string[] rules) {
        var changed = CalamityTweakTables.stripItemRules(rules);
        VanillaQoL.instance.Logger.Info($"Put {changed} items back to their vanilla {what}.");
    }
}
