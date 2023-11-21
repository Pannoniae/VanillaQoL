using System.ComponentModel;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace VanillaQoL.Config;

[BackgroundColor(16, 0, 2, 0)]
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

    [Header("fixes")]
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fixPvPUI { get; set; }

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnChanged() {
        // force-disable tooltips with calamity
        if (ModLoader.HasMod("CalamityMod") && showHookTooltips) {
            showHookTooltips = false;
        }
    }
}