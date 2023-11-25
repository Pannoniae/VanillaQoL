using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using VanillaQoL.API;
using VanillaQoL.Config;
using VanillaQoL.UI;

namespace VanillaQoL.Fixes;

public class ILEdits : ModSystem {
    //IL_0684: stloc.1      // start
    //IL_0685: ldsfld       bool Terraria.Main::dayTime
    //IL_068a: brtrue       IL_0975
    //IL_068f: ldc.i4.0
    //IL_0690: stsfld       bool Terraria.Main::eclipse
    /// <summary>
    /// Patch town NPCs to respawn at night.
    /// </summary>
    /// <param name="il"></param>
    private static void townNPCPatch(ILContext il) {
        // so after the !Main.daytime check, we call UpdateTime_SpawnTownNPCs() anyway
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>("eclipse"))) {
            // call         void Terraria.Main::UpdateTime_SpawnTownNPCs()
            ilCursor.Emit<Main>(OpCodes.Call, "UpdateTime_SpawnTownNPCs");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate daytime check (Main.eclipse)");
        }
    }

    //// [21326 9 - 21326 21]
    // IL_008b: ldc.i4.1
    // IL_008c: stloc.1      // flag1

    // // [21327 7 - 21327 28]
    // IL_008d: ldc.r4       1
    // IL_0092: stloc.2      // damageMult

    // // [21328 7 - 21328 238]
    // IL_0093: call         bool Terraria.Main::get_masterMode()
    // IL_0098: brfalse.s    IL_00bb
    /// <summary>
    /// Patch town NPCs to be able to move at daytime.
    /// </summary>
    /// <param name="il"></param>
    private static void townNPCTeleportPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // I am stupid so we patch before the next instruction instead.
        // I can't figure out how to go directly after a jump, sorry
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("get_masterMode"))) {
            // load 1
            ilCursor.EmitLdcI4(1);
            // set local variable index 1 to 1
            ilCursor.EmitStloc1();
        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate master mode check to patch NPCs moving to their houses (Main.masterMode)");
        }
    }

    //// [34500 7 - 34500 31]
    // IL_0057: ldsfld       int32 Terraria.Main::EquipPage
    // IL_005c: ldc.i4.2
    // IL_005d: bne.un.s     IL_0067
    private static void pvpUIPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // inject before the condition
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchLdcI4(2))) {
            // screenWidth - 93 - factor * 4
            // difference is factor * 1.5

            // don't do anything unless EquipPage is npc housing
            // mark location
            var label = ilCursor.DefineLabel();

            // if EquipPage is not 1, skip
            ilCursor.Emit<Main>(OpCodes.Ldsfld, "EquipPage");
            ilCursor.Emit(OpCodes.Ldc_I4_1);
            ilCursor.Emit(OpCodes.Bne_Un_S, label);

            ilCursor.Emit(OpCodes.Ldloc_0);
            ilCursor.Emit(OpCodes.Ldloc_1);
            ilCursor.Emit<ILEdits>(OpCodes.Call, "shiftButtons");
            ilCursor.Emit(OpCodes.Stloc_1);
            ilCursor.MarkLabel(label);
            updateOffsets(ilCursor);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate EquipPage check at DrawPVPIcons (Main.EquipPage)");
        }
    }

    // thank you absoluteAquarian for SerousCommonLib!
    private static void updateOffsets(ILCursor ilCursor) {
        var instrs = ilCursor.Instrs;
        int curOffset = 0;

        static Instruction[] ConvertToInstructions(ILLabel[] labels) {
            Instruction[] ret = new Instruction[labels.Length];

            for (int i = 0; i < labels.Length; i++)
                ret[i] = labels[i].Target!;

            return ret;
        }

        foreach (var ins in instrs) {
            ins.Offset = curOffset;

            if (ins.OpCode != OpCodes.Switch)
                curOffset += ins.GetSize();
            else {
                //'switch' opcodes don't like having the operand as an ILLabel[] when calling GetSize()
                //thus, this is required to even let the mod compile

                Instruction copy = Instruction.Create(ins.OpCode, ConvertToInstructions((ILLabel[])ins.Operand));
                curOffset += copy.GetSize();
            }
        }
    }

    // [38252 13 - 38252 91]
    // IL_0cd2: ldstr        "GameUI.Speed"
    // IL_0cd7: ldloc.s      a
    // IL_0cd9: conv.r8
    // IL_0cda: call         float64 [System.Runtime]System.Math::Round(float64)
    // IL_0cdf: box          [System.Runtime]System.Double
    // IL_0ce4: call         string Terraria.Localization.Language::GetTextValue(string, object)
    // IL_0ce9: stloc.s      text1
    private static void stopwatchMetricPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdstr("GameUI.Speed"),
                _ => true, // some ldloc idc
                _ => true,
                _ => true,
                _ => true,
                i => i.MatchCall("Terraria.Localization.Language", "GetTextValue"),
                i => i.MatchStloc(10))
           ) {
            ilCursor.Emit(OpCodes.Ldloc_S, (byte)49);
            ilCursor.Emit<ILEdits>(OpCodes.Call, "metricStopwatch");
            // text1 = this thing
            ilCursor.Emit(OpCodes.Stloc_S, (byte)10);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate stopwatch info text at DrawInfoAccs (GameUI.Speed)");
        }
    }

    /// <summary>
    /// Convert speed to km/h.
    /// </summary>
    /// <param name="input">The speed in mph</param>
    /// <returns>The speed formatted in km/h</returns>
    public static string metricStopwatch(float mph) {
        var kph = (int)(mph * Constants.mphToKph);
        return VanillaQoLGlobalInfoDisplay.speed.Format(kph);
    }

    public static int shiftButtons(int one, int two) {
        // we need some margin because the rendering is slightly wider than on other pages so
        // if we have like 5 columns, the gems/pvp icon will directly hug the npc boxes...
        const int margin = 3;

        var numberOfNPCColumns = (int)Math.Ceiling((float)UILinkPointNavigator.Shortcuts.NPCS_IconsTotal /
                                                   UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);
        if (VanillaQoL.instance.hasCensus) {
            numberOfNPCColumns = CensusLogic.numberOfNPCColumns();
        }

        var columnsAfter3 = numberOfNPCColumns - 3;
        return two - (one + one / 2 + margin) * columnsAfter3;
    }

    public static void load() {
        if (QoLConfig.Instance.townNPCSpawning) {
            IL_Main.UpdateTime += townNPCPatch;
            IL_NPC.AI_007_TownEntities += townNPCTeleportPatch;
        }

        if (QoLConfig.Instance.fixPvPUI) {
            IL_Main.DrawPVPIcons += pvpUIPatch;
        }

        if (QoLConfig.Instance.metricSystem) {
            IL_Main.DrawInfoAccs += stopwatchMetricPatch;
        }
    }

    public static void unload() {
        IL_Main.UpdateTime -= townNPCPatch;
        IL_NPC.AI_007_TownEntities -= townNPCTeleportPatch;
        IL_Main.DrawPVPIcons -= pvpUIPatch;
        IL_Main.DrawInfoAccs -= stopwatchMetricPatch;
    }
}

