using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class SliceOfCake : ModSystem {

    private static bool prev;
    private static bool prev2;
    private static bool prev3;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.sliceOfCakeUntilDeath;
    }

    public override void Load() {
        IL_Player.TileInteractionsUse += sliceOfCakePatch;
    }

    public override void SetStaticDefaults() {
        prev = BuffID.Sets.TimeLeftDoesNotDecrease[BuffID.SugarRush];
        prev2 = Main.buffNoTimeDisplay[BuffID.SugarRush];
        prev3 = Main.buffNoSave[BuffID.SugarRush];
        BuffID.Sets.TimeLeftDoesNotDecrease[BuffID.SugarRush] = true;
        Main.buffNoTimeDisplay[BuffID.SugarRush] = true;
        Main.buffNoSave[BuffID.SugarRush] = true;
    }

    public override void Unload() {
        IL_Player.TileInteractionsUse -= sliceOfCakePatch;
        BuffID.Sets.TimeLeftDoesNotDecrease[BuffID.SugarRush] = prev;
        Main.buffNoTimeDisplay[BuffID.SugarRush] = prev2;
        Main.buffNoSave[BuffID.SugarRush] = prev3;
    }

    // IL_11f0: ldc.i4       192 // 0x000000c0
    // IL_11f5: ldc.i4       7200 // 0x00001c20
    public static void sliceOfCakePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(BuffID.SugarRush), i => i.MatchLdcI4(out _))) {
            ilCursor.Prev.Operand = 108000;
        }
    }
}