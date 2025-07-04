using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using VanillaQoL.Shared;

namespace VanillaQoL.Config;

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

[BackgroundColor(16, 0, 2, 1)]
public class QoLConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static QoLConfig Instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public override ConfigScope Mode => ConfigScope.ServerSide;

    #region Gameplay

    [Header("gameplay")]

    // Gold Coin
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool shopExpander { get; set; }

    // Silver Coin
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool extraSellPages { get; set; }

    [BackgroundColor(192, 64, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool sellAdditionalItems { get; set; }

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

    // Tinkerer's Workshop
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool disablePrefixChangingRarity { get; set; }

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

    // Suspicious Looking Eye
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool nonConsumableSummons { get; set; }

    // Cobalt Drill
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool veinMining { get; set; }

    // Copper Pickaxe
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(3)]
    [Range(0, 15)]
    [ReloadRequired]
    public int veinMiningSpeed { get; set; }

    // Silt Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool extendedOres { get; set; }


    // Leaf Wings
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool wingSlot { get; set; }

    // Wood Platform
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool instantPlatformFallthrough { get; set; }
    
    // Defender Medal
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool OOAdrops { get; set; }
    
    // Defender Medal
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool OOAdrops2 { get; set; }

    #endregion
    
    #region NPCs
    
    [Header("npcs")]
    
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
    
    // World Globe
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool removeBiomeRequirementsShops { get; set; }

    // Goblin Tinkerer
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool boundNPCProtection { get; set; }
    
    // Goblin Battle Standard
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(0)]
    [Range(0, 10)]
    [Slider]
    [ReloadRequired]
    public int npcDeathsToCallOffInvasion { get; set; }
    
    // Gravestone
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(0)]
    [Range(0, 10)]
    [Slider]
    [ReloadRequired]
    public int playerDeathsToCallOffInvasion { get; set; }
    
    #endregion
    
    #region Combat
    
    [Header("combat")]
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(DamageSpread.PlayersEnemies)]
    [ReloadRequired]
    public DamageSpread damageSpread { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(15)]
    [Range(0, 100)]
    [Slider]
    [ReloadRequired]
    public int damageSpreadAmount { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool superCrits { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool superCritsColour { get; set; }
    
    
    // public static readonly Color DamagedFriendly = new Color(255, 80, 90, 255);
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(typeof (Color), "255, 40, 50, 255")]
    public Color superCritsColourValue { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool superCritsColouredDamageTypesIntegration { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(90)]
    [ReloadRequired]
    public int superCritsColouredDamageTypesDarken { get; set; }
    
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool critsBypassDefense { get; set; }

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool exactTooltips { get; set; }
    
    #endregion

    #region Information

    [Header("information")]

    // Compass
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool metricSystem { get; set; }

    // Platinum Watch
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool twentyFourHourTime { get; set; }

    // Ivy Whip
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

    // Ladybug
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showLuck { get; set; }


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

    // Cobalt Shield
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool drShield { get; set; }
    
    // Terrarian
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool removeOneDrop { get; set; }

    // The Undertaker
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool grammarFix { get; set; }

    #endregion

    #region Tweaks

    [Header("tweaks")]

    // Work Bench
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(true)]
    public bool invertRecipeListScrolling { get; set; }

    // Ironskin Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(44)]
    [Range(0, 936)]
    public int moreBuffSlots { get; set; }

    // Nature's Gift
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool naturesGiftGlow { get; set; }

    // Sand Block#
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
    
    // Crystal Ball
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool deathLessBuffs { get; set; }

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
    
    // Moonglow Seeds
    //[BackgroundColor(192, 54, 128, 192)]
    //[DefaultValue(true)]
    //[ReloadRequired]
    //public bool axeReplantingHerbs { get; set; }

    // Copper Axe
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool axeReplanting { get; set; }

    // Regeneration Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool quickBuffFavourite { get; set; }

    // Heart
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool noFullHeartPickup { get; set; }

    // Heart
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(300)]
    [Range(0, 5000)]
    [ReloadRequired]
    public int heartTime { get; set; }

    // Shrimpy Truffle
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool truffleKingAndQueenStatue { get; set; }

    // Gravitation Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool persistentBuffs { get; set; }

    // Panic Necklace
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool panicKey { get; set; }

    // Honey Lamp
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool honeyImmuneHoneyLamps { get; set; }

    // Dirt Wall
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool mineWallsFromInside { get; set; }

    // Lava Slime
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool noLavaSlime { get; set; }

    // Encumbering Stone
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool coincumberingStone { get; set; }

    // Encumbering Stone
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool coincumberingStoneRename { get; set; }
    
    // Blazing Wheel
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool killableDungeonNPCs { get; set; }
    
    // Red's Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool removeDevSet { get; set; }
    
    // Guide to Plant Fiber Cordage
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool quickBestiary { get; set; }
    
    // Fallen Star
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool autoResearchFavourite { get; set; }
    
    // Piggy Bank
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool itemsWorkInBanks { get; set; }
    
    // Money Trough
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool itemsWorkInBanksTooltip { get; set; }
    
    // Golden Carp
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool stackableQuestItems { get; set; }
    
    
    // Womannequin
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool allHairsOnStart { get; set; }
    
    // Dangersense Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool siltIsDangerous { get; set; }
    
    // Dangersense Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool thinIceNotDangerous { get; set; }
    
    // Hellstone
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool spelunkerHellstone { get; set; }
    
    // Treasure Bag (Plantera)
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool moreBossLoot { get; set; }
    
    // Void Bag
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool stackVoidBag { get; set; }
    
    // Money Trough
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool storageFollowsPlayer { get; set; }
    
    #endregion

    #region Fixes

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
    [ReloadRequired]
    public bool fixMemoryLeaks { get; set; }

    // Celestial Sigil
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(Credits.AfterModsFinalBoss)]
    [DrawTicks]
    [ReloadRequired]
    public Credits moveCredits { get; set; }

    // Tinkerer's Workshop
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool calamityInWorldRarity { get; set; }
    
    // Fallen Star
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool fixStarlightRiver { get; set; }

    #endregion

    #region Content

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

    // Hallow Pylon
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool morePylons { get; set; }
    
    // Asphalt Platform
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool asphaltPlatform { get; set; }

    #endregion

    #region Holiday Cheer

    [Header("holidayCheer")]

    // Pine Tree
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool christmasAllYear { get; set; }

    // Jack'O Lantern
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool halloweenAllYear { get; set; }

    // Santa Hat
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool stopSantaFromExploding { get; set; }

    #endregion

    #region Cheats

    [Header("cheats")]
    
    // Sparkle Slime Balloon
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool disableNPCHappiness { get; set; }
    
    // Sparkle Slime Balloon
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(0.8f)]
    [Slider]
    [Increment(0.01f)]
    [Range(0, 1.2f)]
    public float disableNPCHappinessConstant { get; set; }

    // Demon Heart
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool accessorySlotUnlock { get; set; }

    // Pink Dungeon Brick
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool softDungeonBricks { get; set; }

    // Torch
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool NPCsLiveInEvilBiomes { get; set; }

    // Clentaminator
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool disableBiomeSpread { get; set; }

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

    // Wooden Spike
    [BackgroundColor(192, 54, 128, 192)]
    [Range(0, 15)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool veinMineSpikes { get; set; }

    // Moon Lord Relic
    [BackgroundColor(192, 54, 128, 192)]
    [ReloadRequired]
    [DefaultValue(false)]
    public bool soyMasterMode { get; set; }
    
    // Pure Water Fountain
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool fountainBiomes { get; set; }
    
    // Oasis Water Fountain
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool fountainFromInventory { get; set; }
    
    // Plantera Relic
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool masterModeRelicsInExpert { get; set; }
    
    
    // idk how to implement this properly so todo lol
    // Void Vault
    //[BackgroundColor(192, 54, 128, 192)]
    //[DefaultValue(false)]
    //[ReloadRequired]
    //public bool moreVoidVault { get; set; }

    #endregion

    #region Suffering

    [Header("suffering")]

    // Sandgun
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool jungleMimic { get; set; }

    // Clown
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
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

    // Cactus
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool cactusHurts { get; set; }

    // Vine (210)
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool jungleThornsAreSticky { get; set; }


    // Cat 637
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool pannoniaeCat { get; set; }

    #endregion

    #region Respawning

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

    #endregion

    #region Multiplayer

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

    #endregion

    #region testing

    [Header("testing")]

    // Trifold Map
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool mapSharingTESTING { get; set; }

    // Dirt
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool testing { get; set; }

    #endregion
    public override void OnChanged() {
        if (sellAdditionalItems) {
            GlobalFeatures.enableFeature(Mod, "NPCShops");
        }
        else {
            GlobalFeatures.disableFeature(Mod, "NPCShops");
        }

        if (nonConsumableSummons) {
            GlobalFeatures.enableFeature(Mod, "nonConsumableSummons");
        }
        else {
            GlobalFeatures.disableFeature(Mod, "nonConsumableSummons");
        }
    }
}

public enum DamageSpread {
    /// Damage spread is off for everyone.
    Off,
    /// Damage spread is only for enemies, not for the player.
    Enemies,
    /// Damage spread is for everyone.
    PlayersEnemies
}