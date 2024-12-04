using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class CactusHurtsPlayers : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.cactusHurts;
    }

    public override void Load() {
        IL_Collision.CanTileHurt += cactusHurtPatch;
    }

    // IL_0011: ldarg.0      // 'type'
    // IL_0012: ldc.i4.s     80 // 0x50
    // IL_0014: bne.un.s     IL_001f
    // IL_0016: ldsfld       bool Terraria.Main::dontStarveWorld
    // IL_001b: brtrue.s     IL_001f
    // IL_001d: ldc.i4.0
    // IL_001e: ret
    public void cactusHurtPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var label = il.DefineLabel();
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(),
                i => i.MatchLdcI4(80),
                i => i.MatchBneUn(out label))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match ldc.i4.s 80 (TileID.Cactus) in Collision.CanTileHurt!");
            return;
        }

        // we add another condition! If QoLConfig.Instance.cactusHurts is true, we skip too
        // [10 9 - 10 47]
        // ldsfld       class ZenithQoL.Config.QoLConfig ZenithQoL.Config.QoLConfig::Instance
        // callvirt     instance bool ZenithQoL.Config.QoLConfig::get_cactusHurts()
        ilCursor.EmitLdsfld<QoLConfig>("Instance");
        ilCursor.EmitCallvirt<QoLConfig>("get_cactusHurts");
        ilCursor.EmitBrtrue(label);
    }

    public override void PostSetupContent() {
        TileID.Sets.TouchDamageImmediate[TileID.Cactus] = 10;
    }

    public override void Unload() {
        TileID.Sets.TouchDamageImmediate[TileID.Cactus] = 0;
        IL_Collision.CanTileHurt -= cactusHurtPatch;
    }
}