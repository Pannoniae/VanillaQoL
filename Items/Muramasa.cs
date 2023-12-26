using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class Muramasa : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        // 1.4.1+ values
        Item.autoReuse = true;
        Item.useTurn = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 18;
        Item.useTime = 18;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 26;
        Item.scale = 1.1f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1, silver: 75);
        Item.knockBack = 3f;
        Item.DamageType = DamageClass.Melee;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        int index = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.DungeonWater,
            player.velocity.X * 0.2f + player.direction * 3, player.velocity.Y * 0.2f, 100, Scale: 0.9f);
        Main.dust[index].noGravity = true;
        Main.dust[index].velocity *= 0.1f;
    }

    public override void AddRecipes() {
        // this one is crafted from the regular one
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Muramasa);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
        // and convert back
        var recipe2 = Recipe.Create(ItemID.Muramasa);
        recipe2.AddIngredient(Item.type);
        recipe2.AddTile(TileID.Anvils);
        recipe2.Register();
    }
}