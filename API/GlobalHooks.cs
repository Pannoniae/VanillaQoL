using System;
using System.Reflection;
using MonoMod.Cil;
using Terraria;
using Terraria.UI.Gamepad;
using ZenithQoL.IL;
using ZenithQoL.UI;

// these are hooks, don't show "unused member"
// ReSharper disable UnusedMember.Global

namespace ZenithQoL.API;

public class GlobalHooks {
    private static readonly FieldInfo mHField = typeof(Main).GetField("mH", BindingFlags.NonPublic | BindingFlags.Static)!;
    
    /// <summary>
    /// Convert speed to km/h.
    /// </summary>
    /// <param name="mph">The speed in mph</param>
    /// <returns>The speed formatted in km/h</returns>
    public static string metricStopwatch(float mph) {
        var kph = (int)(mph * Constants.mphToKph);
        return QoLGlobalInfoDisplay.speed.Format(kph);
    }

    public static int shiftButtons(int one, int two, int y) {
        // we need some margin because the rendering is slightly wider than on other pages so
        // if we have like 5 columns, the gems/pvp icon will directly hug the npc boxes...
        const int margin = 3;

        var numberOfNPCColumns = (int)Math.Ceiling((float)UILinkPointNavigator.Shortcuts.NPCS_IconsTotal /
                                                   UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);

        int iconsPerColumn = 1;
        if (ZenithQoL.instance.hasCensus) {
            numberOfNPCColumns = (int)Math.Ceiling(CensusLogic.numberOfNPCs() / (float)UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);
            // fix for census + 1 column, it produces really large numbers because the first column isn't full
            if (UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn == 1) {
                // fuck this we have to estimate ourselves
                //var maxY = Main.screenHeight - 80;
                int mH = (int)mHField.GetValue(null)!;
                int num2 = 0;
                int num3 = 0;
                int num5 = 0;
                var inventoryScale = 0.85f;
                iconsPerColumn = 1;
                for (int idx = 0; idx < CensusLogic.numberOfNPCs() + 1; ++idx) {
                    int num7 = Main.screenWidth - 64 - 28 + num3;
                    int num8 = (int)(174 + mH + idx * 56 * (double)inventoryScale) + num2;
                    if (num8 > Main.screenHeight - 80) {
                        num3 -= 48;
                        num2 -= num8 - (174 + mH);
                        num7 = Main.screenWidth - 64 - 28 + num3;
                        num8 = (int)(174 + mH + idx * 56 * (double)inventoryScale) + num2;
                        //ZenithQoL.instance.Logger.Warn("num5: " + num5);
                        iconsPerColumn = num5;
                        num5 = 0;
                    }
                    num5++;
                }

                //var iconsPerColumn = (maxY - y) / 56; // why 56? idk, vanilla says the size is 56
                // columns = NPC icons + census icons / icons per column
                numberOfNPCColumns = (int)Math.Ceiling((CensusLogic.numberOfNPCs() + 1) / (float)iconsPerColumn);
            }
        }
        //ZenithQoL.instance.Logger.Warn(numberOfNPCColumns);
        //ZenithQoL.instance.Logger.Warn(CensusLogic.numberOfNPCs());
        //ZenithQoL.instance.Logger.Warn(UILinkPointNavigator.Shortcuts.NPCS_IconsTotal);
        //ZenithQoL.instance.Logger.Warn(UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn);
        //ZenithQoL.instance.Logger.Warn(iconsPerColumn);

        var columnsAfter3 = numberOfNPCColumns - 3;
        return two - (one + one / 2 + margin) * columnsAfter3;

    }

    public static void disableTownSlimeSpawn() {
        NPC.unlockedSlimeBlueSpawn = false;
        NPC.unlockedSlimeGreenSpawn = false;
        NPC.unlockedSlimeOldSpawn = false;
        NPC.unlockedSlimePurpleSpawn = false;
        NPC.unlockedSlimeRainbowSpawn = false;
        NPC.unlockedSlimeRedSpawn = false;
        NPC.unlockedSlimeYellowSpawn = false;
        NPC.unlockedSlimeCopperSpawn = false;
    }

    public static void noop(ILContext il) {
        // the good thing is that we don't do anything
        var ilCursor = new ILCursor(il);
        ilCursor.EmitRet();
    }
}