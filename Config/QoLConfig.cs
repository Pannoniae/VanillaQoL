using System.ComponentModel;
using Terraria.ModLoader.Config;
using VanillaQoL.Shared;

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
    public bool townNPCsSpawningAtNight { get; set; }

    // Universal Pylon
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool townNPCsMovingAtDay { get; set; }

    // Cat License
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool townPetsInvincible { get; set; }

    // Gold Coin
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool shopExpander { get; set; }

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

    // x button
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool enableLockOn { get; set; }

    // x button
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool lockOn { get; set; }

    // Ironskin Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(44)]
    [Range(0, 936)]
    public int moreBuffSlots { get; set; }

    // Tinkerer's Workshop
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool disablePrefixChangingRarity { get; set; }

    // Pink Dungeon Brick
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool softDungeonBricks { get; set; }

    // DPS Meter
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool accessoryLoadoutSupport { get; set; }

    // Vortex Drill
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool drillRework { get; set; }

    // Nature's Gift
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool naturesGiftGlow { get; set; }

    // Suspicious Looking Eye
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool nonConsumableSummons { get; set; }

    // Cobalt Drill
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool veinMining { get; set; }

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

    // Terrarium
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool modNameTooltip { get; set; }


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

    [Header("tweaks")]
    // Work Bench
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool invertRecipeListScrolling { get; set; }

    // Sand Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool noDroppedSand { get; set; }

    // Snow Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool noDroppedSnow { get; set; }

    // Snow Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool noDroppedTombstones { get; set; }

    // Abeemination
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool queenBeeLarvaeBreak { get; set; }

    // Slice of Cake
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool sliceOfCakeUntilDeath { get; set; }

    // Lihzahrd Brick
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool wallsDontExplode { get; set; }

    // Titanium Ore
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool hardModeOresCanExplode { get; set; }

    // DCU
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool dynamicDCUPickaxe { get; set; }

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
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fixMemoryLeaks { get; set; }

    // Celestial Sigil
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(Credits.AfterModsFinalBoss)]
    [DrawTicks]
    [ReloadRequired]
    public Credits moveCredits { get; set; }

    [Header("content")]

    // Bouncy Dirt Bomb
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool moreExplosives { get; set; }

    // Terra Blade
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ancientSwords { get; set; }

    // Snow Globe
    [Header("holidayCheer")]
    // Pine Tree
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool christmasAllYear { get; set; }

    // Jack'O Lantern
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool halloweenAllYear { get; set; }

    // Magma Skull
    [Header("cheats")]

    // Demon Heart
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool accessorySlotUnlock { get; set; }

    // Torch
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool NPCsLiveInEvilBiomes { get; set; }

    // Life Crystal
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool lifeCrystalGlow { get; set; }

    // Life Fruit
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool lifeFruitGlow { get; set; }

    // Sandgun
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool noDroppedSandgun { get; set; }

    // Slimy Saddle
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool mountFallDamage { get; set; }

    // Aetherium Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool disableShimmering { get; set; }


    [Header("suffering")]
    // Sandgun
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool jungleMimic { get; set; }

    // Clown
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool clownExplosions { get; set; }

    // Snow Globe
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool naturalFrostLegion { get; set; }

    // Door
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool enemiesBreakDoors { get; set; }


    // Tungsten Clock
    [Header("respawn")]
    // Fast Clock
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool respawningRework { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(3)]
    [Range(0, 60f)]
    [Increment(0.25f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float respawnTime { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(6)]
    [Range(0, 60f)]
    [Increment(0.25f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float eventRespawnTime { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(10)]
    [Range(0, 60f)]
    [Increment(0.25f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float bossRespawnTime { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(1.5f)]
    [Range(0, 3f)]
    [Increment(0.1f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float respawnFactorExpertMode { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(1f)]
    [Range(0, 3f)]
    [Increment(0.1f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float respawnFactorMasterMode { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(1f)]
    [Range(0, 3f)]
    [Increment(0.1f)]
    [CustomModConfigItem(typeof(FloatInputElement))]
    public float bossMultiplayerMultiplier { get; set; }

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

    [Header("testing")]

    // Dirt
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool testing { get; set; }

    public override void OnChanged() {

        if (nonConsumableSummons) {
            GlobalFeatures.enableFeature(Mod, "nonConsumableSummons");
        }
        else {
            GlobalFeatures.disableFeature(Mod, "nonConsumableSummons");
        }
    }
    public override ConfigScope Mode => ConfigScope.ServerSide;
}

public enum Credits {
    Vanilla,
    AfterModsFinalBoss,
    Disabled
}

public enum Team {
    White = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Yellow = 4,
    Pink = 5
}