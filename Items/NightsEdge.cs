using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

// SWORDS TO BE CHANGED:
// public const short LightsBane = 974;
// public const short BloodButcherer = 975;
// public const short BladeOfGrass = 976;
// public const short Muramasa = 977;
// public const short Volcano = 978;
// public const short Excalibur = 982;
// public const short TrueExcalibur = 983;
// public const short TerraBlade2 = 984;
// public const short TerraBlade2Shot = 985;

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
    }
}