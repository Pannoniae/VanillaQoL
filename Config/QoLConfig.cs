using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace VanillaQoL.Config;

[BackgroundColor(16, 0, 2, 1)]
public class QoLConfig : ModConfig {
    // magic tModLoader-managed field, assigned
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnassignedField.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static QoLConfig Instance;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    // Tungsten Pickaxe
    [Header("gameplay")]
    // Universal Pylon
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool townNPCSpawning { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool disableShimmerPumping { get; set; }

    // Compass
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool metricSystem { get; set; }

    // Platinum Watch
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool twentyFourHourTime { get; set; }

    // Tally Counter
    [Header("tooltips")]
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showHookTooltips { get; set; }

    // Leaf Wings
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showWingTooltips { get; set; }

    // Shimmer Arrow
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool shimmerableTooltip { get; set; }


    // Lunar Hook
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool vanillaThoriumTooltips { get; set; }

    // Red's Helmet (referencing the paid donor shit)
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool removeThoriumEnabledCraftingTooltips { get; set; }

    [Header("fixes")]

    // Amethyst
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool fixPvPUI { get; set; }

    // Cog
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    //public bool fixMemoryLeaks { get; set; }

    public override ConfigScope Mode => ConfigScope.ServerSide;
}