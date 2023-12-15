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

    // Compass
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool metricSystem { get; set; }

    // Platinum Watch
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool twentyFourHourTime { get; set; }

    // Ladybug
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showLuck { get; set; }

    // Nerdy Slime
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool disableTownSlimes { get; set; }


    // Terra Blade
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ancientSwords { get; set; }

    // x button
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool enableLockOn { get; set; }

    // x button
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool lockOn { get; set; }

    // Ironskin Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(44)]
    [Range(0, 936)]
    public int moreBuffSlots { get; set; }

    // Abeemination
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool queenBeeLarvaeBreak { get; set; }

    // Tinkerer's Workshop
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool disablePrefixChangingRarity { get; set; }

    // Pink Dungeon Brick
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool softDungeonBricks { get; set; }

    // DPS Meter
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool accessoryLoadoutSupport { get; set; }

    // Demon Heart
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool accessorySlotUnlock { get; set; }

    // Vortex Drill
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool drillRework { get; set; }

    // Slice of Cake
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool sliceOfCakeUntilDeath { get; set; }

    // Tally Counter
    [Header("tooltips")]
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showHookTooltips { get; set; }

    // Leaf Wings
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showWingTooltips { get; set; }

    // Tungsten Bullet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool ammunitionTooltips { get; set; }

    // Shimmer Arrow
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool shimmerableTooltip { get; set; }


    // Compass
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool worldAndPlayerInfo { get; set; }


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

    // Pump
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool disableShimmerPumping { get; set; }

    // The actual Nurse NPC head
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool overTimeNurseHealing { get; set; }

    // Gold Coin
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(5)]
    public int nurseHealingTime { get; set; }

    // Lens
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool assetFix { get; set; }

    // Cog
    //[BackgroundColor(192, 54, 128, 192)]
    //[DefaultValue(true)]
    //public bool fixMemoryLeaks { get; set; }

    [Header("multiplayer")]

    // Amethyst
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool autoJoinTeam { get; set; }

    // Diamond
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(Team.Pink)]
    [DrawTicks]
    public Team teamToAutoJoin { get; set; }

    // Trifold Map
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool mapSharing { get; set; }


    public override ConfigScope Mode => ConfigScope.ServerSide;
}

public enum Team
{
    White = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Yellow = 4,
    Pink = 5
}