using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

public class UIConfig : ModConfig {
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static UIConfig Instance;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("DisplayNPCSellPrices")]

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool enableNPCSellPrices;

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool showCoinIcon;

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    public bool showPriceMultiplier;

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool npcSellPriceShowBackground;

    [BackgroundColor(192, 54, 128, 192)]
    [Range(-100,100), DefaultValue(40), Slider]
    public int npcSellPriceXOffset;

    [BackgroundColor(192, 54, 128, 192)]
    [Range(-100, 100), DefaultValue(0), Slider]
    public int npcSellPriceYOffset;

    [BackgroundColor(192, 54, 128, 192)]
    [Range(0, 20), DefaultValue(5), Slider]
    public int npcSellPriceBackgroundPadding;

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool dynamicBackgroundColor;

    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(typeof(Color), "63,65,151,165")]
    public Color npcSellPriceBackgroundColor;
    
}