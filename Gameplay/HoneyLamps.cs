using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ZenithQoL.Gameplay;

public class HoneyLamps : GlobalTile {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.honeyImmuneHoneyLamps;
    }

    public override void SetStaticDefaults() {
        var data = TileObjectData.GetTileData(TileID.Lamps, 11);
        data.WaterDeath = false;
        data.WaterPlacement = LiquidPlacement.Allowed;
    }
}