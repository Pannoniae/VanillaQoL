using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using VanillaQoL.API;
using VanillaQoL.Config;
using VanillaQoL.UI;

namespace VanillaQoL.IL;

public class ILEdits : ModSystem {
    // IL_0684: stloc.1      // start
    // IL_0685: ldsfld       bool Terraria.Main::dayTime
    // IL_068a: brtrue       IL_0975
    // IL_068f: ldc.i4.0
    // IL_0690: stsfld       bool Terraria.Main::eclipse
    /// <summary>
    /// Patch town NPCs to respawn at night.
    /// </summary>
    /// <param name="il"></param>
    public static void townNPCPatch(ILContext il) {
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

    // [21326 9 - 21326 21]
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
    public static void townNPCTeleportPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // I am stupid so we patch before the next instruction instead.
        // I can't figure out how to go directly after a jump, sorry
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("get_masterMode"))) {
            // load 1
            ilCursor.Emit(OpCodes.Ldc_I4_1);
            // set local variable index 1 to 1
            ilCursor.EmitStloc1();
        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate master mode check to patch NPCs moving to their houses (Main.masterMode)");
        }
    }

    // [34500 7 - 34500 31]
    // IL_0057: ldsfld       int32 Terraria.Main::EquipPage
    // IL_005c: ldc.i4.2
    // IL_005d: bne.un.s     IL_0067
    public static void PvPUIPatch(ILContext il) {
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
            ilCursor.Emit(OpCodes.Ldloc_2);
            ilCursor.Emit<GlobalHooks>(OpCodes.Call, "shiftButtons");
            ilCursor.Emit(OpCodes.Stloc_1);
            ilCursor.MarkLabel(label);
            updateOffsets(ilCursor);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate EquipPage check at DrawPVPIcons (Main.EquipPage)");
        }
    }

    // also patch out the Main.hidePvPIcons = true as well so it won't hide
    // I am not patching the check out since other mods might want to hide it etc.
    // [34011 15 - 34011 39]
    // IL_025c: ldc.i4.1
    // IL_025d: stsfld       bool Terraria.Main::hidePVPIcons
    public static void PvPNPCDrawPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        // inject before the condition
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1),
                i => i.MatchStsfld<Main>("hidePVPIcons"))) {
            // we don't need these
            ilCursor.RemoveRange(2);
            updateOffsets(ilCursor);
        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate hidePVPIcons assignment at DrawNPCHousesInUI (Main.hidePVPIcons)");
        }
    }

    // thank you absoluteAquarian for SerousCommonLib!
    public static void updateOffsets(ILCursor ilCursor) {
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
    public static void stopwatchMetricPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        int text = 0;
        int a = 0;
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdstr("GameUI.Speed"),
                i => i.MatchLdloc(out a), // some ldloc idc
                _ => true,
                _ => true,
                _ => true,
                i => i.MatchCall("Terraria.Localization.Language", "GetTextValue"),
                i => i.MatchStloc(out text))
           ) {
            ilCursor.Emit(OpCodes.Ldloc_S, (byte)a);
            ilCursor.Emit<GlobalHooks>(OpCodes.Call, "metricStopwatch");
            // text1 = this thing
            ilCursor.Emit(OpCodes.Stloc_S, (byte)text);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate stopwatch info text at DrawInfoAccs (GameUI.Speed)");
        }
    }

    // [473 9 - 473 22]
    // IL_002e: ldloc.3      // num3
    // IL_002f: ldc.i4.0
    public static void disableShimmerPumpingPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc3(),
                i => i.MatchLdcI4(0))) {
            // next instruction is ble <end of loop>, num3 < 0
            // we want to make num3 -1 if our condition is false
            il.Body.Variables.Add(new(il.Import(typeof(bool))));
            var idx = il.Body.Variables.Count - 1; // we use the last variable we just added!
            // we made ourselves some nice code!
            // [202 9 - 202 63]
            // IL_000a: ldloc.0      // liquid
            // IL_000b: ldc.i4.3
            // IL_000c: bne.un.s     IL_0018
            // IL_000e: ldsfld       bool [tModLoader]Terraria.NPC::downedMoonlord
            // IL_0013: ldc.i4.0
            // IL_0014: ceq
            // IL_0016: br.s         IL_0019
            // IL_0018: ldc.i4.0
            // IL_0019: stloc.1      // V_1
            // IL_001a: ldloc.1      // V_1
            // IL_001b: brfalse.s    IL_0022
            // jump if condition isn't met, continue if met
            // ldc.i4.m1
            // stloc.3 // num 3
            // after label to jump to
            var compare = ilCursor.DefineLabel();
            var after = ilCursor.DefineLabel();
            var target = ilCursor.DefineLabel();
            ilCursor.EmitLdloc3();
            ilCursor.Emit(OpCodes.Ldc_I4_3);
            ilCursor.EmitBneUn(compare);
            ilCursor.Emit<NPC>(OpCodes.Ldsfld, "downedMoonlord");
            ilCursor.Emit(OpCodes.Ldc_I4_0);
            ilCursor.EmitCeq();
            ilCursor.EmitBr(after);
            ilCursor.MarkLabel(compare);
            ilCursor.Emit(OpCodes.Ldc_I4_0);
            ilCursor.MarkLabel(after);
            ilCursor.Emit(OpCodes.Stloc_S, (byte)idx);
            ilCursor.Emit(OpCodes.Ldloc_S, (byte)idx);
            ilCursor.EmitBrfalse(target);
            ilCursor.Emit(OpCodes.Ldc_I4_M1);
            ilCursor.EmitStloc3();
            ilCursor.MarkLabel(target);
            updateOffsets(ilCursor);
        }

        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate liquid check at XferWater (Tile.liquid)");
        }
    }


    // todo: make some nice api for this in API

    public static void addPlayerInfo(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchRet())) {
            ilCursor.EmitLdarg0();
            ilCursor.Emit<UIInfo>(OpCodes.Call, "playerInfo");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate return at at IL_UICharacterListItem.ctor");
        }
    }

    public static void addWorldInfo(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchRet())) {
            ilCursor.EmitLdarg0();
            ilCursor.Emit<UIInfo>(OpCodes.Call, "worldInfo");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate return at at IL_UIWorldListItem.ctor");
        }
    }

    public static void load() {
        if (QoLConfig.Instance.townNPCSpawning) {
            IL_Main.UpdateTime += townNPCPatch;
            IL_NPC.AI_007_TownEntities += townNPCTeleportPatch;
        }

        if (QoLConfig.Instance.fixPvPUI) {
            IL_Main.DrawPVPIcons += PvPUIPatch;
            IL_Main.DrawNPCHousesInUI += PvPNPCDrawPatch;
        }

        if (QoLConfig.Instance.metricSystem) {
            IL_Main.DrawInfoAccs += stopwatchMetricPatch;
        }

        if (QoLConfig.Instance.disableShimmerPumping) {
            IL_Wiring.XferWater += disableShimmerPumpingPatch;
        }


        if (QoLConfig.Instance.worldAndPlayerInfo) {
            IL_UICharacterListItem.ctor += addPlayerInfo;
            IL_UIWorldListItem.ctor += addWorldInfo;
        }
    }

    public static void unload() {
        IL_Main.UpdateTime -= townNPCPatch;
        IL_NPC.AI_007_TownEntities -= townNPCTeleportPatch;
        IL_Main.DrawPVPIcons -= PvPUIPatch;
        IL_Main.DrawInfoAccs -= stopwatchMetricPatch;
        IL_Wiring.XferWater -= disableShimmerPumpingPatch;

        IL_UICharacterListItem.ctor -= addPlayerInfo;
        IL_UIWorldListItem.ctor -= addWorldInfo;
    }
}

