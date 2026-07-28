using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Mechanics.SmartCursor;

/**
 * Vanilla's SmartCursorHelper duplicates every placement rule by hand and the copypaste sometimes forgets an entry lol. We patch it back to sanity
 */
public class SmartCursor : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        var config = QoLConfig.Instance;
        return config.torchesOnPlatforms || config.smartPlatformPlacement || config.wallsRespectPlatforms ||
               config.blocksReplaceGrass || config.smartCursorRange;
    }

    public override void Load() {
        var config = QoLConfig.Instance;
        if (config.torchesOnPlatforms) {
            IL_SmartCursorHelper.Step_Torch += torchPlatformPatch;
        }

        if (config.smartPlatformPlacement) {
            IL_SmartCursorHelper.Step_Platforms += platformPatch;
        }

        if (config.wallsRespectPlatforms) {
            IL_DelegateMethods.NotDoorStand += wallPlatformPatch;
        }

        if (config.blocksReplaceGrass) {
            IL_SmartCursorHelper.Step_BlocksLines += grassBlocksPatch;
        }

        if (config.smartCursorRange) {
            IL_SmartCursorHelper.SmartCursorLookup += rangePatch;
        }
    }

    // 1.4.4 added torches to on platforms. The game keeps four hand-written copies of the
    // "can a torch stand here" rule and Re-Logic updated three of them - PlaceTile, CheckTorch and
    // the click gate all accept platforms, but Step_Torch still checks tileNoAttach without the
    // platform exemption. So smart cursor never suggests torches on a floating platform. Fix copy 4...
    //
    // IL_0372: ldsfld       bool[] Terraria.Main::tileNoAttach
    //          ldloca.s     tile4 / call get_type / ldind.u2
    //          ldelem.u1    <- this load is the one that lies about platforms
    // IL_0392: ldsfld       bool[] Terraria.ID.TileID/Sets::Platforms   <- unique in the method
    private static void torchPlatformPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        // you can't place torches on the *side* of platforms so the other two platform checks in the method are correct, we only need to patch the tileNoAttach check.
        // We'd just make torches which immediately pop
        // sidenote: it has never occurred to me so far to actually match on the *names* of fields!
        // Yes it's a bit fragile but 1. the field names are mostly stable, 2. breakage is OBVIOUS and LOUD 3. pattern-matching the IL which we usually do is actually more fragile!
        if (!ilCursor.TryGotoNext(i => i.MatchLdsfld(out var field) && field.Name == nameof(TileID.Sets.Platforms))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find TileID.Sets.Platforms in Step_Torch!");
            return;
        }

        if (!ilCursor.TryGotoPrev(i => i.MatchLdsfld<Main>(nameof(Main.tileNoAttach)))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find Main.tileNoAttach in Step_Torch!");
            return;
        }

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdelemU1())) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the tileNoAttach element load in Step_Torch!");
            return;
        }

        ilCursor.Remove();
        ilCursor.EmitCall<SmartCursor>(nameof(noAttachTorchBase));
    }

    // mirrors CheckTorch: tileSolid && (!tileNoAttach || Platforms)
    public static bool noAttachTorchBase(bool[] noAttach, int type) {
        return noAttach[type] && !TileID.Sets.Platforms[type];
    }

    private static void platformPatch(ILContext il) {
        grassPatch(il);
        selectionPatch(il);
    }

    // todo this is a bit hacky but seems to work?
    private static void grassPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var count = 0;
        while (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchCall<Tile>("active"))) {
            ilCursor.Remove();
            ilCursor.EmitCall<SmartCursor>(nameof(activeAndBlocking));
            count++;
        }

        if (count == 0) {
            VanillaQoL.instance.Logger.Warn("Couldn't find any Tile.active() calls in Step_Platforms!");
        }
    }

    // same signature as the instance call: `ldloca` Tile in, bool out
    public static bool activeAndBlocking(ref Tile tile) {
        return tile.HasTile && !Main.tileCut[tile.TileType];
    }

    // vanilla's selection is "nearest to mouse", which is how you get stairs when you wanted a
    // bridge. we skip the whole block and do it ourselves
    //
    // IL_0322: ldsfld       _targets
    // IL_0327: callvirt     get_Count     <- from here
    //   ...
    // IL_0416: ldsfld       _targets
    // IL_041b: callvirt     Clear         <- to here
    // IL_0420: ret
    private static void selectionPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var targetsField =
            typeof(SmartCursorHelper).GetField("_targets", BindingFlags.NonPublic | BindingFlags.Static)!;
        var infoType = typeof(SmartCursorHelper).GetNestedType("SmartCursorUsageInfo", BindingFlags.NonPublic)!;

        var shim = typeof(PlatformTargeting).GetMethod(nameof(PlatformTargeting.selectVanilla))!;
        if (shim.GetParameters()[0].ParameterType != targetsField.FieldType) {
            VanillaQoL.instance.Logger.Warn(
                "selectVanilla's first parameter doesn't match the type?");
            return;
        }

        if (!ilCursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdsfld(out var field) && field.Name == "_targets",
                i => i.MatchCallvirt(out var method) && method.Name == "get_Count")) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the selection block in Step_Platforms!");
            return;
        }

        var selectionStart = ilCursor.Index;

        if (!ilCursor.GotoFinalRet(MoveType.Before)) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the final ret in Step_Platforms!");
            return;
        }

        if (!ilCursor.TryGotoPrev(MoveType.Before, i => i.MatchLdsfld(out var field) && field.Name == "_targets")) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the trailing _targets.Clear() in Step_Platforms!");
            return;
        }

        var end = ilCursor.MarkLabel();

        ilCursor.Index = selectionStart;
        ilCursor.Emit(OpCodes.Ldsfld, targetsField);
        ilCursor.EmitLdarg0();
        ilCursor.Emit(OpCodes.Ldfld, infoType.GetField("mouse")!);
        ilCursor.EmitLdarg0();
        ilCursor.Emit(OpCodes.Ldfld, infoType.GetField("reachableStartX")!);
        ilCursor.EmitLdarg0();
        ilCursor.Emit(OpCodes.Ldfld, infoType.GetField("reachableStartY")!);
        ilCursor.EmitLdarg0();
        ilCursor.Emit(OpCodes.Ldfld, infoType.GetField("reachableEndX")!);
        ilCursor.EmitLdarg0();
        ilCursor.Emit(OpCodes.Ldfld, infoType.GetField("reachableEndY")!);
        ilCursor.EmitLdarg1();
        ilCursor.EmitLdarg2();
        ilCursor.EmitCall<PlatformTargeting>(nameof(PlatformTargeting.selectVanilla));
        ilCursor.Emit(OpCodes.Br, end);
    }

    // NotDoorStand is the line-of-sight predicate Step_Walls feeds to Collision.CanHitWithCheck -
    // return false and the wall flood stops there. vanilla only stops at closed doors, so wall
    // placement merrily continues into the room under your platform floor. flat platforms now
    // count as floors; hammer them into stairs if you want the walls to flow through.
    private static void wallPlatformPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var label = ilCursor.DefineLabel();
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.EmitCall<SmartCursor>(nameof(platformBlocksSight));
        ilCursor.Emit(OpCodes.Brfalse, label);
        ilCursor.EmitLdcI4(0);
        ilCursor.EmitRet();
        ilCursor.MarkLabel(label);
    }

    public static bool platformBlocksSight(int x, int y) {
        var tile = Main.tile[x, y];
        return tile.HasTile && TileID.Sets.Platforms[tile.TileType] && tile.Slope == SlopeType.Solid;
    }


    // IL_0331: ldc.i4.0     <- ignoreTiles: false, the bug
    // IL_0332: call         bool Terraria.Collision::EmptyTile(int32, int32, bool)
    private static void grassBlocksPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(0),
                i => i.MatchCall<Collision>(nameof(Collision.EmptyTile)))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the EmptyTile call in Step_BlocksLines!");
            return;
        }

        ilCursor.Remove();
        ilCursor.EmitLdcI4(1);
    }

    // IL_00d5: ldfld        class Terraria.Item SmartCursorUsageInfo::item
    // IL_00da: ldfld        int32 Terraria.Item::tileBoost
    // IL_00df: stloc.s      5
    private static void rangePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Item>(nameof(Item.tileBoost)))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the ldfld tileBoost in SmartCursorLookup!");
            return;
        }

        ilCursor.EmitLdarg0();
        ilCursor.EmitLdfld<Player>(nameof(Player.blockRange));
        ilCursor.EmitAdd();
    }
}
