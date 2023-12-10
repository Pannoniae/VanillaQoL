using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class CrackingDungeonBricks : GlobalTile {
    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
        // thank you Calamity!
        var tile = Main.tile[i, j];

        // we have zero business here
        if (!QoLConfig.Instance.softDungeonBricks || Main.netMode == NetmodeID.MultiplayerClient ||
            tile.TileType < TileID.CrackedBlueDungeonBrick || tile.TileType > TileID.CrackedPinkDungeonBrick) {
            return;
        }

        for (int m = 0; m < 8; m++) {
            int x = i;
            int y = j;
            switch (m) {
                case 0:
                    x--;
                    break;
                case 1:
                    x++;
                    break;
                case 2:
                    y--;
                    break;
                case 3:
                    y++;
                    break;
                case 4:
                    x--;
                    y--;
                    break;
                case 5:
                    x++;
                    y--;
                    break;
                case 6:
                    x--;
                    y++;
                    break;
                case 7:
                    x++;
                    y++;
                    break;
            }

            var neighbourTile = Main.tile[x, y];
            breakBrick(tile, neighbourTile, x, y);
        }
    }

    private static void breakBrick(Tile tile, Tile neighbourTile, int x, int y) {
        if (neighbourTile is { HasTile: true, TileType: >= TileID.CrackedBlueDungeonBrick } &&
            neighbourTile.TileType <= TileID.CrackedPinkDungeonBrick) {
            tile.HasTile = false;
            WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: true);
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.TrySendData(MessageID.TileManipulation, -1, -1, null, 20, x, y);
            }
        }
    }
}