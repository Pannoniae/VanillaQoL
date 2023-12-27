using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace VanillaQoL.Gameplay;

public class AxeReplant : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.axeReplanting;
    }

    public override void Load() {
        IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += axePlantingPatch;
    }

    public override void Unload() {
        IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool -= axePlantingPatch;
    }
    // IL_03b7: ldarg.3      // x
    // IL_03b8: ldarg.s      y (4)
    // IL_03ba: call         instance bool Terraria.Player::IsBottomOfTreeTrunkNoRoots(int32, int32)
    // IL_03bf: stloc.s      flag

    // [33080 13 - 33080 36]
    // IL_03c1: ldarg.3      // x
    // IL_03c2: ldarg.s      y
    // IL_03c4: ldc.i4.0
    // IL_03c5: ldc.i4.0
    // IL_03c6: ldc.i4.0
    // IL_03c7: call         void Terraria.WorldGen::KillTile(int32, int32, bool, bool, bool)

    // [33083 13 - 33083 43]
    // IL_03ec: ldarg.1      // sItem
    // IL_03ed: ldfld        int32 Terraria.Item::'type'
    // IL_03f2: ldc.i4       5295 // 0x000014af
    // IL_03f7: ceq
    public void axePlantingPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<Player>("IsBottomOfTreeTrunkNoRoots"),
                i => i.MatchStloc(out var flag))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match bottom of the tree check in Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool!");
            return;
        }

        il.Body.Variables.Add(new(il.Import(typeof(bool))));
        il.Body.Variables.Add(new(il.Import(typeof(int))));
        var idx = il.Body.Variables.Count - 2; // we use the last variable we just added!
        var idx2 = il.Body.Variables.Count - 1; // we use the last variable we just added!
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdarg(4);
        ilCursor.EmitCall<AxeReplant>("isBottomOfGemcornTree");
        ilCursor.EmitStloc(idx);
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdarg(4);
        ilCursor.EmitCall<AxeReplant>("getGemcornTreeType");
        ilCursor.EmitStloc(idx2);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall<WorldGen>("KillTile"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match WorldGen.KillTile in Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool!");
            return;
        }
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdarg(4);
        ilCursor.EmitLdloc(idx);
        ilCursor.EmitLdloc(idx2);
        ilCursor.EmitCall<AxeReplant>("plantGemcornTree");

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdarg1(),
                i => i.MatchLdfld<Item>("type"),
                i => i.MatchLdcI4(5295),
                i => i.MatchCeq())) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match Axe of Regrowth check in Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool!");
            return;
        }

        // emit always true
        ilCursor.EmitPop();
        ilCursor.Emit(OpCodes.Ldc_I4_1);
    }

    public static bool isBottomOfGemcornTree(int x, int y) {
        var tile = Main.tile[x, y];
        if (!tile.HasTile || !TileID.Sets.IsATreeTrunk[tile.TileType]) {
            return false;
        }

        return tile.TileType is >= TileID.TreeTopaz and <= TileID.TreeAmber;
    }

    public static int getGemcornTreeType(int x, int y) {
        var tile = Main.tile[x, y];

        return tile.TileType - TileID.TreeTopaz;
    }

    public static void plantGemcornTree(Player player, int x, int y, bool isGemcornTree, int style) {
        if (!isGemcornTree) {
            return;
        }
        var frameX = style * 54;
        int tileToCreate = 590;
        int previewPlaceStyle = style;
        TileObject objectData;
        if (!TileObject.CanPlace(Player.tileTargetX, Player.tileTargetY, tileToCreate, previewPlaceStyle,
                player.direction, out objectData)) {
            return;
        }

        int num = TileObject.Place(objectData) ? 1 : 0;
        WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
        if (num == 0)
            return;
        TileObjectData.CallPostPlacementPlayerHook(Player.tileTargetX, Player.tileTargetY, tileToCreate,
            previewPlaceStyle, player.direction, objectData.alternate, objectData);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendObjectPlacement(-1, Player.tileTargetX, Player.tileTargetY, objectData.type,
                objectData.style,
                objectData.alternate, objectData.random, player.direction);
        }
    }
}