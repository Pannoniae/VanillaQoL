using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class HolidayCheer : ModSystem {
    public override void Load() {
        IL_Main.checkXMas += xMasPatch;
        IL_Main.checkHalloween += halloweenPatch;
    }

    public override void Unload() {
        IL_Main.checkXMas -= xMasPatch;
        IL_Main.checkHalloween -= halloweenPatch;
    }

    public void xMasPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var cont = ilCursor.DefineLabel();
        ilCursor.Emit<HolidayCheer>(OpCodes.Call, "isXMas");
        ilCursor.EmitBrfalse(cont);
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
        ilCursor.EmitRet();
        ilCursor.MarkLabel(cont);
    }

    public static bool isHalloween() {
        return QoLConfig.Instance.halloweenAllYear;
    }
}