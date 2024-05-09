using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class MineWallsFromInside : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.mineWallsFromInside;
    }

    public override void Load() {
        IL_Player.CanPlayerSmashWall += mineWallPatch;
    }


    // just return true lol
    public void mineWallPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.Emit(OpCodes.Ldc_I4_1);
        ilCursor.Emit(OpCodes.Ret);
    }
}