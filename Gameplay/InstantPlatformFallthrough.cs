using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class InstantPlatformFallthrough : ModSystem {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.instantPlatformFallthrough;
    }

    public override void Load() {
        IL_Player.Update += instantPlatformFallthroughPatch;
    }

    // [23008 3 - 23008 16]
    // IL_7e8a: ldarg.0      // this
    // IL_7e8b: ldc.i4.0
    // IL_7e8c: stfld        int32 Terraria.Player::slideDir
    // IL_7e91: ldc.i4.0
    // IL_7e92: stloc.s      ignorePlats

    // // [23010 3 - 23010 34]
    // IL_7e94: ldarg.0      // this
    // IL_7e95: ldfld        bool Terraria.Player::controlDown
    // IL_7e9a: stloc.s      fallThrough
    private void instantPlatformFallthroughPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(0), i => i.MatchStloc(out int ignorePlats),
                i => i.MatchLdarg0(), i => i.MatchLdfld<Player>("controlDown"), i => i.MatchStloc(out int fallThrough))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Player.slideDir in Player.Update!");
        }
        // replace ignorePlats with 1!
        ilCursor.Next!.OpCode = OpCodes.Ldc_I4_1;
    }
}