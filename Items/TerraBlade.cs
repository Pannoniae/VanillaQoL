using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class TerraBlade : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.damage = 115;
        Item.useAnimation = 14;
        Item.useTime = 14;
        Item.width = 30;
        Item.height = 30;
        Item.shoot = ProjectileID.TerraBeam;
        Item.scale = 1.1f;
        Item.shootSpeed = 12f;
        Item.knockBack = 6.5f;
        Item.DamageType = DamageClass.Melee;
        Item.value = Item.sellPrice(gold: 20);
        Item.autoReuse = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type,
        ref int damage,
        ref float knockback) {
        damage = (int)(damage * 1.5);
    }

    public override void AddRecipes() {

        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.TrueNightsEdge);
        recipe.AddIngredient(ItemID.TrueExcalibur);
        recipe.AddIngredient(ItemID.BrokenHeroSword);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();

        /*int[] trueNightsEdge = { ItemID.TrueNightsEdge, ModContent.ItemType<TrueNightsEdge>() };
        int[] trueExcalibur = { ItemID.TrueExcalibur, ModContent.ItemType<TrueExcalibur>() };

        foreach (var n in trueNightsEdge) {
            foreach (var e in trueExcalibur) {
                var recipe1 = CreateRecipe();
                recipe1.AddIngredient(n);
                recipe1.AddIngredient(e);
                recipe1.AddIngredient(ItemID.BrokenHeroSword);
                recipe1.AddTile(TileID.MythrilAnvil);
                recipe1.Register();

                // skip if fully vanilla
                if (n == ItemID.TrueNightsEdge && e == ItemID.TrueExcalibur) {
                    continue;
                }

                var recipe2 = Recipe.Create(ItemID.TerraBlade);
                recipe2.AddIngredient(n);
                recipe2.AddIngredient(e);
                recipe2.AddIngredient(ItemID.BrokenHeroSword);
                recipe2.AddTile(TileID.MythrilAnvil);
                recipe2.Register();
            }
        }*/
    }
}