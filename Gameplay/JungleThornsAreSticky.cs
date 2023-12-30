using System.Diagnostics.CodeAnalysis;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

[SuppressMessage("ReSharper", "NotAccessedOutParameterVariable")]
public class JungleThornsAreSticky : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.jungleThornsAreSticky;
    }

    public override void Load() {
        IL_Player.StickyMovement += stickyMovementPatch;
        IL_Collision.StickyTiles += stickyTilesPatch;
    }


    public void stickyMovementPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // IL_010b: ldloc.s      num
        // IL_010d: ldc.i4.s     51 // 0x33
        // IL_010f: bne.un       IL_01b5
        // etc.
        var num = 0;
        ILLabel label;
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(out num), i => i.MatchLdcI4(51),
                i => i.MatchBneUn(out label!))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match cobweb type checks in Player.StickyMovement!");
            return;
        }

        ilCursor.Index--;
        // before the bne.un
        ilCursor.EmitCeq();
        ilCursor.EmitNot(); // 0xFFFFFFFF
        // jump if not equal or not sticky
        ilCursor.EmitLdloc(num);
        ilCursor.EmitCall<JungleThornsAreSticky>("isNotSticky");
        ilCursor.EmitAnd();
        // if != 51 or isSticky, skip
        ilCursor.Next!.OpCode = OpCodes.Brtrue;

        MonoModHooks.DumpIL(VanillaQoL.instance, il);
    }

    public void stickyTilesPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // [2969 17 - 2969 46]
        // IL_00ec: ldloca.s     tile
        // IL_00ee: call         instance unsigned int16& Terraria.Tile::get_type()
        // IL_00f3: ldind.u2
        // IL_00f4: ldc.i4.s     51 // 0x33
        // IL_00f6: bne.un       IL_0210
        var tile = 0;
        var ctr = 0;
        ILLabel label;
        while (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdloca(out tile),
                   i => i.MatchCall<Tile>("get_type"),
                   i => i.MatchLdindU2(),
                   i => i.MatchLdcI4(51), i => i.MatchBneUn(out label!))) {
            ctr++;
            ilCursor.Index--;
            // before the bne.un
            ilCursor.EmitCeq();
            ilCursor.EmitNot(); // 0xFFFFFFFF
            // jump if not equal or not sticky
            // IL_00ec: ldloca.s     tile
            // IL_00ee: call         instance unsigned int16& Terraria.Tile::get_type()
            // IL_00f3: ldind.u2
            ilCursor.EmitLdloca(tile);
            ilCursor.EmitCall<Tile>("get_type");
            ilCursor.EmitLdindU2();
            ilCursor.EmitCall<JungleThornsAreSticky>("isNotSticky");
            ilCursor.EmitAnd();
            // if != 51 or isSticky, skip
            ilCursor.Next!.OpCode = OpCodes.Brtrue;
        }

        if (ctr == 0) {
            VanillaQoL.instance.Logger.Warn("Couldn't match cobweb type checks in Collision.StickyTiles!");
        }
        VanillaQoL.instance.Logger.Info("Patched Collision.StickyTiles {ctr} times!");

        MonoModHooks.DumpIL(VanillaQoL.instance, il);
    }

    public static bool isNotSticky(int type) {
        return !Constants.isSticky(type);
    }

    public override void Unload() {
        IL_Player.StickyMovement -= stickyMovementPatch;
        IL_Collision.StickyTiles -= stickyTilesPatch;
    }
}