using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class HolidayCheer : ModSystem {
    public override void Load() {
        IL_Main.checkXMas += xMasPatch;
        IL_Main.checkHalloween += halloweenPatch;
        IL_Item.NewItem_Inner += allHeartsPatch;
        if (QoLConfig.Instance.stopSantaFromExploding) {
            IL_NPC.AI_007_TownEntities += stopSantaExplosionPatch;
        }
    }

    public override void Unload() {
        IL_Main.checkXMas -= xMasPatch;
        IL_Main.checkHalloween -= halloweenPatch;
        IL_Item.NewItem_Inner -= allHeartsPatch;
        IL_NPC.AI_007_TownEntities -= stopSantaExplosionPatch;
    }

    public void xMasPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var cont = ilCursor.DefineLabel();
        ilCursor.Emit<HolidayCheer>(OpCodes.Call, "isXMas");
        ilCursor.EmitBrfalse(cont);
        ilCursor.Emit(OpCodes.Ldc_I4_1);
        ilCursor.EmitStsfld<Main>("xMas");
        ilCursor.EmitRet();
        ilCursor.MarkLabel(cont);
    }

    public static bool isXMas() {
        return QoLConfig.Instance.christmasAllYear;
    }

    public void halloweenPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var cont = ilCursor.DefineLabel();
        ilCursor.Emit<HolidayCheer>(OpCodes.Call, "isHalloween");
        ilCursor.EmitBrfalse(cont);
        ilCursor.Emit(OpCodes.Ldc_I4_1);
        ilCursor.EmitStsfld<Main>("halloween");
        ilCursor.EmitRet();
        ilCursor.MarkLabel(cont);
    }

    public static bool isHalloween() {
        return QoLConfig.Instance.halloweenAllYear;
    }

    public void allHeartsPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // [50056 7 - 50056 26]
        // IL_0087: ldsfld       bool Terraria.Main::halloween
        if (!ilCursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<Main>("halloween"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Main.halloween in Item.NewItem_Inner!");
        }
        // we do the check ourselves
        ilCursor.Emit(OpCodes.Ldarg_S, (byte)6);
        ilCursor.EmitCall<HolidayCheer>("switchItem");
        ilCursor.Emit(OpCodes.Starg_S, (byte)6);
        // copy cursor
        var ilCursor2 = new ILCursor(ilCursor);
        ILLabel label = null!;
        if (!ilCursor2.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("xMas"),
                 i => i.MatchBrfalse(out label!))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Main.halloween in Item.NewItem_Inner!");
            return;
        }
        //// [50063 7 - 50063 21]
        // IL_00ab: ldsfld       bool Terraria.Main::xMas
        // IL_00b0: brfalse.s    IL_00cf

        // skip the section
        ilCursor.EmitBr(label);
    }

    public static int switchItem(int type) {
        var list1 = new List<int> {
            58
        };
        var list2 = new List<int> {
            184
        };
        if (Main.halloween) {
            list1.Add(1734);
            list2.Add(1735);
        }

        if (Main.xMas) {
            list1.Add(1867);
            list2.Add(1868);
        }

        if (type == 58) {
            type = Main.rand.NextFromCollection(list1);
        }

        if (type == 184) {
            type = Main.rand.NextFromCollection(list2);
        }

        return type;
    }

    // IL_02e2: ldsfld       bool Terraria.Main::xMas
    // IL_02e7: brtrue.s     IL_0325
    public void stopSantaExplosionPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ILLabel label;
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("xMas"),
                i => i.MatchBrtrue(out label!))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match Main.xMas in PC.AI_007_TownEntities!");
        }
        // before brtrue.s
        ilCursor.Index--;
        // pop xmas from the stack
        ilCursor.EmitPop();
        ilCursor.Next!.OpCode = OpCodes.Br_S;
    }
}