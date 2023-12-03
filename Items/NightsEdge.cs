using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

// SWORDS TO BE CHANGED:
// all done ;)

public class NightsEdge : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ancientSwords;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 21;
        Item.useTime = 21;
        Item.knockBack = 4.5f;
        Item.width = 40;
        Item.height = 40;
        Item.damage = 42;
        Item.scale = 1.15f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 4);
        Item.DamageType = DamageClass.Melee;
        Item.autoReuse = true;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox) {
        // both for night's edge and true night's edge
        CommonItem.nightsEdgeEffects(player, hitbox);
    }

    public override void AddRecipes() {
        // create all permutations of the recipes
        var recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.LightsBane);
        recipe1.AddIngredient(ItemID.Muramasa);
        recipe1.AddIngredient(ItemID.BladeofGrass);
        recipe1.AddIngredient(ItemID.FieryGreatsword);
        recipe1.AddTile(TileID.DemonAltar);
        recipe1.AddCondition(Condition.CorruptWorld);
        recipe1.ApplyConditionsAsDecraftConditions();
        recipe1.Register();
        var recipe2 = CreateRecipe();
        recipe2.AddIngredient(ItemID.BloodButcherer);
        recipe2.AddIngredient(ItemID.Muramasa);
        recipe2.AddIngredient(ItemID.BladeofGrass);
        recipe2.AddIngredient(ItemID.FieryGreatsword);
        recipe2.AddTile(TileID.DemonAltar);
        recipe2.AddCondition(Condition.CrimsonWorld);
        recipe2.ApplyConditionsAsDecraftConditions();
        recipe2.Register();

        // now with ancient weapons
        int[] muramasa = { ItemID.Muramasa, ModContent.ItemType<Muramasa>() };
        int[] bladeOfGrass = { ItemID.Muramasa, ModContent.ItemType<BladeOfGrass>() };
        int[] fieryGreatsword = { ItemID.Muramasa, ModContent.ItemType<FieryGreatsword>() };
        foreach (var m in muramasa) {
            foreach (var b in bladeOfGrass) {
                foreach (var f in fieryGreatsword) {
                    var recipe3 = CreateRecipe();
                    recipe3.AddIngredient(ItemID.LightsBane);
                    recipe3.AddIngredient(m);
                    recipe3.AddIngredient(b);
                    recipe3.AddIngredient(f);
                    recipe3.AddTile(TileID.DemonAltar);
                    recipe3.AddCondition(Condition.CorruptWorld);
                    recipe3.ApplyConditionsAsDecraftConditions();
                    recipe3.Register();
                    var recipe4 = CreateRecipe();
                    recipe4.AddIngredient(ItemID.BloodButcherer);
                    recipe4.AddIngredient(m);
                    recipe4.AddIngredient(b);
                    recipe4.AddIngredient(f);
                    recipe4.AddTile(TileID.DemonAltar);
                    recipe4.AddCondition(Condition.CrimsonWorld);
                    recipe4.ApplyConditionsAsDecraftConditions();
                    recipe4.Register();

                    // don't add fully vanilla recipe producing vanilla night's edge
                    if (m == ItemID.Muramasa && b == ItemID.BladeofGrass &&
                        f == ItemID.FieryGreatsword) {
                        continue;
                    }
                    // craft the vanilla night's edge as well!
                    var recipe5 = Recipe.Create(ItemID.NightsEdge);
                    recipe5.AddIngredient(ItemID.LightsBane);
                    recipe5.AddIngredient(m);
                    recipe5.AddIngredient(b);
                    recipe5.AddIngredient(f);
                    recipe5.AddTile(TileID.DemonAltar);
                    recipe5.AddCondition(Condition.CorruptWorld);
                    recipe5.ApplyConditionsAsDecraftConditions();
                    recipe5.Register();
                    var recipe6 = Recipe.Create(ItemID.NightsEdge);
                    recipe6.AddIngredient(ItemID.BloodButcherer);
                    recipe6.AddIngredient(m);
                    recipe6.AddIngredient(b);
                    recipe6.AddIngredient(f);
                    recipe6.AddTile(TileID.DemonAltar);
                    recipe6.AddCondition(Condition.CrimsonWorld);
                    recipe6.ApplyConditionsAsDecraftConditions();
                    recipe6.Register();
                }
            }
        }
    }
}