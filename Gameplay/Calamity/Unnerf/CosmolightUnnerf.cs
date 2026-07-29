using CalamityMod.Items.Materials;
using CalamityMod.Items.Tools.ClimateChange;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class CosmolightUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.earlyCosmolight;
    }

    public override void AddRecipes() {
        Recipe.Create(ModContent.ItemType<Cosmolight>())
            .AddIngredient(ItemID.FallenStar, 10)
            .AddIngredient(ItemID.SoulofLight, 7)
            .AddIngredient(ItemID.SoulofNight, 7)
            .AddIngredient<EssenceofSunlight>(5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
