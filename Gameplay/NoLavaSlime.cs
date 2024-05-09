using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class NoLavaSlime : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.noLavaSlime;
    }

    public override void Load() {
        IL_NPC.VanillaHitEffect += lavaSlimePatch;
        //NPC
    }

    private void lavaSlimePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("get_expertMode"), i => i.Match(OpCodes.Brfalse), i => i.Match(OpCodes.Ldarg_0),
                i => i.MatchLdfld<NPC>("type"), i => i.MatchLdcI4(59))) {

            // replace the bne with a br
            ilCursor.Next.OpCode = OpCodes.Br;
            // pop the extra ldc away before jumping
            ilCursor.EmitPop();
            ilCursor.EmitPop();
        }
    }
}