using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZenithQoL.Items;

namespace ZenithQoL.API;

public class Constants {
    public const float feetToMetre = (float)1 / 4;
    public const float speedToMph = (float)216000 / 42240;
    public const float mphToKph = 1.609344f;

    public const float FLT_PROJ_TOLERANCE = 0.0001f;

    /// <summary>
    /// List of town slimes.
    /// </summary>
    public static readonly List<int> slimes = new(Enumerable.Range(678, 688 - 678)) { 670 };

    /// <summary>
    /// List of town slimes.
    /// </summary>
    public static readonly List<int> pets = new() { NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny };

    /// <summary>
    /// List of explosives. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> explosives = new() {
        ModContent.ProjectileType<BouncyDirtBombProjectile>(),
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    };

    /// <summary>
    /// List of dynamites. Must be an explosive too. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> dynamites = new() {
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    };

    /// <summary>
    /// List of explosives which explode into dirt. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> dirtExplosives = new() {
        ModContent.ProjectileType<BouncyDirtBombProjectile>(),
        ModContent.ProjectileType<DirtDynamiteProjectile>(),
        ModContent.ProjectileType<BouncyDirtDynamiteProjectile>(),
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    };

    /// <summary>
    /// List of explosives which stick to surfaces. Used for aiStyle 16 spoofing.
    /// </summary>
    public static readonly List<int> stickyExplosives = new() {
        ModContent.ProjectileType<StickyDirtDynamiteProjectile>()
    };

    /// <summary>
    /// List of walls which shouldn't be exploded.
    /// </summary>
    /// <returns></returns>
    public static readonly List<int> explosionProofWalls = new() {
        WallID.BlueDungeonUnsafe, WallID.GreenDungeonUnsafe, WallID.PinkDungeonUnsafe,
        WallID.BlueDungeonSlabUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.PinkDungeonSlabUnsafe,
        WallID.BlueDungeonTileUnsafe, WallID.GreenDungeonTileUnsafe, WallID.PinkDungeonTileUnsafe,
        WallID.BlueDungeon, WallID.GreenDungeon, WallID.PinkDungeon,
        WallID.BlueDungeonSlab, WallID.GreenDungeonSlab, WallID.PinkDungeonSlab,
        WallID.BlueDungeonTile, WallID.GreenDungeonTile, WallID.PinkDungeonTile,
        WallID.LihzahrdBrick, WallID.LihzahrdBrickUnsafe
    };

    public static readonly List<int> spikes = new() {
        TileID.Spikes,
        TileID.WoodenSpikes
    };

    public static readonly List<string> thoriumSummons = new() {
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
    };

    public static readonly List<int> extendedOres = new() {
        TileID.Silt,
        TileID.Slush,
        TileID.FossilOre,
        TileID.DesertFossil
    };

    public static readonly List<string> calamityOres = new() {
        "SeaPrism"
    };

    public static readonly List<string> thoriumOres = new() {
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
        "Opal",
    };

    public static readonly List<int> recalls = new() {
        ItemID.MagicMirror,
        ItemID.IceMirror,
        ItemID.CellPhone,
        ItemID.Shellphone,
        ItemID.ShellphoneSpawn
    };

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
}