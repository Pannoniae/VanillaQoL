using System;
using MonoMod.Cil;
using Terraria;
using Terraria.UI.Gamepad;
using VanillaQoL.IL;
using VanillaQoL.UI;

// these are hooks, don't show "unused member"
// ReSharper disable UnusedMember.Global

namespace VanillaQoL.API;

public class GlobalHooks {
    /// <summary>
    /// Convert speed to km/h.
    /// </summary>
    /// <param name="input">The speed in mph</param>
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
        if (VanillaQoL.instance.hasCensus) {
            numberOfNPCColumns = CensusLogic.numberOfNPCColumns();
            // fix for census + 1 column, it produces really large numbers because the first column isn't full
            if (UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1 <=
                UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) {
                // fuck this we have to estimate ourselves
                var maxY = Main.screenHeight - 80;

                var iconsPerColumn = (maxY - y) / 56; // why 56? idk, vanilla says the size is 56
                numberOfNPCColumns = CensusLogic.numberOfNPCColumns() / iconsPerColumn;
            }
        }


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