using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

// These came over from CalamityQOL, but Calamity has since dropped every single one of them, so they aren't Calamity's features any more - which
// means they work with Calamity loaded too, unlike everything in CalamityRecipes.
// 'I don't like removing features' (but fr)
public class CalamityLRecipes : ModSystem {
    public override void AddRecipes() {
        var config = QoLRecipeConfig.Instance;
        if (config.debuffImmunityRecipes) {
            debuffImmunity();
        }

        if (config.lootAccessories) {
            accessories();
        }

        if (config.lootGear) {
            gear();
        }

        if (config.lootTools) {
            tools();
        }

        if (config.lootKeys) {
            keys();
        }
    }

    #region Debuff immunity

    private static void debuffImmunity() {
        Recipe.Create(ItemID.ArmorPolish)
            .AddIngredient(ItemID.Bone, 50)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.AdhesiveBandage)
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ItemID.Gel, 50)
            .AddIngredient(ItemID.HealingPotion)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.Bezoar)
            .AddIngredient(ItemID.Stinger, 15)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.Nazar)
            .AddIngredient(ItemID.SoulofNight, 15)
            .AddIngredient(ItemID.Lens, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.Vitamins)
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.Waterleaf, 5)
            .AddIngredient(ItemID.Blinkroot, 5)
            .AddIngredient(ItemID.Daybloom, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.Blindfold)
            .AddIngredient(ItemID.Silk, 30)
            .AddIngredient(ItemID.TatteredCloth, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.TrifoldMap)
            .AddIngredient(ItemID.Silk, 20)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.FastClock)
            .AddIngredient(ItemID.Timer1Second)
            .AddIngredient(ItemID.PixieDust, 15)
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.Megaphone)
            .AddIngredient(ItemID.Wire, 10)
            .AddRecipeGroup(RecipeGroups.anyCobaltBar, 5)
            .AddIngredient(ItemID.Ruby, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.PocketMirror)
            .AddIngredient(ItemID.Glass, 10)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 4)
            .AddIngredient(ItemID.CrystalShard, 2)
            .AddIngredient(ItemID.SoulofNight, 2)
            .AddTile(TileID.Anvils)
            .Register();
    }

    #endregion

    #region Accessories

    private static void accessories() {
        Recipe.Create(ItemID.FeralClaws)
            .AddIngredient(ItemID.Leather, 10)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.BandofRegeneration)
            .AddIngredient(ItemID.Shackle)
            .AddIngredient(ItemID.LifeCrystal)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.ShoeSpikes)
            .AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddIngredient(ItemID.Spike, 10)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.FlareGun)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 5)
            .AddIngredient(ItemID.Torch, 10)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.MetalDetector)
            .AddIngredient(ItemID.Wire, 10)
            .AddIngredient(ItemID.SpelunkerGlowstick, 5)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.DPSMeter)
            .AddIngredient(ItemID.Wire, 10)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.FrozenTurtleShell)
            .AddIngredient(ItemID.TurtleShell, 3)
            .AddTile(TileID.IceMachine)
            .Register();

        Recipe.Create(ItemID.CelestialMagnet)
            .AddIngredient(ItemID.TreasureMagnet)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.MagicQuiver)
            .AddIngredient(ItemID.EndlessQuiver)
            .AddIngredient(ItemID.PixieDust, 10)
            .AddIngredient(ItemID.Lens, 5)
            .AddIngredient(ItemID.SoulofLight, 8)
            .AddTile(TileID.CrystalBall)
            .Register();

        // vanilla wants an Avenger Emblem for both of these, which means crafting the emblem you
        // didn't want first. same idea, one tier of faffing about removed
        Recipe.Create(ItemID.MechanicalGlove)
            .AddIngredient(ItemID.PowerGlove)
            .AddIngredient(ItemID.WarriorEmblem)
            .AddIngredient(ItemID.SoulofFright)
            .AddIngredient(ItemID.SoulofMight)
            .AddIngredient(ItemID.SoulofSight)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        Recipe.Create(ItemID.CelestialEmblem)
            .AddIngredient(ItemID.CelestialMagnet)
            .AddIngredient(ItemID.SorcererEmblem)
            .AddIngredient(ItemID.SoulofFright)
            .AddIngredient(ItemID.SoulofMight)
            .AddIngredient(ItemID.SoulofSight)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }

    #endregion

    #region Weapons and vanity

    private static void gear() {
        Recipe.Create(ItemID.IceBoomerang)
            .AddIngredient(ItemID.WoodenBoomerang)
            .AddRecipeGroup(RecipeGroups.anyIceBlock, 20)
            .AddIngredient(ItemID.SnowBlock, 10)
            .AddIngredient(ItemID.Shiverthorn)
            .AddTile(TileID.IceMachine)
            .Register();

        Recipe.Create(ItemID.Shuriken, 50)
            .AddRecipeGroup(RecipeGroupID.IronBar)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.ThrowingKnife, 50)
            .AddRecipeGroup(RecipeGroupID.IronBar)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.PharaohsMask)
            .AddIngredient(ItemID.AncientCloth, 3)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.PharaohsRobe)
            .AddIngredient(ItemID.AncientCloth, 4)
            .AddTile(TileID.Loom)
            .Register();
    }

    #endregion

    #region Tools and stations

    private static void tools() {
        Recipe.Create(ItemID.MagicMirror)
            .AddRecipeGroup(RecipeGroups.anySilverBar, 10)
            .AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.IceMirror)
            .AddRecipeGroup(RecipeGroups.anySilverBar, 5)
            .AddRecipeGroup(RecipeGroups.anyIceBlock, 20)
            .AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.BugNet)
            .AddIngredient(ItemID.Cobweb, 30)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.StaffofRegrowth)
            .AddIngredient(ItemID.RichMahogany, 10)
            .AddIngredient(ItemID.JungleSpores, 5)
            .AddIngredient(ItemID.JungleRose)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.IceMachine)
            .AddRecipeGroup(RecipeGroups.anyIceBlock, 25)
            .AddIngredient(ItemID.SnowBlock, 15)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.SkyMill)
            .AddIngredient(ItemID.SunplateBlock, 10)
            .AddIngredient(ItemID.Cloud, 5)
            .AddIngredient(ItemID.RainCloud, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.DesertMinecart)
            .AddIngredient(ItemID.SandstoneBrick, 20)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 6)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddTile(TileID.Anvils)
            .Register();
    }

    #endregion

    #region Keys and summons

    private static void keys() {
        Recipe.Create(ItemID.TempleKey)
            .AddIngredient(ItemID.JungleSpores, 15)
            .AddIngredient(ItemID.RichMahogany, 15)
            .AddIngredient(ItemID.SoulofLight, 15)
            .AddIngredient(ItemID.SoulofNight, 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();

        Recipe.Create(ItemID.LihzahrdPowerCell)
            .AddIngredient(ItemID.LihzahrdBrick, 15)
            .AddTile(TileID.LihzahrdFurnace)
            .Register();

        Recipe.Create(ItemID.ShadowKey)
            .AddIngredient(ItemID.GoldenKey)
            .AddIngredient(ItemID.Obsidian, 20)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.GuideVoodooDoll)
            .AddIngredient(ItemID.Leather, 2)
            .AddRecipeGroup(RecipeGroups.anyEvilPowder, 10)
            .AddTile(TileID.Hellforge)
            .Register();

        Recipe.Create(ItemID.QueenSlimeCrystal)
            .AddIngredient(ItemID.CrystalShard, 20)
            .AddIngredient(ItemID.PinkGel, 10)
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddTile(TileID.Solidifier)
            .Register();
    }

    #endregion
}
