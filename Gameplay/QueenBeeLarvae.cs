using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class QueenBeeLarvae : ModSystem {
    private static bool prev;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.queenBeeLarvaeBreak;
    }

    public override void Load()
    {
        prev = Main.tileCut[TileID.Larva];
        Main.tileCut[TileID.Larva] = false;
    }

    public override void Unload() => Main.tileCut[TileID.Larva] = prev;
}