[JITWhenModsEnabled("Census")]
public static class CensusLogic {
    private static readonly int lengthOfTheList;

    // time to initialise the hackery
    // also seriously fuck you census for making everything internal so time for hackery
    static CensusLogic() {
        VanillaQoL.instance.Logger.Info(
            "If the Census developers read this, please please don't make everything internal in your mod so I don't have to suffer. <3"
        );
        try {
            // too bad tModLoader exposes the class anyway
            var censusMod = ModLoader.GetMod("Census");
            var assembly = censusMod.Code;
            var type = assembly.GetType("Census.CensusSystem");
            var instanceField = type!.GetField("instance", BindingFlags.NonPublic | BindingFlags.Static);
            var instance = instanceField!.GetValue(null);
            var townNPCInfo = type.GetField("realTownNPCsInfos", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var theList = townNPCInfo.GetValue(instance)!;
            var countProperty = theList.GetType().GetProperty("Count")!;
            lengthOfTheList = (int)countProperty.GetValue(theList)!;
        }
        catch (Exception e) {
            VanillaQoL.instance.Logger.Error($"Couldn't load Census integration! Error message: {e}");
            // we just break it silently so it's the original behaviour instead of crashing the game
            lengthOfTheList = 0;
        }
    }

    public static int numberOfNPCColumns() {
        // with census, we need the number of all NPCs divided by the NPC's per column
        var numberOfNPCs = lengthOfTheList;
        return (int)Math.Ceiling((float)numberOfNPCs / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);
    }
}