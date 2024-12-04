using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class NPCsLiveInEvilBiomes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.NPCsLiveInEvilBiomes;
    }

    public override void Load() {
        IL_WorldGen.ScoreRoom += NpcHouseScorePatch;
        IL_ShopHelper.LimitAndRoundMultiplier += increaseUnhappinessCapPatch;
    }

    public override void Unload() {
        IL_WorldGen.ScoreRoom -= NpcHouseScorePatch;
        IL_ShopHelper.LimitAndRoundMultiplier -= increaseUnhappinessCapPatch;
    }

    // [1417 9 - 1417 101]
    // IL_005e: ldc.i4.4
    // IL_005f: call         int32 Terraria.WorldGen::GetTileTypeCountByCategory(int32[], valuetype Terraria.Enums.TileScanGroup)
    // IL_0064: neg
    // IL_0065: stloc.s      num2
    public void NpcHouseScorePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<WorldGen>("GetTileTypeCountByCategory"))) {
            ZenithQoL.instance.Logger.Warn("Couldn't find GetTileTypeCountByCategory in WorldGen.ScoreRoom!");
            return;
        }
        // what if we just didn't?
        ilCursor.Index++;
        ilCursor.EmitPop();
        ilCursor.Remove();
    }

    public void increaseUnhappinessCapPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // [166 7 - 166 71]
        //IL_0000: ldarg.1      // priceAdjustment
        //IL_0001: ldc.r4       0.75
        //IL_0006: ldc.r4       1.5
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.5f))) {
            ilCursor.Next!.Operand = 2.0f;
        }
        else {
            ZenithQoL.instance.Logger.Warn("Couldn't find price cap in ShopHelper.LimitAndRoundMultiplier!");

        }
    }
}