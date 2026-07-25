using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class RecipeGroups : ModSystem {
    public static RecipeGroup anyNightsEdge = null!;
    public static RecipeGroup anyTrueNightsEdge = null!;
    public static RecipeGroup anyMuramasa = null!;
    public static RecipeGroup anyTerraBlade = null!;
    public static RecipeGroup anyExcalibur = null!;
    public static RecipeGroup anyTrueExcalibur = null!;
    public static RecipeGroup anyFieryGreatsword = null!;
    public static RecipeGroup anyBladeOfGrass = null!;


    // we can't use the Calamity ones, because this might run w/o Calamity
    public static int anyCopperBar;
    public static int anySilverBar;
    public static int anyGoldBar;
    public static int anyCobaltBar;
    public static int anyStoneBlock;
    public static int anySnowBlock;
    public static int anyIceBlock;
    public static int anyEvilPowder;

    public override void AddRecipeGroups() {
        anyCopperBar = group("AnyCopperBar", ItemID.CopperBar, ItemID.CopperBar, ItemID.TinBar);
        anySilverBar = group("AnySilverBar", ItemID.SilverBar, ItemID.SilverBar, ItemID.TungstenBar);
        anyGoldBar = group("AnyGoldBar", ItemID.GoldBar, ItemID.GoldBar, ItemID.PlatinumBar);
        anyCobaltBar = group("AnyCobaltBar", ItemID.CobaltBar, ItemID.CobaltBar, ItemID.PalladiumBar);
        anyStoneBlock = group("AnyStoneBlock", ItemID.StoneBlock, ItemID.StoneBlock, ItemID.EbonstoneBlock,
            ItemID.CrimstoneBlock, ItemID.PearlstoneBlock);
        anySnowBlock = group("AnySnowBlock", ItemID.SnowBlock, ItemID.SnowBlock);
        anyIceBlock = group("AnyIceBlock", ItemID.IceBlock, ItemID.IceBlock, ItemID.PurpleIceBlock,
            ItemID.RedIceBlock, ItemID.PinkIceBlock);
        anyEvilPowder = group("AnyEvilPowder", ItemID.VilePowder, ItemID.VilePowder, ItemID.ViciousPowder);

        anyNightsEdge = new(() => getName(ItemID.NightsEdge), ItemID.NightsEdge, ModContent.ItemType<NightsEdge>());
        RecipeGroup.RegisterGroup(nameof(ItemID.NightsEdge), anyNightsEdge);
        anyTrueNightsEdge = new(() => getName(ItemID.TrueNightsEdge), ItemID.TrueNightsEdge,
            ModContent.ItemType<TrueNightsEdge>());
        RecipeGroup.RegisterGroup(nameof(ItemID.TrueNightsEdge), anyTrueNightsEdge);
        anyMuramasa = new(() => getName(ItemID.Muramasa), ItemID.Muramasa, ModContent.ItemType<Muramasa>());
        RecipeGroup.RegisterGroup(nameof(ItemID.Muramasa), anyMuramasa);
        anyTerraBlade = new(() => getName(ItemID.TerraBlade), ItemID.TerraBlade, ModContent.ItemType<TerraBlade>());
        RecipeGroup.RegisterGroup(nameof(ItemID.TerraBlade), anyTerraBlade);
        anyExcalibur = new(() => getName(ItemID.Excalibur), ItemID.Excalibur, ModContent.ItemType<Excalibur>());
        RecipeGroup.RegisterGroup(nameof(ItemID.Excalibur), anyExcalibur);
        anyTrueExcalibur = new(() => getName(ItemID.TrueExcalibur), ItemID.TrueExcalibur,
            ModContent.ItemType<TrueExcalibur>());
        RecipeGroup.RegisterGroup(nameof(ItemID.TrueExcalibur), anyTrueExcalibur);
        anyFieryGreatsword = new(() => getName(ItemID.FieryGreatsword), ItemID.FieryGreatsword,
            ModContent.ItemType<FieryGreatsword>());
        RecipeGroup.RegisterGroup(nameof(ItemID.FieryGreatsword), anyFieryGreatsword);
        anyBladeOfGrass = new(() => getName(ItemID.BladeofGrass), ItemID.BladeofGrass,
            ModContent.ItemType<BladeOfGrass>());
        RecipeGroup.RegisterGroup(nameof(ItemID.BladeofGrass), anyBladeOfGrass);
    }

    public override void PostAddRecipes() {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            Recipe recipe = Main.recipe[i];
            // replace items with groups
            if (recipe.HasIngredient(ItemID.NightsEdge)) {
                recipe.RemoveIngredient(ItemID.NightsEdge);
                recipe.AddRecipeGroup(anyNightsEdge);
            }

            if (recipe.HasIngredient(ItemID.TrueNightsEdge)) {
                recipe.RemoveIngredient(ItemID.TrueNightsEdge);
                recipe.AddRecipeGroup(anyTrueNightsEdge);
            }

            if (recipe.HasIngredient(ItemID.Muramasa)) {
                recipe.RemoveIngredient(ItemID.Muramasa);
                recipe.AddRecipeGroup(anyMuramasa);
            }

            if (recipe.HasIngredient(ItemID.TerraBlade)) {
                recipe.RemoveIngredient(ItemID.TerraBlade);
                recipe.AddRecipeGroup(anyTerraBlade);
            }

            if (recipe.HasIngredient(ItemID.Excalibur)) {
                recipe.RemoveIngredient(ItemID.Excalibur);
                recipe.AddRecipeGroup(anyExcalibur);
            }

            if (recipe.HasIngredient(ItemID.TrueExcalibur)) {
                recipe.RemoveIngredient(ItemID.TrueExcalibur);
                recipe.AddRecipeGroup(anyTrueExcalibur);
            }

            if (recipe.HasIngredient(ItemID.FieryGreatsword)) {
                recipe.RemoveIngredient(ItemID.FieryGreatsword);
                recipe.AddRecipeGroup(anyFieryGreatsword);
            }

            if (recipe.HasIngredient(ItemID.BladeofGrass)) {
                recipe.RemoveIngredient(ItemID.BladeofGrass);
                recipe.AddRecipeGroup(anyBladeOfGrass);
            }
        }
    }

    public static string getName(int type) {
        return $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(type)}";
    }

    private static int group(string name, int named, params int[] items) {
        var label = getName(named);
        return RecipeGroup.RegisterGroup($"VanillaQoL:{name}", new RecipeGroup(() => label, items));
    }
}