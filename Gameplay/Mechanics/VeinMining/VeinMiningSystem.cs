using System.Collections.Generic;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.VeinMining;

public class VeinMiningSystem : ModSystem {

    public static ModKeybind toggle;
    public static bool toggleState;

    public static int threshold = 120;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.veinMining;
    }

    public override void Load() {
        IL_Player.PickTile += pickTilePatch;
        toggle = KeybindLoader.RegisterKeybind(Mod, "VeinMineToggleKey", "N");
        toggleState = true;
    }

    public override void Unload() {
        IL_Player.PickTile -= pickTilePatch;
    }

    // [38928 9 - 38928 32]
    // IL_02be: ldarg.1      // x
    // IL_02bf: ldarg.2      // y
    // IL_02c0: ldc.i4.0
    // IL_02c1: ldc.i4.0
    // IL_02c2: ldc.i4.0
    // IL_02c3: call         void Terraria.WorldGen::KillTile(int32, int32, bool, bool, bool)
    public void pickTilePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg1(),
                i => i.MatchLdarg2(),
                i => i.MatchLdcI4(0),
                i => i.MatchLdcI4(0),
                i => i.MatchLdcI4(0),
                i => i.MatchCall<WorldGen>("KillTile"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the actual WorldGen.KillTile call in Player.PickTile!");
            return;
        }

        ilCursor.EmitLdarg0();
        ilCursor.EmitCall<VeinMiningSystem>("clearCD");
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.EmitLdarg2();
        ilCursor.EmitLdarg3();
        ilCursor.EmitCall<VeinMiningSystem>("veinMine");
    }

    public static void clearCD(Player player) {
        // we reset counter *before* mining. Could do with additional parameter to veinMine functional-style
        // but eh, injecting another call is way easier
        var modPlayer = player.GetModPlayer<VeinMiningPlayer>();
        modPlayer.ctr = 0;
    }

    public static void veinMine(Player player, int x, int y, int pickPower) {
        // if this is ore, don't even bother so we won't fall into recursion
        var tile = Framing.GetTileSafely(x, y);
        var modPlayer = player.GetModPlayer<VeinMiningPlayer>();

        if (tile.HasTile && toggleState && canVeinMine(tile) && modPlayer.canMine) {
            modPlayer.pickPower = pickPower;
            foreach (var coords in tileRot(x, y)) {
                var tile2 = Framing.GetTileSafely(coords.x, coords.y);

                bool notInQueue = modPlayer.notInQueue(coords.x, coords.y);

                if (tile2.HasTile && canVeinMine(tile2) && notInQueue) {
                    // handle limits
                    modPlayer.ctr++;
                    if (modPlayer.ctr > threshold) {
                        VanillaQoL.instance.Logger.Warn("Reached threshold!");
                        modPlayer.ctr = 0;
                        modPlayer.canMine = false;
                    }

                    // queue it first
                    modPlayer.queueTile(coords.x, coords.y);
                    // recursion!
                    veinMine(player, coords.x, coords.y, pickPower);
                }
            }
        }
    }

    public static bool canVeinMine(Tile tile) {
        return Constants.isOre(tile) || (QoLConfig.Instance.veinMineSpikes && Constants.isSpike(tile.TileType));
    }

    // the other kind of rot
    public static IEnumerable<(int x, int y)> tileRot(int x, int y) {
        for (int i = x - 1; i <= x + 1; ++i) {
            for (int j = y - 1; j <= y + 1; ++j) {
                if (i != x || j != y) {
                    yield return (i, j);
                }
            }
        }
    }
}