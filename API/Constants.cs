using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
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
    public static readonly List<int> slimes = new(Enumerable.Range(678, 688 - 678)) { 670 };

    /// <summary>
    /// List of town slimes.
    /// </summary>
    public static readonly List<int> pets = new() {NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny};

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


    public static bool isDrill(int type) {
        return ItemID.Sets.IsDrill[type] || ItemID.Sets.IsChainsaw[type] || type == ItemID.ChlorophyteJackhammer;
    }
}