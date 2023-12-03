using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class MiscRecipes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void AddRecipes() {
        // fixup zenith recipe with new terra blade
        var recipe = Recipe.Create(ItemID.Zenith);
        recipe.AddIngredient(ModContent.ItemType<TerraBlade>());
        recipe.AddIngredient(ItemID.Meowmere);
        recipe.AddIngredient(ItemID.StarWrath);
        recipe.AddIngredient(ItemID.InfluxWaver);
        recipe.AddIngredient(ItemID.TheHorsemansBlade);
        recipe.AddIngredient(ItemID.Seedler);
        recipe.AddIngredient(ItemID.Starfury);
        recipe.AddIngredient(ItemID.BeeKeeper);
        recipe.AddIngredient(ItemID.EnchantedSword);
        recipe.AddIngredient(ItemID.CopperShortsword);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}