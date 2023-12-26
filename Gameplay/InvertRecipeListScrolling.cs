using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class InvertRecipeListScrolling : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.invertRecipeListScrolling;
    }

    public override void Load() {
        IL_Player.Update += mouseScrollPatch;
    }

    public override void Unload() {
        IL_Player.Update -= mouseScrollPatch;
    }

    // [15836 23 - 15836 74]
    // IL_147b: call         int32 Terraria.Player::GetMouseScrollDelta()
    // IL_1480: stloc.s      mouseScrollDelta
    private static void mouseScrollPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchCall<Player>("GetMouseScrollDelta"),
                i => i.MatchStloc(out var loc))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Player.GetMouseScrollDelta() in Player.Update!");
        }

        ilCursor.Index--;
        // we are between the call and the stloc
        ilCursor.EmitNeg();
    }
}