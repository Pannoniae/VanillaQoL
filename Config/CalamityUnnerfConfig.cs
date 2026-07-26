using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace VanillaQoL.Config;

/// Undoing Calamity's "vanilla changes" (https://calamitymod.wiki.gg/wiki/Vanilla_changes)
[BackgroundColor(16, 0, 2, 1)]
[EnabledIf(typeof(CalamityLoaded))]
public class CalamityUnnerfConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618
    public static CalamityUnnerfConfig Instance;
#pragma warning restore CS8618

    public override ConfigScope Mode => ConfigScope.ServerSide;

    #region Systems

    [Header("systems")]

    // Cobalt Shield
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool defenseDamage { get; set; }

    // Beetle Shell
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool damageReduction { get; set; }

    // Ale
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool buffs { get; set; }

    #endregion

    #region Items

    [Header("items")]

    // Soaring Insignia
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool soaringInsignia { get; set; }

    // Shadow Scale
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shadowArmour { get; set; }

    #endregion
}
