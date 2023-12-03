using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class FieryGreatsword : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        // 1.4.1+ stats
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.knockBack = 6.5f;
        Item.width = 24;
        Item.height = 28;
        Item.damage = 40;
        Item.scale = 1.3f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 27);
        Item.DamageType = DamageClass.Melee;
    }

    public override void AddRecipes() {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.HellstoneBar, 20);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        for (int index3 = 0; index3 < 2; ++index3) {
            int index4 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch,
                player.velocity.X * 0.2f + player.direction * 3, player.velocity.Y * 0.2f, 100, Scale: 2.5f);
            Main.dust[index4].noGravity = true;
            Main.dust[index4].velocity.X *= 2f;
            Main.dust[index4].velocity.Y *= 2f;
        }
    }
}