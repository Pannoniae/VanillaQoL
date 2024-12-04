using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ZenithQoL.UI;

public class LuckInfoDisplay : InfoDisplay {

    public static LocalizedText luckText = null!;

    public override void SetStaticDefaults() => luckText = this.GetLocalization(nameof(luckText));

    public override bool Active() => QoLConfig.Instance.showLuck && MetalDetector.Active();

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        return luckText.Format(Main.LocalPlayer.luck.ToString("0.##"));
    }
}