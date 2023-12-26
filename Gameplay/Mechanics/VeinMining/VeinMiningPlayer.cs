using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace VanillaQoL.Gameplay.VeinMining;

public class VeinMiningPlayer : ModPlayer {
    public int ctr;
    private bool _canMine;
    private int cd;
    private int mcd;

    public static int miningSpeed = 4;

    private PriorityQueue<Point16, double> picks = new();

    public override void Initialize() {
        canMine = true;
    }

    public bool canMine {
        get => _canMine;
        set {
            cd = 60;
            _canMine = value;
        }
    }


    public override void PreUpdate() {
        cd--;
        mcd--;
        if (cd == 0) {
            canMine = true;
        }

        if (mcd <= 0) {
            var success = picks.TryDequeue(out var tile, out var _);
            if (success) {
                WorldGen.KillTile(tile.X, tile.Y);
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    NetMessage.SendData(MessageID.TileManipulation, number2: tile.X, number3: tile.Y);
                }

                mcd = miningSpeed;
            }
        }
    }

    public void queueTile(int x, int y) {
        var prio = Player.Distance(new Vector2(x * 16, y * 16));
        picks.Enqueue(new Point16(x, y), prio);
    }

    public bool notInQueue(int x, int y) {
        return !contains(picks, new Point16(x, y));
    }

    private bool contains<T, U>(PriorityQueue<T, U> priorityQueue, T item) {
        return priorityQueue.UnorderedItems.Any(el => el.Element!.Equals(item));
    }
}