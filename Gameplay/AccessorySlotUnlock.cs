using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

public class AccessorySlotUnlock : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.accessorySlotUnlock;
    }

    public override void OnModLoad() {
        IL_Player.IsItemSlotUnlockedAndUsable += itemSlotPatch;
        if (VanillaQoL.instance.hasCalamity) {
            CalamityLogic.load();
        }
    }

    public override void Unload() {
        IL_Player.IsItemSlotUnlockedAndUsable -= itemSlotPatch;
    }

    private void itemSlotPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // match expert mode
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("get_expertMode"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match expertMode check in Player.IsItemSlotUnlockedAndUsable!");
        }

        // we also just pop the stack instead of just erasing the entire expert/mastermode call
        // in case some other mod wants to undo/detect this patch
        // let's be nice for once instead of just straight-up deleting instructions
        // brtrue.s -> br.s
        ilCursor.Next!.OpCode = OpCodes.Br_S;
        ilCursor.EmitPop();

        // insert pop before
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("get_masterMode"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match masterMode check in Player.IsItemSlotUnlockedAndUsable!");
        }

        // brtrue.s -> br.s
        ilCursor.Next.OpCode = OpCodes.Br_S;
        ilCursor.EmitPop();
    }
}