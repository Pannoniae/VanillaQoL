using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class HighlightInfo : GlobalTile {
    public override bool? IsTileDangerous(int i, int j, int type, Player player) {
        if (QoLConfig.Instance.siltIsDangerous && type is TileID.Silt or TileID.Slush) {
            return true;
        }

        if (QoLConfig.Instance.thinIceNotDangerous && type == TileID.BreakableIce) {
            return false;
        }

        return base.IsTileDangerous(i, j, type, player);
    }

    public override bool? IsTileSpelunkable(int i, int j, int type) {
        if (QoLConfig.Instance.spelunkerHellstone && type == TileID.Hellstone) {
            return true;
        }

        return base.IsTileSpelunkable(i, j, type);
    }
}