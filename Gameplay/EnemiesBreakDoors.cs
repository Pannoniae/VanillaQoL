using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class EnemiesBreakDoors : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.enemiesBreakDoors;
    }

    public override void Load() {
        IL_NPC.AI_003_Fighters += enemiesBreakDoorsPatch;
    }

    // IL_cf6c: ldfld        int32 Terraria.NPC::'type'
    // IL_cf71: ldc.i4.s     26 // 0x1a
    // IL_cf73: bne.un.s     IL_cfbe
    private void enemiesBreakDoorsPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<NPC>("type"),
                i => i.MatchLdcI4(26),
                i => i.MatchBneUn(out var label))) {
            // go to before the load
            ilCursor.Index++;
            // replace it with type != 0 (so always true lol)
            ilCursor.Next!.Operand = (sbyte)0;
        }
    }
}