[JITWhenModsEnabled("Census")]
public static class CensusLogic {
    private static readonly object theList = null!;

    private static readonly int lengthOfTheList;

    private static readonly PropertyInfo countProperty;

    // time to initialise the hackery
    // also seriously fuck you census for making everything internal so time for hackery
    static CensusLogic() {
        VanillaQoL.instance.Logger.Info(
            "If the Census developers read this, please please don't make everything internal in your mod so I don't have to suffer. <3"
        );
        // too bad tModLoader exposes the class anyway
        var censusMod = ModLoader.GetMod("Census");
        var assembly = censusMod.Code;
        var type = assembly.GetType("Census.CensusSystem");
        var instanceField = type!.GetField("instance", BindingFlags.NonPublic | BindingFlags.Static);
        var instance = instanceField!.GetValue(null);
        var townNPCInfo = type.GetField("realTownNPCsInfos", BindingFlags.NonPublic | BindingFlags.Instance)!;
        theList = townNPCInfo.GetValue(instance)!;
        countProperty = theList.GetType().GetProperty("Count")!;
        lengthOfTheList = (int)countProperty.GetValue(theList)!;
    }

    public static int numberOfNPCColumns() {
        // with census, we need the number of all NPCs divided by the NPC's per column
        var numberOfNPCs = lengthOfTheList;
        return (int)Math.Ceiling((float)numberOfNPCs / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);
    }
}