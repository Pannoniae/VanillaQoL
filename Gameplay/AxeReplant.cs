using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class AxeReplant : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.axeReplanting;
    }

    public override void Load() {
        IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += axePlantingPatch;
    }

    public override void Unload() {
        IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool -= axePlantingPatch;
    }
    // [33083 13 - 33083 43]
    // IL_03ec: ldarg.1      // sItem
    // IL_03ed: ldfld        int32 Terraria.Item::'type'
    // IL_03f2: ldc.i4       5295 // 0x000014af
    // IL_03f7: ceq
    public void axePlantingPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdarg1(),
                i => i.MatchLdfld<Item>("type"),
                i => i.MatchLdcI4(5295),
                i => i.MatchCeq())) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Axe of Regrowth check in Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool!");
            return;
        }
        // emit always true
        ilCursor.EmitPop();
        ilCursor.Emit(OpCodes.Ldc_I4_1);
    }
}