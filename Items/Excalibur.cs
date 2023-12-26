using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class Excalibur : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.knockBack = 4.5f;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 66;
        Item.scale = 1.15f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 23);
        Item.DamageType = DamageClass.Melee;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        CommonItem.excaliburEffects(player, hitbox);
    }

    public override void AddRecipes() {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.HallowedBar, 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}