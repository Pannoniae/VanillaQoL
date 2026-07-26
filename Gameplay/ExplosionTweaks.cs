using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class ExplosionTweaks : GlobalWall {
    public override void Load() {
        // If you add another toggle here, put it in vanillaLogic, not in the method.
        if (QoLConfig.Instance.hardModeOresCanExplode || QoLConfig.Instance.meteoriteCanExplode) {
            IL_Projectile.CanExplodeTile += canExplodeTilePatch;
        }
    }


    public override void Unload() {
        IL_Projectile.CanExplodeTile -= canExplodeTilePatch;
    }

    public override bool CanExplode(int i, int j, int type) {
        if (QoLConfig.Instance.wallsDontExplode && Constants.explosionProofWalls.Contains(type)) {
            return false;
        }

        return true;
    }

    // [34057 7 - 34057 36]
    // IL_0047: ldsflda      valuetype Terraria.Tilemap Terraria.Main::tile
    // IL_004c: ldarg.1      // x
    // IL_004d: ldarg.2      // y
    // IL_004e: call         instance valuetype Terraria.Tile Terraria.Tilemap::get_Item(int32, int32)
    // IL_0053: stloc.0      // V_0

    // IL_0054: ldloca.s     V_0
    // IL_0056: call         instance unsigned int16& Terraria.Tile::get_type()
    // IL_005b: ldind.u2
    // IL_005c: stloc.1      // V_1
    // IL_005d: ldloc.1      // V_1
    public void canExplodeTilePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsflda<Main>("tile"),
                i => i.MatchLdarg1(),
                i => i.MatchLdarg2(),
                i => i.MatchCall<Tilemap>("get_Item"),
                i => i.MatchStloc0(),
                i => i.MatchLdloca(0),
                i => i.MatchCall<Tile>("get_type"),
                i => i.MatchLdindU2(),
                i => i.MatchStloc1(),
                i => i.MatchLdloc1())) {
            ilCursor.EmitLdarg1();
            ilCursor.EmitLdarg2();
            ilCursor.EmitCall<ExplosionTweaks>("vanillaLogic");
            ilCursor.EmitRet();
        }
        else {
            VanillaQoL.instance.Logger.Warn("Couldn't match Main.tile in Projectile.CanExplodeTile!");
        }
    }

    public static bool vanillaLogic(int x, int y) {
        switch (Main.tile[x, y].TileType) {
            case TileID.DemonAltar:
            case TileID.Dressers:
            case TileID.LihzahrdBrick:
            case TileID.LihzahrdAltar:
            case TileID.DisplayDoll:
            case TileID.HatRack:
                return false;
            case TileID.Cobalt:
            case TileID.Mythril:
            case TileID.Adamantite:
            case TileID.Chlorophyte:
            case TileID.Palladium:
            case TileID.Orichalcum:
            case TileID.Titanium:
                // the actual hardModeOresCanExplode feature.
                if (!QoLConfig.Instance.hardModeOresCanExplode) {
                    return false;
                }

                return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            case TileID.Meteorite:
                if (QoLConfig.Instance.meteoriteCanExplode) {
                    break;
                }

                if (!Main.hardMode) {
                    return false;
                }

                break;
            case TileID.Hellstone:
                if (!Main.hardMode)
                    return false;
                break;
            case TileID.Spikes:
            case TileID.WoodenSpikes:
                if (Main.getGoodWorld)
                    return false;
                break;
            case TileID.Hellforge:
                if (!Main.hardMode && y >= Main.UnderworldLayer)
                    return false;
                break;
            case TileID.Traps:
                if (!NPC.downedGolemBoss) {
                    switch (Main.tile[x, y].TileFrameY / 18) {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            return false;
                    }
                }
                else
                    break;

                break;
        }

        return true;
    }
}