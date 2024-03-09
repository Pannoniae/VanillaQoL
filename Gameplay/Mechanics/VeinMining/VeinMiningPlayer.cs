using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ID;

namespace VanillaQoL.Gameplay.VeinMining;

public class VeinMiningPlayer : ModPlayer {
    public int ctr;
    private bool _canMine;
    private int cd;
    private int mcd;

    public static int miningSpeed => QoLConfig.Instance.veinMiningSpeed;

    private PriorityQueue<Point16, double> picks = new();

    // to stop cheating :P
    public int pickPower;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.veinMining;
    }

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

    public override void ProcessTriggers(TriggersSet triggersSet) {
        if (VeinMiningSystem.toggle.JustPressed) {
            VeinMiningSystem.toggleState = !VeinMiningSystem.toggleState;
        }
    }


    public override void PreUpdate() {
        // reflection
        var GetPickaxeDamage =
            typeof(Player).GetMethod("GetPickaxeDamage", BindingFlags.Instance | BindingFlags.NonPublic)!;
        cd--;
        mcd--;
        if (cd == 0) {
            canMine = true;
        }

        if (mcd <= 0) {
            var success = picks.TryDequeue(out var tile, out var _);
            if (success) {
                var x = tile.X;
                var y = tile.Y;
                int dmg = (int)GetPickaxeDamage.Invoke(Player, new object[] { x, y, pickPower, 0, Main.tile[x, y] })!;
                if (!WorldGen.CanKillTile(x, y)) {
                    dmg = 0;
                }

                if (dmg != 0) {
                    WorldGen.KillTile(tile.X, tile.Y);
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.TileManipulation, number2: tile.X, number3: tile.Y);
                    }

                    mcd = miningSpeed;
                }
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