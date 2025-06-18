using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class Fountains : ModPlayer {
    public override void PostUpdateMiscEffects() {
        if (QoLConfig.Instance.fountainBiomes) {
            if (Main.SceneMetrics.ActiveFountainColor == 0) {
                Player.ZoneBeach = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 2) {
                Player.ZoneCorrupt = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 3) {
                Player.ZoneJungle = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 4 && Main.hardMode) {
                Player.ZoneHallow = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 5) {
                Player.ZoneSnow = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 6 || Main.SceneMetrics.ActiveFountainColor == 12) {
                Player.ZoneDesert = true;
            }

            if ((Main.SceneMetrics.ActiveFountainColor == 6 || Main.SceneMetrics.ActiveFountainColor == 12) &&
                Player.Center.Y > 3200f) {
                Player.ZoneUndergroundDesert = true;
            }

            if (Main.SceneMetrics.ActiveFountainColor == 10) {
                Player.ZoneCrimson = true;
            }
        }

        if (QoLConfig.Instance.fountainFromInventory) {
            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[0])) {
                Player.ZoneBeach = true;
            }

            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[1])) {
                Player.ZoneCorrupt = true;
            }

            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[2])) {
                Player.ZoneJungle = true;
            }

            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[3]) && Main.hardMode) {
                Player.ZoneHallow = true;
            }

            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[4])) {
                Player.ZoneSnow = true;
            }

            if ((Player.HasItemInAnyInventory(Constants.VanillaFountains[5]) ||
                 Player.HasItemInAnyInventory(Constants.VanillaFountains[6]))) {
                Player.ZoneDesert = true;
            }

            if ((Player.HasItemInAnyInventory(Constants.VanillaFountains[5]) ||
                 Player.HasItemInAnyInventory(Constants.VanillaFountains[6])) && Player.Center.Y > 3200f) {
                Player.ZoneDesert = true;
            }

            if (Player.HasItemInAnyInventory(Constants.VanillaFountains[7])) {
                Player.ZoneCrimson = true;
            }
        }
    }
}