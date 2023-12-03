using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class TrueExcalibur : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 16;
        Item.useTime = 16;
        Item.shoot = ProjectileID.LightBeam;
        Item.shootSpeed = 11f;
        Item.knockBack = 4.5f;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 70;
        Item.scale = 1.05f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
        Item.DamageType = DamageClass.Melee;
        Item.autoReuse = true;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        CommonItem.excaliburEffects(player, hitbox);
    }

    public override void AddRecipes() {
        var recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.Excalibur);
        recipe1.AddIngredient(ItemID.ChlorophyteBar, 20);
        recipe1.AddTile(TileID.MythrilAnvil);
        recipe1.Register();

        var recipe2 = CreateRecipe();
        recipe2.AddIngredient(ModContent.ItemType<Excalibur>());
        recipe2.AddIngredient(ItemID.ChlorophyteBar, 20);
        recipe2.AddTile(TileID.MythrilAnvil);
        recipe2.Register();

        var recipe3 = Recipe.Create(ItemID.TrueExcalibur);
        recipe3.AddIngredient(ModContent.ItemType<Excalibur>());
        recipe3.AddIngredient(ItemID.ChlorophyteBar, 20);
        recipe3.AddTile(TileID.MythrilAnvil);
        recipe3.Register();
    }
}