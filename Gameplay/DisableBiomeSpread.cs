using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader; 

namespace VanillaQoL.Gameplay;

public class DisableBiomeSpread : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableBiomeSpread;
    }

    public override void Load() {
        IL_WorldGen.UpdateWorld_Inner += DisableBiomeSpreadPatch;
    }

    public override void Unload() {
        IL_WorldGen.UpdateWorld_Inner -= DisableBiomeSpreadPatch;
    }


    // IL_0000: ldc.i4.1
    // IL_0001: stsfld       bool Terraria.WorldGen::AllowedToSpreadInfections

    // IL_0023: ceq
    // IL_0025: stsfld       bool Terraria.WorldGen::AllowedToSpreadInfections
    private void DisableBiomeSpreadPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1),
                i => i.MatchStsfld<WorldGen>("AllowedToSpreadInfections"))) {
            VanillaQoL.instance.Logger.Error("Failed to find AllowedToSpreadInfections in WorldGen.UpdateWorld_Inner!");
            return;
        }
        ilCursor.Next!.OpCode = OpCodes.Ldc_I4_0;

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchCeq(),
                i => i.MatchStsfld<WorldGen>("AllowedToSpreadInfections"))) {
            VanillaQoL.instance.Logger.Error("Failed to find AllowedToSpreadInfections in WorldGen.UpdateWorld_Inner!");
            return;
        }
        ilCursor.Index++;
        ilCursor.EmitPop();
        ilCursor.Emit(OpCodes.Ldc_I4_0); // Set AllowedToSpreadInfections to false
    }
}