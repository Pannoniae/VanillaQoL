using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Tiles;

namespace VanillaQoL.Items;

public class AsphaltPlatform : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.asphaltPlatform;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 200;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<AsphaltPlatformTile>());

        Item.SetShopValues(ItemRarityColor.White0, Item.buyPrice(0, 0, 0, 0));
    }

    public override void AddRecipes() {
        var r = Recipe.Create(Type, 2);
        r.AddIngredient(ItemID.AsphaltBlock);
        r.Register();
        
        r = Recipe.Create(ItemID.AsphaltBlock, 1);
        r.AddIngredient(ModContent.ItemType<AsphaltPlatform>(), 2);
        r.Register();
    }
}