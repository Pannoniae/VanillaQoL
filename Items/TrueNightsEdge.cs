using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Items;

public class TrueNightsEdge : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 26;
        Item.useTime = 26;
        Item.shoot = ProjectileID.NightBeam;
        Item.shootSpeed = 10f;
        Item.knockBack = 4.75f;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 105;
        Item.scale = 1.15f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
        Item.DamageType = DamageClass.Melee;
        Item.autoReuse = true;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        // both for night's edge and true night's edge
        CommonItem.nightsEdgeEffects(player, hitbox);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type,
        ref int damage,
        ref float knockback) {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        // use 1.4.1+ stats, damage is 1.5x base
        damage = (int)(damage * 1.5);
    }

    public override void AddRecipes() {
        var recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.NightsEdge);
        recipe1.AddIngredient(ItemID.SoulofFright, 20);
        recipe1.AddIngredient(ItemID.SoulofMight, 20);
        recipe1.AddIngredient(ItemID.SoulofSight, 20);
        recipe1.AddTile(TileID.MythrilAnvil);
        recipe1.ApplyConditionsAsDecraftConditions();
        recipe1.Register();

        /*r recipe2 = CreateRecipe();
        recipe2.AddIngredient<NightsEdge>();
        recipe2.AddIngredient(ItemID.SoulofFright, 20);
        recipe2.AddIngredient(ItemID.SoulofMight, 20);
        recipe2.AddIngredient(ItemID.SoulofSight, 20);
        recipe2.AddTile(TileID.MythrilAnvil);
        recipe2.ApplyConditionsAsDecraftConditions();
        recipe2.Register();

        var recipe3 = Recipe.Create(ItemID.TrueNightsEdge);
        recipe3.AddIngredient<NightsEdge>();
        recipe3.AddIngredient(ItemID.SoulofFright, 20);
        recipe3.AddIngredient(ItemID.SoulofMight, 20);
        recipe3.AddIngredient(ItemID.SoulofSight, 20);
        recipe3.AddTile(TileID.MythrilAnvil);
        recipe3.ApplyConditionsAsDecraftConditions();
        recipe3.Register();*/
    }
}