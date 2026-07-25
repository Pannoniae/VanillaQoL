using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

// Expert quietly halves your natural regen unless you're Well Fed.
public class FullExpertRegen : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active && CalamityQoLConfig.Instance.fullExpertRegen;
    }

    public override void Load() {
        IL_Player.UpdateLifeRegen += fullExpertRegenPatch;
    }

    // // [18013 3 - 18013 34]
    // IL_04b1: call         bool Terraria.Main::get_expertMode()
    // IL_04b6: brfalse.s    IL_04d0
    // IL_04b8: ldarg.0      // this
    // IL_04b9: ldfld        bool Terraria.Player::wellFed
    // IL_04be: brtrue.s     IL_04d0
    private static void fullExpertRegenPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // anchor on the expert check first
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Main>("get_expertMode")) ||
            !ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("wellFed"))) {
            VanillaQoL.instance.Logger.Warn("Failed to locate the expert Well Fed regen check (Player.wellFed)");
            return;
        }

        // tell it you've eaten
        ilCursor.Emit(OpCodes.Ldc_I4_1);
        ilCursor.Emit(OpCodes.Or);
    }
}
