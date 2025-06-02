using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;


/// <summary>
/// CalamityFables injects the same shit. Disable if they are loaded
/// </summary>
public class InstantPlatformFallthrough : ModSystem {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.instantPlatformFallthrough;
    }

    public override void Load() {
        if (ModLoader.HasMod("CalamityFables")) {
            VanillaQoL.instance.Logger.Warn("Tried to enable instant platform fallthrough but CalamityFables is loaded. This won't work so Vanilla+ QoL is disabling" +
                                            " this feature. Please complain to them or something");
            return;
        }
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
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i=> i.MatchStfld<Player>("slideDir"), i => i.MatchLdcI4(0), i => i.MatchStloc(out int ignorePlats),
                i => i.MatchLdarg0(), i => i.MatchLdfld<Player>("controlDown"), i => i.MatchStloc(out int fallThrough))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Player.slideDir in Player.Update!");
        }
        // we skip the first three instructions
        ilCursor.Index += 3;

        // replace ignorePlats with this:
        // IL_7e94: ldarg.0      // this
        // IL_7e95: ldfld        bool Terraria.Player::controlDown
        ilCursor.Remove();
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdfld<Player>("controlDown");
        // dump IL code
    }
}