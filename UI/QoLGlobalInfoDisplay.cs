using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.UI;

public class QoLGlobalInfoDisplay : GlobalInfoDisplay, ILocalizedModType {
    public string LocalizationCategory => "UI";

    public static LocalizedText timeText;

    // Look out there, here comes the west wind yes!
    public static LocalizedText westWind;
    public static LocalizedText eastWind;

    public static LocalizedText compassWest;
    public static LocalizedText compassEast;

    public static LocalizedText depth;
    public static LocalizedText speed;

    public override void SetStaticDefaults() {
        timeText = LocalisationUtils.GetLocalization(this, nameof(timeText));
        westWind = LocalisationUtils.GetLocalization(this, nameof(westWind));
        eastWind = LocalisationUtils.GetLocalization(this, nameof(eastWind));
        compassWest = LocalisationUtils.GetLocalization(this, nameof(compassWest));
        compassEast = LocalisationUtils.GetLocalization(this, nameof(compassEast));
        depth = LocalisationUtils.GetLocalization(this, nameof(depth));
        speed = LocalisationUtils.GetLocalization(this, nameof(speed));
    }

    public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue,
        ref string displayName,
        ref Color displayColor, ref Color displayShadowColor) {
        var player = Main.LocalPlayer;
        if (QoLConfig.Instance.twentyFourHourTime && currentDisplay == InfoDisplay.Watches) {
            double time = Main.time;
            if (!Main.dayTime)
                time += 54000.0;
            double hours = time / 86400.0 * 24.0 - 7.5 - 12.0;
            if (hours < 0.0)
                hours += 24.0;
            int exactHours = (int)hours;
            double minutes = (int)((hours - exactHours) * 60.0);
            string minutesStr = minutes.ToString(CultureInfo.InvariantCulture);
            if (minutes < 10.0)
                minutesStr = "0" + minutesStr;
            if (Main.player[Main.myPlayer].accWatch == 1)
                minutesStr = "00";
            else if (Main.player[Main.myPlayer].accWatch == 2)
                minutesStr = minutes >= 30.0 ? "30" : "00";
            displayValue = exactHours + ":" + minutesStr + " ";
        }

        if (QoLConfig.Instance.metricSystem) {
            if (currentDisplay == InfoDisplay.WeatherRadio) {
                if (Main.IsItStorming)
                    displayValue = Language.GetTextValue("GameUI.Storm");
                else if (Main.maxRaining > 0.6)
                    displayValue = Language.GetTextValue("GameUI.HeavyRain");
                else if (Main.maxRaining >= 0.2)
                    displayValue = Language.GetTextValue("GameUI.Rain");
                else if (Main.maxRaining > 0.0)
                    displayValue = Language.GetTextValue("GameUI.LightRain");
                else if (Main.cloudBGActive > 0.0)
                    displayValue = Language.GetTextValue("GameUI.Overcast");
                else if (Main.numClouds > 90)
                    displayValue = Language.GetTextValue("GameUI.MostlyCloudy");
                else if (Main.numClouds > 55)
                    displayValue = Language.GetTextValue("GameUI.Cloudy");
                else if (Main.numClouds <= 15)
                    displayValue = Language.GetTextValue("GameUI.Clear");
                else
                    displayValue = Language.GetTextValue("GameUI.PartlyCloudy");
                var windSpeed = (int)(Main.windSpeedCurrent * 50.0 * Constants.mphToKph);
                if (windSpeed < 0)
                    displayValue += eastWind.Format(Math.Abs(windSpeed));
                else if (windSpeed > 0)
                    displayValue += westWind.Format(windSpeed);
                if (Sandstorm.Happening) {
                    if (Main.GlobalTimeWrappedHourly % 10.0 >= 5.0)
                        displayValue = Language.GetTextValue("GameUI.Sandstorm");
                    displayValue += " +";
                }
            }

            if (currentDisplay == InfoDisplay.Stopwatch) {
                // we handle this in IL in ILEdits, I'm not copying vanilla logic, this would break too much
            }

            if (currentDisplay == InfoDisplay.Compass) {
                // 1 tile = 2ft, in old console / here, 1 tile = 1/2m = 50cm
                var distance = (int)((Main.player[Main.myPlayer].position.X +
                                      (double)Main.player[Main.myPlayer].width / 2) * 2.0 / 16.0 -
                                     Main.maxTilesX);
                distance = (int)(distance * Constants.feetToMetre);
                displayValue = distance switch {
                    > 0 => compassEast.Format(distance),
                    0 => Language.GetTextValue("GameUI.CompassCenter"),
                    < 0 => compassWest.Format(-distance)
                };
            }

            if (currentDisplay == InfoDisplay.DepthMeter) {
                int depthNumber = (int)((Main.player[Main.myPlayer].position.Y +
                                   Main.player[Main.myPlayer].height) * 2.0 / 16.0 - Main.worldSurface * 2.0);
                float worldWidthFactor = Main.maxTilesX / 4200f;
                float wwFactorSquared = worldWidthFactor * worldWidthFactor;
                float depthNumberScaled =
                    (float)((Main.player[Main.myPlayer].Center.Y / 16.0 - (65.0 + 10.0 * wwFactorSquared)) /
                            (Main.worldSurface / 5.0));
                string layer;
                if (Main.player[Main.myPlayer].position.Y > (Main.maxTilesY - 204) * 16)
                    layer = Language.GetTextValue("GameUI.LayerUnderworld");
                else if (Main.player[Main.myPlayer].position.Y >
                         Main.rockLayer * 16.0 + 600 + 16.0)
                    layer = Language.GetTextValue("GameUI.LayerCaverns");
                else if (depthNumber > 0)
                    layer = Language.GetTextValue("GameUI.LayerUnderground");
                else if (depthNumberScaled < 1.0)
                    layer = Language.GetTextValue("GameUI.LayerSpace");
                else
                    layer = Language.GetTextValue("GameUI.LayerSurface");
                int absoluteDepth = Math.Abs(depthNumber);
                displayValue = (absoluteDepth != 0
                    ? depth.Format(absoluteDepth)
                    : Language.GetTextValue("GameUI.DepthLevel")) + " " + layer;
            }
        }
    }
}