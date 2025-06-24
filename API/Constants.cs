using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using VanillaQoL.Items;

namespace VanillaQoL.API;

public class Constants {
    public const float feetToMetre = (float)1 / 4;
    public const float speedToMph = (float)216000 / 42240;
    public const float mphToKph = 1.609344f;

    public const float FLT_PROJ_TOLERANCE = 0.0001f;

    /// <summary>
    /// List of town slimes.
    /// </summary>
    public static readonly List<int> slimes = [..Enumerable.Range(678, 688 - 678), 670];

    /// <summary>
    /// List of town slimes.
    /// </summary>
    public static readonly List<int> pets = [NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny];

    /// <summary>
    /// List of explosives. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> explosives = [
        ModContent.ProjectileType<BouncyDirtBombProjectile>(),
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    ];

    /// <summary>
    /// List of dynamites. Must be an explosive too. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> dynamites = [
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    ];

    /// <summary>
    /// List of explosives which explode into dirt. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> dirtExplosives = [
        ModContent.ProjectileType<BouncyDirtBombProjectile>(),
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    ];

    /// <summary>
    /// List of explosives which stick to surfaces. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> stickyExplosives = [ModContent.ProjectileType<StickyDirtDynamiteProjectile>()];

    /// <summary>
    /// List of walls which shouldn't be exploded.
    /// </summary>
    /// <returns></returns>
    public static readonly List<int> explosionProofWalls = [
        WallID.BlueDungeonUnsafe, WallID.GreenDungeonUnsafe, WallID.PinkDungeonUnsafe,
        WallID.BlueDungeonSlabUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.PinkDungeonSlabUnsafe,
        WallID.BlueDungeonTileUnsafe, WallID.GreenDungeonTileUnsafe, WallID.PinkDungeonTileUnsafe,
        WallID.BlueDungeon, WallID.GreenDungeon, WallID.PinkDungeon,
        WallID.BlueDungeonSlab, WallID.GreenDungeonSlab, WallID.PinkDungeonSlab,
        WallID.BlueDungeonTile, WallID.GreenDungeonTile, WallID.PinkDungeonTile,
        WallID.LihzahrdBrick, WallID.LihzahrdBrickUnsafe
    ];

    public static readonly List<int> spikes = [
        TileID.Spikes,
        TileID.WoodenSpikes
    ];

    public static readonly List<string> thoriumSummons = [
        "StormFlare",
        "JellyfishResonator",
        "UnstableCore",
        "AncientBlade",
        "StarCaller",
        "StriderTear",
        "VoidLens",
        "AromaticBulb",
        "AbyssalShadow2",
        "DoomSayersCoin"
    ];

    public static readonly List<int> extendedOres = [
        TileID.Silt,
        TileID.Slush,
        TileID.FossilOre,
        TileID.DesertFossil
    ];

    public static readonly List<string> calamityOres = ["SeaPrism"];

    public static readonly List<string> thoriumOres = [
        "DepthsAmber",
        "DepthsAmethyst",
        "DepthsAquamarine",
        "DepthsDiamond",
        "DepthsEmerald",
        "DepthsOpal",
        "DepthsRuby",
        "DepthsSapphire",
        "DepthsTopaz",
        "Aquamarine",
        "Opal"
    ];

    public static readonly List<int> recalls = [
        ItemID.MagicMirror,
        ItemID.IceMirror,
        ItemID.CellPhone,
        ItemID.Shellphone,
        ItemID.ShellphoneSpawn
    ];

    /// <summary>
    /// Accessories which should be usable from a bank.
    /// </summary>
    public static readonly List<int> bankItems = [
        ItemID.DiscountCard,
        ItemID.LuckyCoin,
        ItemID.GoldRing,
        ItemID.CoinRing,
        ItemID.GreedyRing,
        ItemID.MechanicalLens,
        ItemID.LaserRuler,
        ItemID.WireKite,
        ItemID.PDA,
        ItemID.CellPhone,
        ItemID.ShellphoneDummy,
        ItemID.Shellphone,
        ItemID.ShellphoneSpawn,
        ItemID.ShellphoneOcean,
        ItemID.ShellphoneHell,
        ItemID.GPS,
        ItemID.REK,
        ItemID.GoblinTech,
        ItemID.FishFinder,
        ItemID.CopperWatch,
        ItemID.TinWatch,
        ItemID.SilverWatch,
        ItemID.TungstenWatch,
        ItemID.GoldWatch,
        ItemID.PlatinumWatch,
        ItemID.DepthMeter,
        ItemID.Compass,
        ItemID.Radar,
        ItemID.LifeformAnalyzer,
        ItemID.TallyCounter,
        ItemID.MetalDetector,
        ItemID.Stopwatch,
        ItemID.DPSMeter,
        ItemID.FishermansGuide,
        ItemID.WeatherRadio,
        ItemID.Sextant,
        ItemID.AncientChisel,
        ItemID.Toolbelt,
        ItemID.Toolbox,
        ItemID.ExtendoGrip,
        ItemID.PortableCementMixer,
        ItemID.PaintSprayer,
        ItemID.BrickLayer,
        ItemID.ArchitectGizmoPack,
        ItemID.HandOfCreation,
        ItemID.ActuationAccessory,
        ItemID.HighTestFishingLine,
        ItemID.AnglerEarring,
        ItemID.TackleBox,
        ItemID.LavaFishingHook,
        ItemID.AnglerTackleBag,
        ItemID.LavaproofTackleBag,
        ItemID.FishingBobber,
        ItemID.FishingBobberGlowingStar,
        ItemID.FishingBobberGlowingLava,
        ItemID.FishingBobberGlowingKrypton,
        ItemID.FishingBobberGlowingXenon,
        ItemID.FishingBobberGlowingArgon,
        ItemID.FishingBobberGlowingViolet,
        ItemID.FishingBobberGlowingRainbow,
        ItemID.TreasureMagnet,
        ItemID.RoyalGel,
        ItemID.SpectreGoggles,
        ItemID.DontHurtCrittersBook,
        ItemID.DontHurtNatureBook,
        ItemID.DontHurtComboBook,
        ItemID.ShimmerCloak,
        ItemID.DontStarveShaderItem,
        ItemID.EncumberingStone
    ];

    public static readonly List<int> PermanentUpgrades = [
        ItemID.AegisCrystal,
        ItemID.ArcaneCrystal,
        ItemID.AegisFruit,
        ItemID.Ambrosia,
        ItemID.GummyWorm,
        ItemID.GalaxyPearl,
        ItemID.PeddlersSatchel,
        ItemID.ArtisanLoaf,
        ItemID.CombatBook,
        ItemID.CombatBookVolumeTwo,
        ItemID.TorchGodsFavor,
        ItemID.MinecartPowerup
    ];

    public static readonly int[] BossIDs = [
        NPCID.KingSlime,
        NPCID.EyeofCthulhu,
        NPCID.EaterofWorldsHead,
        NPCID.BrainofCthulhu,
        NPCID.QueenBee,
        NPCID.Deerclops,
        NPCID.SkeletronHead,
        NPCID.WallofFlesh,
        NPCID.QueenSlimeBoss,
        NPCID.Retinazer,
        NPCID.Spazmatism,
        NPCID.TheDestroyer,
        NPCID.SkeletronPrime,
        NPCID.Plantera,
        NPCID.Golem,
        NPCID.DukeFishron,
        NPCID.HallowBoss,
        NPCID.CultistBoss,
        NPCID.MoonLordCore
    ];

    public static readonly List<int> VanillaBossAndEventSummons = [
        ItemID.SlimeCrown,
        ItemID.SuspiciousLookingEye,
        ItemID.WormFood,
        ItemID.BloodySpine,
        ItemID.Abeemination,
        ItemID.DeerThing,
        ItemID.QueenSlimeCrystal,
        ItemID.MechanicalWorm,
        ItemID.MechanicalEye,
        ItemID.MechanicalSkull,
        ItemID.MechdusaSummon,
        ItemID.CelestialSigil,
        ItemID.BloodMoonStarter,
        ItemID.GoblinBattleStandard,
        ItemID.PirateMap,
        ItemID.SolarTablet,
        ItemID.SnowGlobe,
        ItemID.PumpkinMoonMedallion,
        ItemID.NaughtyPresent
    ];

    public static readonly List<int> VanillaRightClickBossAndEventSummons = [
        ItemID.LihzahrdPowerCell,
        ItemID.DD2ElderCrystal
    ];

    public static readonly List<int> ModdedBossAndEventSummons = [];

    public static readonly List<int> FargosBossAndEventSummons = [];

    public static readonly ushort[] EvilWallIDs = [
        WallID.CorruptGrassEcho,
        WallID.CorruptGrassUnsafe,
        WallID.CrimsonGrassEcho,
        WallID.CrimsonGrassUnsafe,
        WallID.HallowedGrassEcho,
        WallID.HallowedGrassUnsafe,
        WallID.EbonstoneEcho,
        WallID.EbonstoneUnsafe,
        WallID.CrimstoneEcho,
        WallID.CrimstoneUnsafe,
        WallID.PearlstoneEcho,
        WallID.CorruptHardenedSandEcho,
        WallID.CorruptHardenedSand,
        WallID.CrimsonHardenedSandEcho,
        WallID.CrimsonHardenedSand,
        WallID.HallowHardenedSandEcho,
        WallID.HallowHardenedSand,
        WallID.CorruptSandstoneEcho,
        WallID.CorruptSandstone,
        WallID.CrimsonSandstoneEcho,
        WallID.CrimsonSandstone,
        WallID.HallowSandstoneEcho,
        WallID.HallowSandstone,
        WallID.Corruption1Echo,
        WallID.CorruptionUnsafe1,
        WallID.Corruption2Echo,
        WallID.CorruptionUnsafe2,
        WallID.Corruption3Echo,
        WallID.CorruptionUnsafe3,
        WallID.Corruption4Echo,
        WallID.CorruptionUnsafe4,
        WallID.Crimson1Echo,
        WallID.CrimsonUnsafe1,
        WallID.Crimson2Echo,
        WallID.CrimsonUnsafe2,
        WallID.Crimson3Echo,
        WallID.CrimsonUnsafe3,
        WallID.Crimson4Echo,
        WallID.CrimsonUnsafe4,
        WallID.Hallow1Echo,
        WallID.HallowUnsafe1,
        WallID.Hallow2Echo,
        WallID.HallowUnsafe2,
        WallID.Hallow3Echo,
        WallID.HallowUnsafe3,
        WallID.Hallow4Echo,
        WallID.HallowUnsafe4
    ];

    public static readonly ushort[] PureWallIDs = [
        WallID.Grass,
        WallID.GrassUnsafe,
        WallID.Grass,
        WallID.GrassUnsafe,
        WallID.Grass,
        WallID.GrassUnsafe,
        WallID.Stone,
        WallID.Stone,
        WallID.Stone,
        WallID.Stone,
        WallID.Stone,
        WallID.HardenedSandEcho,
        WallID.HardenedSand,
        WallID.HardenedSandEcho,
        WallID.HardenedSand,
        WallID.HardenedSandEcho,
        WallID.HardenedSand,
        WallID.SandstoneEcho,
        WallID.Sandstone,
        WallID.SandstoneEcho,
        WallID.Sandstone,
        WallID.SandstoneEcho,
        WallID.Sandstone,
        WallID.Dirt1Echo,
        WallID.DirtUnsafe1,
        WallID.Dirt2Echo,
        WallID.DirtUnsafe2,
        WallID.Dirt3Echo,
        WallID.DirtUnsafe3,
        WallID.Dirt4Echo,
        WallID.DirtUnsafe4,
        WallID.Dirt1Echo,
        WallID.DirtUnsafe1,
        WallID.Dirt2Echo,
        WallID.DirtUnsafe2,
        WallID.Dirt3Echo,
        WallID.DirtUnsafe3,
        WallID.Dirt4Echo,
        WallID.DirtUnsafe4,
        WallID.Dirt1Echo,
        WallID.DirtUnsafe1,
        WallID.Dirt2Echo,
        WallID.DirtUnsafe2,
        WallID.Dirt3Echo,
        WallID.DirtUnsafe3,
        WallID.Dirt4Echo,
        WallID.DirtUnsafe4
    ];

    public static readonly HashSet<TileDefinition> DefaultVeinMinerWhitelist = [
        new TileDefinition(TileID.Copper),
        new TileDefinition(TileID.Tin),
        new TileDefinition(TileID.Iron),
        new TileDefinition(TileID.Lead),
        new TileDefinition(TileID.Silver),
        new TileDefinition(TileID.Tungsten),
        new TileDefinition(TileID.Gold),
        new TileDefinition(TileID.Platinum),
        new TileDefinition(TileID.Meteorite),
        new TileDefinition(TileID.Demonite),
        new TileDefinition(TileID.Crimtane),
        new TileDefinition(TileID.Obsidian),
        new TileDefinition(TileID.Hellstone),
        new TileDefinition(TileID.Cobalt),
        new TileDefinition(TileID.Palladium),
        new TileDefinition(TileID.Mythril),
        new TileDefinition(TileID.Orichalcum),
        new TileDefinition(TileID.Adamantite),
        new TileDefinition(TileID.Titanium),
        new TileDefinition(TileID.Chlorophyte),
        new TileDefinition(TileID.LunarOre),
        new TileDefinition(TileID.Amethyst),
        new TileDefinition(TileID.Topaz),
        new TileDefinition(TileID.Sapphire),
        new TileDefinition(TileID.Emerald),
        new TileDefinition(TileID.Ruby),
        new TileDefinition(TileID.Diamond),
        new TileDefinition(TileID.Silt),
        new TileDefinition(TileID.Slush),
        new TileDefinition(TileID.DesertFossil)
    ];

    public static readonly List<int> PowerUpItems = [
        ItemID.Heart,
        ItemID.CandyApple,
        ItemID.CandyCane,
        ItemID.Star,
        ItemID.SoulCake,
        ItemID.SugarPlum,
        ItemID.NebulaPickup1,
        ItemID.NebulaPickup2,
        ItemID.NebulaPickup3
    ];

    public static readonly int[] VanillaFountains = [
        ItemID.PureWaterFountain,
        ItemID.CorruptWaterFountain,
        ItemID.JungleWaterFountain,
        ItemID.HallowedWaterFountain,
        ItemID.IcyWaterFountain,
        ItemID.DesertWaterFountain,
        ItemID.OasisFountain,
        ItemID.CrimsonWaterFountain
    ];

    public static readonly bool[] NormalBunnies = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bunny, NPCID.GemBunnyTopaz,
        NPCID.GemBunnySapphire, NPCID.GemBunnyRuby, NPCID.GemBunnyEmerald, NPCID.GemBunnyDiamond,
        NPCID.GemBunnyAmethyst, NPCID.GemBunnyAmber, NPCID.ExplosiveBunny, NPCID.BunnySlimed, NPCID.BunnyXmas,
        NPCID.CorruptBunny, NPCID.CrimsonBunny, NPCID.PartyBunny);

    public static readonly bool[] NormalSquirrels = NPCID.Sets.Factory.CreateBoolSet(NPCID.Squirrel, NPCID.SquirrelRed,
        NPCID.GemSquirrelTopaz, NPCID.GemSquirrelSapphire, NPCID.GemSquirrelRuby, NPCID.GemSquirrelEmerald,
        NPCID.GemSquirrelDiamond, NPCID.GemSquirrelAmethyst, NPCID.GemSquirrelAmber);

    public static readonly bool[] NormalButterflies =
        NPCID.Sets.Factory.CreateBoolSet(NPCID.Butterfly, NPCID.HellButterfly, NPCID.EmpressButterfly);

    public static readonly bool[] NormalBirds =
        NPCID.Sets.Factory.CreateBoolSet(NPCID.Bird, NPCID.BirdBlue, NPCID.BirdRed);

    public static readonly List<int> Prefixes = [
        PrefixID.Legendary,
        PrefixID.Legendary2,
        PrefixID.Godly,
        PrefixID.Light,
        PrefixID.Rapid,
        PrefixID.Demonic,
        PrefixID.Unreal,
        PrefixID.Mythical,
        PrefixID.Ruthless,
        PrefixID.Warding,
        PrefixID.Arcane,
        PrefixID.Lucky,
        PrefixID.Menacing,
        PrefixID.Quick2,
        PrefixID.Violent
    ];
    
    public static readonly List<Condition> biomeConditions = [
        Condition.InDungeon,
        Condition.InCorrupt,
        Condition.InMeteor,
        Condition.InJungle,
        Condition.InSnow,
        Condition.InCrimson,
        Condition.InTowerSolar,
        Condition.InTowerNebula,
        Condition.InTowerStardust,
        Condition.InDesert,
        Condition.InGlowshroom,
        Condition.InUndergroundDesert,
        Condition.InSkyHeight,
        Condition.InSpace,
        Condition.InOverworldHeight,
        Condition.InDirtLayerHeight,
        Condition.InRockLayerHeight,
        Condition.InUnderworldHeight,
        Condition.InUnderworld,
        Condition.InBeach,
        Condition.InRain,
        Condition.InSandstorm,
        Condition.InOldOneArmy,
        Condition.InGranite,
        Condition.InMarble,
        Condition.InHive,
        Condition.InGemCave,
        Condition.InLihzhardTemple,
        Condition.InGraveyard,
        Condition.InAether,
        Condition.InShoppingZoneForest,
        Condition.InEvilBiome,
        Condition.NotInEvilBiome,
        Condition.NotInHallow,
        Condition.NotInGraveyard,
        Condition.NotInUnderworld
    ];


    #region Boss Drops

    public static readonly int[] kingSlimeDrops = [
        ItemID.SlimySaddle,
        ItemID.NinjaHood,
        ItemID.NinjaShirt,
        ItemID.NinjaPants,
        ItemID.SlimeHook,
        ItemID.SlimeGun
    ];

    public static readonly int[] eyeOfCthulhuDrops = [ItemID.Binoculars];

    public static readonly int[] eaterOfWorldsDrops = [ItemID.EatersBone];

    public static readonly int[] brainOfCthulhuDrops = [ItemID.BoneRattle];

    public static readonly int[] queenBeeDrops = [
        ItemID.BeeGun,
        ItemID.BeeKeeper,
        ItemID.BeesKnees,
        ItemID.HiveWand,
        ItemID.BeeHat,
        ItemID.BeeShirt,
        ItemID.BeePants,
        ItemID.HoneyComb,
        ItemID.Nectar,
        ItemID.HoneyedGoggles
    ];

    public static readonly int[] deerclopsDrops = [
        ItemID.ChesterPetItem,
        ItemID.Eyebrella,
        ItemID.DontStarveShaderItem,
        ItemID.DizzyHat,
        ItemID.PewMaticHorn,
        ItemID.WeatherPain,
        ItemID.HoundiusShootius,
        ItemID.LucyTheAxe
    ];

    public static readonly int[] skeletronDrops = [
        ItemID.SkeletronHand,
        ItemID.BookofSkulls,
        ItemID.ChippysCouch
    ];

    public static readonly int[] wallOfFleshDrops = [
        ItemID.BreakerBlade,
        ItemID.ClockworkAssaultRifle,
        ItemID.LaserRifle,
        ItemID.FireWhip,
        ItemID.WarriorEmblem,
        ItemID.RangerEmblem,
        ItemID.SorcererEmblem,
        ItemID.SummonerEmblem
    ];

    public static readonly int[] queenSlimeDrops = [
        ItemID.CrystalNinjaHelmet,
        ItemID.CrystalNinjaChestplate,
        ItemID.CrystalNinjaLeggings,
        ItemID.Smolstar,
        ItemID.QueenSlimeMountSaddle,
        ItemID.QueenSlimeHook
    ];

    public static readonly int[] planteraDrops = [
        ItemID.GrenadeLauncher,
        ItemID.VenusMagnum,
        ItemID.NettleBurst,
        ItemID.LeafBlower,
        ItemID.FlowerPow,
        ItemID.WaspGun,
        ItemID.Seedler,
        ItemID.PygmyStaff,
        ItemID.ThornHook,
        ItemID.TheAxe,
        ItemID.Seedling
    ];

    public static readonly int[] golemDrops = [
        ItemID.Picksaw,
        ItemID.Stynger,
        ItemID.PossessedHatchet,
        ItemID.SunStone,
        ItemID.EyeoftheGolem,
        ItemID.HeatRay,
        ItemID.StaffofEarth,
        ItemID.GolemFist
    ];

    public static readonly int[] betsyDrops = [
        ItemID.BetsyWings,
        ItemID.DD2BetsyBow,
        ItemID.MonkStaffT3,
        ItemID.ApprenticeStaffT3,
        ItemID.DD2SquireBetsySword
    ];

    public static readonly int[] dukeFishronDrops = [
        ItemID.FishronWings,
        ItemID.BubbleGun,
        ItemID.Flairon,
        ItemID.RazorbladeTyphoon,
        ItemID.TempestStaff,
        ItemID.Tsunami
    ];

    public static readonly int[] empressOfLightDrops = [
        ItemID.FairyQueenMagicItem,
        ItemID.PiercingStarlight,
        ItemID.RainbowWhip,
        ItemID.FairyQueenRangedItem,
        ItemID.RainbowWings,
        ItemID.SparkleGuitar,
        ItemID.RainbowCursor
    ];

    public static readonly int[] moonLordDrops = [
        ItemID.Meowmere,
        ItemID.Terrarian,
        ItemID.StarWrath,
        ItemID.SDMG,
        ItemID.Celeb2,
        ItemID.LastPrism,
        ItemID.LunarFlareBook,
        ItemID.RainbowCrystalStaff,
        ItemID.MoonlordTurretStaff,
        ItemID.MeowmereMinecart
    ];

    #endregion

    public static int AnyPirateBanner;

    public static int AnyArmoredBonesBanner;

    public static int AnySlimeBanner;

    public static int AnyBatBanner;

    public static int AnyHallowBanner;

    public static int AnyCorruptionBanner;

    public static int AnyCrimsonBanner;

    public static int AnyJungleBanner;

    public static int AnySnowBanner;

    public static int AnyDesertBanner;

    public static int AnyUnderworldBanner;

    public enum PlacedPlatformStyles {
        Wood,
        Ebonwood,
        RichMahogany,
        Pearlwood,
        Bone,
        Shadewood,
        BlueBrick,
        PinkBrick,
        GreenBrick,
        MetalShelf,
        BrassShelf,
        WoodShelf,
        DungeonShelf,
        Obsidian,
        Glass,
        Pumpkin,
        SpookyWood,
        PalmWood,
        Mushroom,
        BorealWood,
        Slime,
        Steampunk,
        Skyware,
        LivingWood,
        Honey,
        Cactus,
        Martian,
        Meteorite,
        Granite,
        Marble,
        Crystal,
        Golden,
        DynastyWood,
        Lihzahrd,
        Flesh,
        Frozen,
        Spider,
        Lesion,
        Solar,
        Vortex,
        Nebula,
        Stardust,
        Sandstone,
        Stone,
        Bamboo,
        Reef,
        Balloon,
        AshWood,
        Echo,
    }

    public enum PlacedTableStyles1 {
        Wooden,
        Ebonwood,
        RichMahogany,
        Pearlwood,
        Bone,
        Flesh,
        LivingWood,
        Skyware,
        Shadewood,
        Lihzahrd,
        BlueDungeon,
        GreenDungeon,
        PinkDungeon,
        Obsidian,
        Gothic,
        Glass,
        Banquet,
        Bar,
        Golden,
        Honey,
        Steampunk,
        Pumpkin,
        Spooky,
        Pine,
        Frozen,
        Dynasty,
        PalmWood,
        Mushroom,
        BorealWood,
        Slime,
        Cactus,
        Martian,
        Meteorite,
        Granite,
        Marble
    }

    public enum PlacedTableStyles2 {
        Crystal,
        Spider,
        Lesion,
        Solar,
        Vortex,
        Nebula,
        Stardust,
        Sandstone,
        Bamboo,
        Reef,
        Balloon,
        AshWood
    }

    public enum PlacedChairStyles {
        Wooden,
        Tiolet,
        Ebonwood,
        RichMahogany,
        Pearlwood,
        LivingWood,
        Cactus,
        Bone,
        Flesh,
        Mushroom,
        Skyware,
        Shadewood,
        Lihzahrd,
        BlueDungeon,
        GreenDungeon,
        PinkDungeon,
        Obsidian,
        Gothic,
        Glass,
        Golden,
        GoldenToilet,
        BarStool,
        Honey,
        Steampunk,
        Pumpkin,
        Spooky,
        Pine,
        Dynasty,
        Frozen,
        PalmWood,
        BorealWood,
        Slime,
        Martian,
        Meteorite,
        Granite,
        Marble,
        Crystal,
        Spider,
        Lesion,
        Solar,
        Vortex,
        Nebula,
        Stardust,
        Sandstone,
        Bamboo
    }

    public enum PlacedDoorStyles {
        Wooden,
        Ebonwood,
        RichMahogany,
        Pearlwood,
        Cactus,
        Flesh,
        Mushroom,
        LivingWood,
        Bone,
        Skyware,
        Shadewood,
        LockedLihzahrd,
        Lihzahrd,
        Dungeon,
        Lead,
        Iron,
        BlueDungeon,
        GreenDungeon,
        PinkDungeon,
        Obsidian,
        Glass,
        Golden,
        Honey,
        Steampunk,
        Pumpkin,
        Spooky,
        Pine,
        Frozen,
        Dynasty,
        PalmWood,
        BorealWood,
        Slime,
        Martian,
        Meteorite,
        Granite,
        Marble,
        Crystal,
        Spider,
        Lesion,
        Solar,
        Vortex,
        Nebula,
        Stardust,
        Sandstone,
        Stone,
        Bamboo
    }

    public enum PlacedTorchStyles {
        Torch,
        BlueTorch,
        RedTorch,
        GreenTorch,
        PurpleTorch,
        WhiteTorch,
        YellowTorch,
        DemonTorch,
        CursedTorch,
        IceTorch,
        OrangeTorch,
        IchorTorch,
        UltrabrightTorch,
        BoneTorch,
        RainbowTorch,
        PinkTorch,
        DesertTorch,
        CoralTorch,
        CorruptTorch,
        CrimsonTorch,
        HallowedTorch,
        JungleTorch,
        MushroomTorch,
        AetherTorch
    }

    public static void postSetup() {
        List<int> ModBankItems = [
            GetModItem(ModContentCompat.aequusMod, "AnglerBroadcaster"),
            GetModItem(ModContentCompat.aequusMod, "Calendar"),
            GetModItem(ModContentCompat.aequusMod, "GeigerCounter"),
            GetModItem(ModContentCompat.aequusMod, "HoloLens"),
            GetModItem(ModContentCompat.aequusMod, "RichMansMonocle"),
            GetModItem(ModContentCompat.aequusMod, "DevilsTongue"),
            GetModItem(ModContentCompat.aequusMod, "NeonGenesis"),
            GetModItem(ModContentCompat.aequusMod, "RadonFishingBobber"),
            GetModItem(ModContentCompat.aequusMod, "Ramishroom"),
            GetModItem(ModContentCompat.aequusMod, "RegrowingBait"),
            GetModItem(ModContentCompat.aequusMod, "LavaproofMitten"),
            GetModItem(ModContentCompat.aequusMod, "BusinessCard"),
            GetModItem(ModContentCompat.aequusMod, "HaltingMachine"),
            GetModItem(ModContentCompat.aequusMod, "HaltingMagnet"),
            GetModItem(ModContentCompat.aequusMod, "HyperJet"),
            GetModItem(ModContentCompat.afkpetsMod, "FishermansPride"),
            GetModItem(ModContentCompat.afkpetsMod, "LampyridaeHairpin"),
            GetModItem(ModContentCompat.afkpetsMod, "Piracy"),
            GetModItem(ModContentCompat.afkpetsMod, "PortableSonar"),
            GetModItem(ModContentCompat.afkpetsMod, "TheHandyman"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "AttendanceLog"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "BiomeCrystal"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "EngiRegistry"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "FortuneMirror"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "HitMarker"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "Magimeter"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "RSH"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "SafteyScanner"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "ScryingMirror"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "ThreatAnalyzer"),
            GetModItem(ModContentCompat.blocksInfoAccessoriesMod, "WantedPoster"),
            GetModItem(ModContentCompat.calamityMod, "AlluringBait"),
            GetModItem(ModContentCompat.calamityMod, "EnchantedPearl"),
            GetModItem(ModContentCompat.calamityMod, "SupremeBaitTackleBoxFishingStation"),
            GetModItem(ModContentCompat.calamityMod, "AncientFossil"),
            GetModItem(ModContentCompat.calamityMod, "OceanCrest"),
            GetModItem(ModContentCompat.calamityMod, "SpelunkersAmulet"),
            GetModItem(ModContentCompat.clickerClassMod, "ButtonMasher"),
            GetModItem(ModContentCompat.luiAFKMod, "FasterMining"),
            GetModItem(ModContentCompat.luiAFKMod, "SuperToolTime"),
            GetModItem(ModContentCompat.luiAFKMod, "ToolTime"),
            GetModItem(ModContentCompat.martainsOrderMod, "ArmorDisplayer"),
            GetModItem(ModContentCompat.martainsOrderMod, "FlightTimer"),
            GetModItem(ModContentCompat.martainsOrderMod, "Journal"),
            GetModItem(ModContentCompat.martainsOrderMod, "IronWatch"),
            GetModItem(ModContentCompat.martainsOrderMod, "LeadWatch"),
            GetModItem(ModContentCompat.martainsOrderMod, "LeprechaunSensor"),
            GetModItem(ModContentCompat.martainsOrderMod, "MinionCounter"),
            GetModItem(ModContentCompat.martainsOrderMod, "SentryCounter"),
            GetModItem(ModContentCompat.martainsOrderMod, "SummonersTracker"),
            GetModItem(ModContentCompat.martainsOrderMod, "SurvivalTracker"),
            //GetModItem(ModContentCompat.moomoosUltimateYoyoRevampMod, "HitDisplay"),
            //GetModItem(ModContentCompat.moomoosUltimateYoyoRevampMod, "SpeedDisplay"),
            GetModItem(ModContentCompat.spiritMod, "FisheyeGem"),
            GetModItem(ModContentCompat.spiritMod, "MetalBand"),
            GetModItem(ModContentCompat.spiritMod, "MimicRepellent"),
            GetModItem(ModContentCompat.thoriumMod, "HeartRateMonitor"),
            GetModItem(ModContentCompat.thoriumMod, "HightechSonarDevice"),
            GetModItem(ModContentCompat.thoriumMod, "GlitteringChalice"),
            GetModItem(ModContentCompat.thoriumMod, "GreedyGoblet"),
            GetModItem(ModContentCompat.thoriumMod, "LuckyRabbitsFoot")
        ];

        bankItems.AddRangeN1(ModBankItems);

        List<int> TempModdedBossAndEventSummons = [
            GetModItem(ModContentCompat.aequusMod, "GalacticStarfruit"),
            //AFKPETS
            GetModItem(ModContentCompat.afkpetsMod, "AncientSand"),
            GetModItem(ModContentCompat.afkpetsMod, "BlackenedHeart"),
            GetModItem(ModContentCompat.afkpetsMod, "BrokenDelftPlate"),
            GetModItem(ModContentCompat.afkpetsMod, "CookingBook"),
            GetModItem(ModContentCompat.afkpetsMod, "CorruptedServer"),
            GetModItem(ModContentCompat.afkpetsMod, "DemonicAnalysis"),
            GetModItem(ModContentCompat.afkpetsMod, "DesertMirror"),
            GetModItem(ModContentCompat.afkpetsMod, "DuckWhistle"),
            GetModItem(ModContentCompat.afkpetsMod, "FallingSlimeReplica"),
            GetModItem(ModContentCompat.afkpetsMod, "FrozenSkull"),
            GetModItem(ModContentCompat.afkpetsMod, "GoldenKingSlimeIdol"),
            GetModItem(ModContentCompat.afkpetsMod, "GoldenSkull"),
            GetModItem(ModContentCompat.afkpetsMod, "HaniwaIdol"),
            GetModItem(ModContentCompat.afkpetsMod, "HolographicSlimeReplica"),
            GetModItem(ModContentCompat.afkpetsMod, "IceBossCrystal"),
            GetModItem(ModContentCompat.afkpetsMod, "MagicWand"),
            GetModItem(ModContentCompat.afkpetsMod, "NightmareFuel"),
            GetModItem(ModContentCompat.afkpetsMod, "PinkDiamond"),
            GetModItem(ModContentCompat.afkpetsMod, "PlantAshContainer"),
            GetModItem(ModContentCompat.afkpetsMod, "PreyTrackingChip"),
            GetModItem(ModContentCompat.afkpetsMod, "RoastChickenPlate"),
            GetModItem(ModContentCompat.afkpetsMod, "SeveredClothierHead"),
            GetModItem(ModContentCompat.afkpetsMod, "SeveredDryadHead"),
            GetModItem(ModContentCompat.afkpetsMod, "SeveredHarpyHead"),
            GetModItem(ModContentCompat.afkpetsMod, "ShogunSlimesHelmet"),
            GetModItem(ModContentCompat.afkpetsMod, "SlimeinaGlassCube"),
            GetModItem(ModContentCompat.afkpetsMod, "SlimyWarBanner"),
            GetModItem(ModContentCompat.afkpetsMod, "SoulofAgonyinaBottle"),
            GetModItem(ModContentCompat.afkpetsMod, "SpineWormFood"),
            GetModItem(ModContentCompat.afkpetsMod, "SpiritofFunPot"),
            GetModItem(ModContentCompat.afkpetsMod, "SpiritualHeart"),
            GetModItem(ModContentCompat.afkpetsMod, "StoryBook"),
            GetModItem(ModContentCompat.afkpetsMod, "SuspiciousLookingChest"),
            GetModItem(ModContentCompat.afkpetsMod, "SwissChocolate"),
            GetModItem(ModContentCompat.afkpetsMod, "TiedBunny"),
            GetModItem(ModContentCompat.afkpetsMod, "TinyMeatIdol"),
            GetModItem(ModContentCompat.afkpetsMod, "TradeDeal"),
            GetModItem(ModContentCompat.afkpetsMod, "UnstableRainbowCookie"),
            GetModItem(ModContentCompat.afkpetsMod, "UntoldBurial"),
            //Awful Garbage
            GetModItem(ModContentCompat.awfulGarbageMod, "InsectOnAStick"),
            GetModItem(ModContentCompat.awfulGarbageMod, "PileOfFakeBones"),
            //Blocks Core Boss
            GetModItem(ModContentCompat.blocksCoreBossMod, "ChargedOrb"),
            GetModItem(ModContentCompat.blocksCoreBossMod, "ChargedOrbCrim"),
            //Consolaria
            GetModItem(ModContentCompat.consolariaMod, "SuspiciousLookingEgg"),
            GetModItem(ModContentCompat.consolariaMod, "CursedStuffing"),
            GetModItem(ModContentCompat.consolariaMod, "SuspiciousLookingSkull"),
            GetModItem(ModContentCompat.consolariaMod, "Wishbone"),
            //Coralite
            GetModItem(ModContentCompat.coraliteMod, "RedBerry"),
            //Edorbis
            GetModItem(ModContentCompat.edorbisMod, "BiomechanicalMatter"),
            GetModItem(ModContentCompat.edorbisMod, "CursedSoul"),
            GetModItem(ModContentCompat.edorbisMod, "KelviniteRadar"),
            GetModItem(ModContentCompat.edorbisMod, "SlayerTrophy"),
            GetModItem(ModContentCompat.edorbisMod, "ThePrettiestFlower"),
            //Enchanted Moons
            GetModItem(ModContentCompat.enchantedMoonsMod, "BlueMedallion"),
            GetModItem(ModContentCompat.enchantedMoonsMod, "CherryAmulet"),
            GetModItem(ModContentCompat.enchantedMoonsMod, "HarvestLantern"),
            GetModItem(ModContentCompat.enchantedMoonsMod, "MintRing"),
            //Everjade
            GetModItem(ModContentCompat.everjadeMod, "FestivalLantern"),
            //Excelsior
            GetModItem(ModContentCompat.excelsiorMod, "ReflectiveIceShard"),
            GetModItem(ModContentCompat.excelsiorMod, "PlanetaryTrackingDevice"),
            //Exxo Avalon Origins
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "BloodyAmulet"),
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "InfestedCarcass"),
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "DesertHorn"),
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "GoblinRetreatOrder"),
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "FalseTreasureMap"),
            GetModItem(ModContentCompat.exxoAvalonOriginsMod, "OddFertilizer"),
            //Gensokyo
            GetModItem(ModContentCompat.gensokyoMod, "AliceMargatroidSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "CirnoSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "EternityLarvaSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "HinaKagiyamaSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "KaguyaHouraisanSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "LilyWhiteSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "MayumiJoutouguuSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "MedicineMelancholySpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "MinamitsuMurasaSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "NazrinSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "NitoriKawashiroSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "RumiaSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "SakuyaIzayoiSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "SeijaKijinSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "SeiranSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "SekibankiSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "TenshiHinanawiSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "ToyosatomimiNoMikoSpawner"),
            GetModItem(ModContentCompat.gensokyoMod, "UtsuhoReiujiSpawner"),
            //Homeward Journey
            GetModItem(ModContentCompat.homewardJourneyMod, "CannedSoulofFlight"),
            GetModItem(ModContentCompat.homewardJourneyMod, "MetalSpine"),
            GetModItem(ModContentCompat.homewardJourneyMod, "SouthernPotting"),
            GetModItem(ModContentCompat.homewardJourneyMod, "SunlightCrown"),
            GetModItem(ModContentCompat.homewardJourneyMod, "UltimateTorch"),
            GetModItem(ModContentCompat.homewardJourneyMod, "UnstableGlobe"),
            //Martains Order
            GetModItem(ModContentCompat.martainsOrderMod, "FrigidEgg"),
            GetModItem(ModContentCompat.martainsOrderMod, "SuspiciousLookingCloud"),
            GetModItem(ModContentCompat.martainsOrderMod, "Catnip"),
            GetModItem(ModContentCompat.martainsOrderMod, "CarnageSuspiciousRazor"),
            GetModItem(ModContentCompat.martainsOrderMod, "VoidWorm"),
            GetModItem(ModContentCompat.martainsOrderMod, "LuminiteSlimeCrown"),
            GetModItem(ModContentCompat.martainsOrderMod, "LuminiteEye"),
            GetModItem(ModContentCompat.martainsOrderMod, "JunglesLastTreasure"),
            GetModItem(ModContentCompat.martainsOrderMod, "TeslaRemote"),
            GetModItem(ModContentCompat.martainsOrderMod, "BloodyNight"),
            GetModItem(ModContentCompat.martainsOrderMod, "LucidDay"),
            GetModItem(ModContentCompat.martainsOrderMod, "LucidFestival"),
            GetModItem(ModContentCompat.martainsOrderMod, "LucidNight"),
            //Medial Rift
            GetModItem(ModContentCompat.medialRiftMod, "RemoteOfTheMetalHeads"),
            //Metroid Mod
            GetModItem(ModContentCompat.metroidMod, "GoldenTorizoSummon"),
            GetModItem(ModContentCompat.metroidMod, "KraidSummon"),
            GetModItem(ModContentCompat.metroidMod, "NightmareSummon"),
            GetModItem(ModContentCompat.metroidMod, "OmegaPirateSummon"),
            GetModItem(ModContentCompat.metroidMod, "PhantoonSummon"),
            GetModItem(ModContentCompat.metroidMod, "SerrisSummon"),
            GetModItem(ModContentCompat.metroidMod, "TorizoSummon"),
            //Ophioid
            GetModItem(ModContentCompat.ophioidMod, "DeadFungusbug"),
            GetModItem(ModContentCompat.ophioidMod, "InfestedCompost"),
            GetModItem(ModContentCompat.ophioidMod, "LivingCarrion"),
            //Qwerty
            GetModItem(ModContentCompat.qwertyMod, "AncientEmblem"),
            GetModItem(ModContentCompat.qwertyMod, "B4Summon"),
            GetModItem(ModContentCompat.qwertyMod, "BladeBossSummon"),
            GetModItem(ModContentCompat.qwertyMod, "DinoEgg"),
            //GetModItem(ModContentCompat.qwertyMod, "FortressBossSummon"),
            //GetModItem(ModContentCompat.qwertyMod, "GodSealKeycard"),
            GetModItem(ModContentCompat.qwertyMod, "HydraSummon"),
            GetModItem(ModContentCompat.qwertyMod, "RitualInterupter"),
            GetModItem(ModContentCompat.qwertyMod, "SummoningRune"),
            //Redemption
            GetModItem(ModContentCompat.redemptionMod, "EaglecrestSpelltome"),
            GetModItem(ModContentCompat.redemptionMod, "EggCrown"),
            GetModItem(ModContentCompat.redemptionMod, "FowlWarHorn"),
            //Secrets of the Shadows
            GetModItem(ModContentCompat.secretsOfTheShadowsMod, "ElectromagneticLure"),
            GetModItem(ModContentCompat.secretsOfTheShadowsMod, "SuspiciousLookingCandle"),
            GetModItem(ModContentCompat.secretsOfTheShadowsMod, "JarOfPeanuts"),
            GetModItem(ModContentCompat.secretsOfTheShadowsMod, "CatalystBomb"),
            //Shadows of Abaddon
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "PumpkinLantern"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "PrimordiaSummon"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "AbaddonSummon"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "SerpentSummon"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "SoranEmblem"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "HeirsAuthority"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "PigmanBanner"),
            GetModItem(ModContentCompat.shadowsOfAbaddonMod, "SandstormMedallion"),
            //Spirit
            GetModItem(ModContentCompat.spiritMod, "DistressJellyItem"),
            GetModItem(ModContentCompat.spiritMod, "GladeWreath"),
            GetModItem(ModContentCompat.spiritMod, "ReachBossSummon"),
            GetModItem(ModContentCompat.spiritMod, "JewelCrown"),
            GetModItem(ModContentCompat.spiritMod, "BlackPearl"),
            GetModItem(ModContentCompat.spiritMod, "BlueMoonSpawn"),
            GetModItem(ModContentCompat.spiritMod, "DuskCrown"),
            GetModItem(ModContentCompat.spiritMod, "CursedCloth"),
            GetModItem(ModContentCompat.spiritMod, "StoneSkin"),
            GetModItem(ModContentCompat.spiritMod, "MartianTransmitter"),
            //Spooky
            GetModItem(ModContentCompat.spookyMod, "Fertilizer"),
            GetModItem(ModContentCompat.spookyMod, "RottenSeed"),
            //Storms Additions
            GetModItem(ModContentCompat.stormsAdditionsMod, "AridBossSummon"),
            GetModItem(ModContentCompat.stormsAdditionsMod, "MoonlingSummoner"),
            GetModItem(ModContentCompat.stormsAdditionsMod, "StormBossSummoner"),
            GetModItem(ModContentCompat.stormsAdditionsMod, "UltimateBossSummoner"),
            //Supernova
            GetModItem(ModContentCompat.supernovaMod, "BugOnAStick"),
            GetModItem(ModContentCompat.supernovaMod, "EerieCrystal"),
            //Thorium
            GetModItem(ModContentCompat.thoriumMod, "StormFlare"),
            GetModItem(ModContentCompat.thoriumMod, "JellyfishResonator"),
            GetModItem(ModContentCompat.thoriumMod, "UnstableCore"),
            GetModItem(ModContentCompat.thoriumMod, "AncientBlade"),
            GetModItem(ModContentCompat.thoriumMod, "StarCaller"),
            GetModItem(ModContentCompat.thoriumMod, "StriderTear"),
            GetModItem(ModContentCompat.thoriumMod, "VoidLens"),
            GetModItem(ModContentCompat.thoriumMod, "AromaticBulb"),
            GetModItem(ModContentCompat.thoriumMod, "AbyssalShadow2"),
            GetModItem(ModContentCompat.thoriumMod, "DoomSayersCoin"),
            GetModItem(ModContentCompat.thoriumMod, "FreshBrain"),
            GetModItem(ModContentCompat.thoriumMod, "RottingSpore"),
            GetModItem(ModContentCompat.thoriumMod, "IllusionaryGlass"),
            //Utric
            GetModItem(ModContentCompat.uhtricMod, "RareGeode"),
            GetModItem(ModContentCompat.uhtricMod, "SnowyCharcoal"),
            GetModItem(ModContentCompat.uhtricMod, "CosmicLure"),
            //Universe of Swords
            GetModItem(ModContentCompat.universeOfSwordsMod, "SwordBossSummon"),
            //Valhalla
            GetModItem(ModContentCompat.valhallaMod, "HeavensSeal"),
            GetModItem(ModContentCompat.valhallaMod, "HellishRadish"),
            //Vitality
            GetModItem(ModContentCompat.vitalityMod, "CloudCore"),
            GetModItem(ModContentCompat.vitalityMod, "AncientCrown"),
            GetModItem(ModContentCompat.vitalityMod, "MultigemCluster"),
            GetModItem(ModContentCompat.vitalityMod, "MoonlightLotusFlower"),
            GetModItem(ModContentCompat.vitalityMod, "Dreadcandle"),
            GetModItem(ModContentCompat.vitalityMod, "MeatyMushroom"),
            GetModItem(ModContentCompat.vitalityMod, "AnarchyCrystal"),
            GetModItem(ModContentCompat.vitalityMod, "TotemofChaos"),
            GetModItem(ModContentCompat.vitalityMod, "MartianRadio"),
            GetModItem(ModContentCompat.vitalityMod, "SpiritBox"),
            //Wayfair
            GetModItem(ModContentCompat.wayfairContentMod, "MagicFertilizer"),
            //Zylon
            GetModItem(ModContentCompat.zylonMod, "ForgottenFlame"),
            GetModItem(ModContentCompat.zylonMod, "SlimyScepter")
        ];
        ModdedBossAndEventSummons.AddRangeN1(TempModdedBossAndEventSummons);

        List<int> TempFargosBossAndEventSummons = [
            GetModItem(ModContentCompat.fargosMutantMod, "Anemometer"),
            GetModItem(ModContentCompat.fargosMutantMod, "BatteredClub"),
            GetModItem(ModContentCompat.fargosMutantMod, "BetsyEgg"),
            GetModItem(ModContentCompat.fargosMutantMod, "FestiveOrnament"),
            GetModItem(ModContentCompat.fargosMutantMod, "ForbiddenScarab"),
            GetModItem(ModContentCompat.fargosMutantMod, "ForbiddenTome"),
            GetModItem(ModContentCompat.fargosMutantMod, "HeadofMan"),
            GetModItem(ModContentCompat.fargosMutantMod, "IceKingsRemains"),
            GetModItem(ModContentCompat.fargosMutantMod, "MartianMemoryStick"),
            GetModItem(ModContentCompat.fargosMutantMod, "MatsuriLantern"),
            GetModItem(ModContentCompat.fargosMutantMod, "NaughtyList"),
            GetModItem(ModContentCompat.fargosMutantMod, "PartyInvite"),
            GetModItem(ModContentCompat.fargosMutantMod, "PillarSummon"),
            GetModItem(ModContentCompat.fargosMutantMod, "RunawayProbe"),
            GetModItem(ModContentCompat.fargosMutantMod, "SlimyBarometer"),
            GetModItem(ModContentCompat.fargosMutantMod, "SpentLantern"),
            GetModItem(ModContentCompat.fargosMutantMod, "SpookyBranch"),
            GetModItem(ModContentCompat.fargosMutantMod, "SuspiciousLookingScythe"),
            GetModItem(ModContentCompat.fargosMutantMod, "WeatherBalloon"),
            //DEVIANTT NPC ITEMS
            GetModItem(ModContentCompat.fargosMutantMod, "AmalgamatedSkull"),
            GetModItem(ModContentCompat.fargosMutantMod, "AmalgamatedSpirit"),
            GetModItem(ModContentCompat.fargosMutantMod, "AthenianIdol"),
            GetModItem(ModContentCompat.fargosMutantMod, "AttractiveOre"),
            GetModItem(ModContentCompat.fargosMutantMod, "BloodSushiPlatter"),
            GetModItem(ModContentCompat.fargosMutantMod, "BloodUrchin"),
            GetModItem(ModContentCompat.fargosMutantMod, "CloudSnack"),
            GetModItem(ModContentCompat.fargosMutantMod, "ClownLicense"),
            GetModItem(ModContentCompat.fargosMutantMod, "CoreoftheFrostCore"),
            GetModItem(ModContentCompat.fargosMutantMod, "CorruptChest"),
            GetModItem(ModContentCompat.fargosMutantMod, "CrimsonChest"),
            GetModItem(ModContentCompat.fargosMutantMod, "DemonicPlushie"),
            GetModItem(ModContentCompat.fargosMutantMod, "DilutedRainbowMatter"),
            GetModItem(ModContentCompat.fargosMutantMod, "Eggplant"),
            GetModItem(ModContentCompat.fargosMutantMod, "ForbiddenForbiddenFragment"),
            GetModItem(ModContentCompat.fargosMutantMod, "GnomeHat"),
            GetModItem(ModContentCompat.fargosMutantMod, "GoblinScrap"),
            GetModItem(ModContentCompat.fargosMutantMod, "GoldenSlimeCrown"),
            GetModItem(ModContentCompat.fargosMutantMod, "GrandCross"),
            GetModItem(ModContentCompat.fargosMutantMod, "HallowChest"),
            GetModItem(ModContentCompat.fargosMutantMod, "HeartChocolate"),
            GetModItem(ModContentCompat.fargosMutantMod, "HemoclawCrab"),
            GetModItem(ModContentCompat.fargosMutantMod, "HolyGrail"),
            GetModItem(ModContentCompat.fargosMutantMod, "JungleChest"),
            GetModItem(ModContentCompat.fargosMutantMod, "LeesHeadband"),
            GetModItem(ModContentCompat.fargosMutantMod, "MothLamp"),
            GetModItem(ModContentCompat.fargosMutantMod, "MothronEgg"),
            GetModItem(ModContentCompat.fargosMutantMod, "Pincushion"),
            GetModItem(ModContentCompat.fargosMutantMod, "PinkSlimeCrown"),
            GetModItem(ModContentCompat.fargosMutantMod, "PirateFlag"),
            GetModItem(ModContentCompat.fargosMutantMod, "PlunderedBooty"),
            GetModItem(ModContentCompat.fargosMutantMod, "RuneOrb"),
            GetModItem(ModContentCompat.fargosMutantMod, "ShadowflameIcon"),
            GetModItem(ModContentCompat.fargosMutantMod, "SlimyLockBox"),
            GetModItem(ModContentCompat.fargosMutantMod, "SuspiciousLookingChest"),
            GetModItem(ModContentCompat.fargosMutantMod, "SuspiciousLookingLure"),
            GetModItem(ModContentCompat.fargosMutantMod, "WormSnack"),
            //MUTANT NPC ITEMS
            GetModItem(ModContentCompat.fargosMutantMod, "Abeemination2"),
            GetModItem(ModContentCompat.fargosMutantMod, "AncientSeal"),
            GetModItem(ModContentCompat.fargosMutantMod, "CelestialSigil2"),
            GetModItem(ModContentCompat.fargosMutantMod, "CultistSummon"),
            GetModItem(ModContentCompat.fargosMutantMod, "DeathBringerFairy"),
            GetModItem(ModContentCompat.fargosMutantMod, "DeerThing2"),
            GetModItem(ModContentCompat.fargosMutantMod, "FleshyDoll"),
            GetModItem(ModContentCompat.fargosMutantMod, "GoreySpine"),
            GetModItem(ModContentCompat.fargosMutantMod, "JellyCrystal"),
            GetModItem(ModContentCompat.fargosMutantMod, "LihzahrdPowerCell2"),
            GetModItem(ModContentCompat.fargosMutantMod, "MechanicalAmalgam"),
            GetModItem(ModContentCompat.fargosMutantMod, "MechEye"),
            GetModItem(ModContentCompat.fargosMutantMod, "MechSkull"),
            GetModItem(ModContentCompat.fargosMutantMod, "MechWorm"),
            GetModItem(ModContentCompat.fargosMutantMod, "MutantVoodoo"),
            GetModItem(ModContentCompat.fargosMutantMod, "PlanterasFruit"),
            GetModItem(ModContentCompat.fargosMutantMod, "PrismaticPrimrose"),
            GetModItem(ModContentCompat.fargosMutantMod, "SlimyCrown"),
            GetModItem(ModContentCompat.fargosMutantMod, "SuspiciousEye"),
            GetModItem(ModContentCompat.fargosMutantMod, "SuspiciousSkull"),
            GetModItem(ModContentCompat.fargosMutantMod, "TruffleWorm2"),
            GetModItem(ModContentCompat.fargosMutantMod, "WormyFood"),
            //Fargos Souls
            GetModItem(ModContentCompat.fargosSoulsMod, "AbomsCurse"),
            GetModItem(ModContentCompat.fargosSoulsMod, "ChampionySigil"),
            GetModItem(ModContentCompat.fargosSoulsMod, "CoffinSummon"),
            GetModItem(ModContentCompat.fargosSoulsMod, "DevisCurse"),
            GetModItem(ModContentCompat.fargosSoulsMod, "FragilePixieLamp"),
            GetModItem(ModContentCompat.fargosSoulsMod, "MechLure"),
            GetModItem(ModContentCompat.fargosSoulsMod, "MutantsCurse"),
            GetModItem(ModContentCompat.fargosSoulsMod, "SquirrelCoatofArms"),
            //Fargos DLC
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "AbandonedRemote"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "ABombInMyNation"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "AstrumCor"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "BirbPheromones"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "BlightedEye"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "BloodyWorm"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "ChunkyStardust"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "ClamPearl"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "ColossalTentacle"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "CryingKey"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "DeepseaProteinShake"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "DefiledCore"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "DefiledShard"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "DragonEgg"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "EyeofExtinction"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "FriedDoll"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "HiveTumor"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "LetterofKos"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "MaulerSkull"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "MedallionoftheDesert"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "MurkySludge"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "NoisyWhistle"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "NuclearChunk"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "OphiocordycipitaceaeSprout"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "PlaguedWalkieTalkie"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "PolterplasmicBeacon"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "PortableCodebreaker"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "QuakeIdol"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "RedStainedWormFood"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "RiftofKos"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "SeeFood"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "SirensPearl"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "SomeKindofSpaceWorm"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "StormIdol"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "SulphurBearTrap"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "WormFoodofKos"),
            GetModItem(ModContentCompat.fargosSoulsDLCMod, "WyrmTablet"),
            //Fargos Extras
            GetModItem(ModContentCompat.fargosSoulsExtrasMod, "PandorasBox")
        ];
        FargosBossAndEventSummons.AddRangeN1(TempFargosBossAndEventSummons);

        List<int> ModPrefixes = [
            GetModPrefix(ModContentCompat.calamityMod, "Flawless"),
            GetModPrefix(ModContentCompat.calamityMod, "Silent"),
            GetModPrefix(ModContentCompat.clickerClassMod, "Elite"),
            GetModPrefix(ModContentCompat.clickerClassMod, "ClickerRadius"),
            GetModPrefix(ModContentCompat.martainsOrderMod, "StrikerPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "EmpyreanPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "EtherealPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "BlockingPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "BrewingPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "LoadedPrefix"),
            GetModPrefix(ModContentCompat.orchidMod, "SpiritualPrefix"),
            GetModPrefix(ModContentCompat.secretsOfTheShadowsMod, "Omnipotent"),
            GetModPrefix(ModContentCompat.secretsOfTheShadowsMod, "Omniscient"),
            GetModPrefix(ModContentCompat.secretsOfTheShadowsMod, "Soulbound"),
            GetModPrefix(ModContentCompat.thoriumMod, "Fabled"),
            GetModPrefix(ModContentCompat.thoriumMod, "Engrossing"),
            GetModPrefix(ModContentCompat.thoriumMod, "Lucrative"),
            GetModPrefix(ModContentCompat.vitalityMod, "MalevolentPrefix"),
            GetModPrefix(ModContentCompat.vitalityMod, "RelentlessPrefix")
        ];
        Prefixes.AddRangeN1(ModPrefixes);

        if (ModContentCompat.calamityLoaded) {
            CalamityConstants.calamityBiomes(biomeConditions);
        }
    }

    public static bool isSpike(int type) {
        return spikes.Contains(type);
    }

    public static bool isSticky(int type) {
        return type is TileID.JungleThorns or TileID.CorruptThorns or TileID.CrimsonThorns or TileID.PlanteraThorns;
    }


    public static bool isDrill(int type) {
        return ItemID.Sets.IsDrill[type] || ItemID.Sets.IsChainsaw[type] || type == ItemID.ChlorophyteJackhammer;
    }

    public static bool isSummon(Item item) {
        var group = ContentSamples.CreativeHelper.GetItemGroup(item, out _);
        // important, to apply modded categories
        ItemLoader.ModifyResearchSorting(item, ref group);
        // that's vanilla and well-behaving mods taken care of
        if (group is ContentSamples.CreativeHelper.ItemGroup.BossItem
            or ContentSamples.CreativeHelper.ItemGroup.EventItem) {
            return true;
        }

        // false positive as well, but those won't be consumables anyway, right?..... right?
        if (ItemID.Sets.SortingPriorityBossSpawns[item.type] != -1) {
            if (item.type != ItemID.ManaCrystal && item.type != ItemID.LifeCrystal && item.type != ItemID.LifeFruit) {
                return true;
            }
        }

        // onto modded!
        if (item.ModItem != null) {
            var mod = item.ModItem.Mod;
            var name = item.ModItem.Name;
            return isModdedSummon(mod, name);
        }

        return false;
    }

    public static bool isModdedSummon(Mod mod, string name) {
        // Calamity is a good boy
        if (mod.Name == "ThoriumMod") {
            return thoriumSummons.Contains(name);
        }

        return false;
    }

    public static bool isOre(Tile tile) {
        var type = tile.TileType;
        if (TileID.Sets.Ore[type] || isGem(tile) || type == TileID.LunarOre) {
            return true;
        }

        if (QoLConfig.Instance.extendedOres && extendedOres.Contains(type)) {
            return true;
        }

        var modTile = ModContent.GetModTile(type);
        if (modTile != null) {
            var mod = modTile.Mod;
            var name = modTile.Name;
            return isModdedOre(mod, name);
        }

        return false;
    }

    public static bool isGem(Tile tile) {
        return tile.TileType is >= TileID.Sapphire and <= TileID.Diamond or TileID.AmberStoneBlock
            or TileID.ExposedGems;
    }

    public static bool isModdedOre(Mod mod, string name) {
        if (mod.Name == "ThoriumMod") {
            return thoriumOres.Contains(name);
        }

        if (mod.Name == "CalamityMod") {
            return calamityOres.Contains(name);
        }

        return false;
    }

    public static bool isWing(Item item) {
        return item.wingSlot > 0;
    }

    public static bool isBalloon(Item item) {
        return item.balloonSlot > 0;
    }

    public static bool isBottle(Item item) {
        return item.type is ItemID.CloudinaBottle or ItemID.BlizzardinaBottle or ItemID.SandstorminaBottle
            or ItemID.TsunamiInABottle or ItemID.FartinaJar;
    }

    public static bool isRecall(Item item) {
        return recalls.Contains(item.type);
    }

    public static int GetModItem(Mod mod, string itemName) {
        if (mod != null) {
            if (mod.TryFind(itemName, out ModItem currItem) && currItem != null) {
                return currItem.Type;
            }
        }

        return ItemID.None;
    }

    public static int GetModProjectile(Mod mod, string projName) {
        if (mod != null) {
            if (mod.TryFind(projName, out ModProjectile currProj) && currProj != null) {
                return currProj.Type;
            }
        }

        return ProjectileID.None;
    }

    public static int GetModNPC(Mod mod, string npcName) {
        if (mod != null) {
            if (mod.TryFind(npcName, out ModNPC currNPC) && currNPC != null) {
                return currNPC.Type;
            }
        }

        return NPCID.None;
    }

    public static int GetModTile(Mod mod, string tileName) {
        if (mod != null) {
            if (mod.TryFind(tileName, out ModTile currTile) && currTile != null) {
                return currTile.Type;
            }
        }

        return -1;
    }

    public static int GetModWall(Mod mod, string wallName) {
        if (mod != null) {
            if (mod.TryFind(wallName, out ModWall currWall) && currWall != null) {
                return currWall.Type;
            }
        }

        return WallID.None;
    }

    public static int GetModBuff(Mod mod, string buffName) {
        if (mod != null) {
            if (mod.TryFind(buffName, out ModBuff currBuff) && currBuff != null) {
                return currBuff.Type;
            }
        }

        return -1;
    }

    public static int GetModPrefix(Mod mod, string prefixName) {
        if (mod != null) {
            if (mod.TryFind(prefixName, out ModPrefix currPrefix) && currPrefix != null) {
                return currPrefix.Type;
            }
        }

        return -1;
    }

    public static DamageClass GetModDamageClass(Mod mod, string className) {
        if (mod != null) {
            if (mod.TryFind(className, out DamageClass currClass) && currClass != null) {
                return currClass;
            }
        }

        return DamageClass.Default;
    }
}

[JITWhenModsEnabled("CalamityMod")]
public static class CalamityConstants {
    /// <summary>
    /// Add the Calamity biomes to the list of biome conditions.
    /// </summary>
    public static void calamityBiomes(List<Condition> biomeConditions) {
        List<Condition> calamityBiomes = [
            CalamityConditions.InAstral,
            CalamityConditions.InCrag,
            CalamityConditions.InSulph,
            CalamityConditions.InSunken,
        ];
        biomeConditions.AddRange(calamityBiomes);
    }
}