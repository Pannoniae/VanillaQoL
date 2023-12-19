using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

public class RespawningRework : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.respawningRework;
    }

    public override void Load() {
        // sorry calamity, we do the respawning instead.
        // this one makes it configurable, so the calamity logic needs to be bypassed
        // the defaults are close to the calamity values
        if (VanillaQoL.isCalamityLoaded()) {
            CalamityLogic2.load();
        }

        IL_Player.UpdateDead += deathPatch;
    }

    public override void Unload() {
        IL_Player.UpdateDead -= deathPatch;
    }

    // [10192 11 - 10192 79]
    // IL_0410: ldarg.0      // this
    // IL_0411: ldarg.0      // this
    // IL_0412: ldfld        int32 Terraria.Player::respawnTimer
    // IL_0417: ldc.i4.1
    // IL_0418: sub
    // --IL_0419: ldc.i4.0
    // --IL_041a: ldc.i4       3600 // 0x00000e10
    // --IL_041f: call         !!0/*int32*/ Terraria.Utils::Clamp<int32>(!!0/*int32*/, !!0/*int32*/, !!0/*int32*/)
    // IL_0424: stfld        int32 Terraria.Player::respawnTimer
    private void deathPatch(ILContext il) {
        Func<Instruction, bool>[] preds = {
            i => i.MatchLdcI4(0),
            i => i.MatchLdcI4(3600),
            i => i.MatchCall(out _),
            i => i.MatchStfld<Player>("respawnTimer")
        };
        ILCursor ilCursor = new ILCursor(il);
        // IL_0424: stfld        int32 Terraria.Player::respawnTimer
        if (!ilCursor.TryGotoNext(MoveType.Before, preds)) {
            VanillaQoL.instance.Logger.Warn("Couldn't match first respawnTimer set in Player.UpdateDead!");
        }
        ilCursor.RemoveRange(3);

        if (!ilCursor.TryGotoNext(MoveType.Before, preds)) {
            VanillaQoL.instance.Logger.Warn("Couldn't match second respawnTimer set in Player.UpdateDead!");
        }
        ilCursor.RemoveRange(3);
    }
}