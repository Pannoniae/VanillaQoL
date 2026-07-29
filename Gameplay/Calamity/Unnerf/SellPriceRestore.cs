using System;
using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class SellPriceRestore : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.sellPrices;
    }

    public override void OnModLoad() {
        hook(nameof(CalamityGlobalItem.RarityLightRedBuyPrice), Item.buyPrice(0, 12, 0, 0));
        hook(nameof(CalamityGlobalItem.RarityPinkBuyPrice), Item.buyPrice(0, 24, 0, 0));
        hook(nameof(CalamityGlobalItem.RarityLightPurpleBuyPrice), Item.buyPrice(0, 36, 0, 0));
        hook(nameof(CalamityGlobalItem.RarityLimeBuyPrice), Item.buyPrice(0, 48, 0, 0));
    }

    private static void hook(string property, int price) {
        var getter = typeof(CalamityGlobalItem).GetProperty(property)?.GetGetMethod();
        if (getter == null) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find CalamityGlobalItem.{property}!");
            return;
        }

        MonoModHooks.Add(getter, (Func<Func<int>, int>)(_ => price));
    }
}
