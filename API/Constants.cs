using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace VanillaQoL.API;

public class Constants {
    public const float feetToMetre = (float)1 / 4;
    public const float speedToMph = (float)216000 / 42240;
    public const float mphToKph = 1.609344f;

    /// <summary>
    /// List of town slimes. (npc.type ID)
    /// </summary>
    public static readonly List<int> slimes = new(Enumerable.Range(678, 688 - 678)) { 670 };

    public static bool isDrill(int type) {
        return ItemID.Sets.IsDrill[type] || ItemID.Sets.IsChainsaw[type] || type == ItemID.ChlorophyteJackhammer;
    }
}