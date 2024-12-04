using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Items;

public class BladeOfGrass : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.knockBack = 3f;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 28;
        Item.scale = 1.4f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Orange;
        Item.value = 27000;
        Item.DamageType = DamageClass.Melee;
    }

    public override void AddRecipes() {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Stinger, 12);
        recipe.AddIngredient(ItemID.JungleSpores, 15);
        recipe.AddIngredient(ItemID.Vine, 3);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        int index = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width,
            hitbox.Height, DustID.t_Cactus, player.velocity.X * 0.2f + player.direction * 3, player.velocity.Y * 0.2f,
            Scale: 1.2f);
        Main.dust[index].noGravity = true;
    }
}