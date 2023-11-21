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
    [DefaultValue(true)]
    public bool townNPCSpawning { get; set; }

    // Tally Counter
    [Header("tooltips")]

    // Universal Pylon
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showHookTooltips { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool vanillaThoriumTooltips { get; set; }

    [Header("fixes")]
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fixPvPUI { get; set; }

    public override ConfigScope Mode => ConfigScope.ServerSide;
}