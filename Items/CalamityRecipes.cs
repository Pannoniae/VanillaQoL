using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Condition = Terraria.Condition;

namespace VanillaQoL.Items;

// Calamity recipes, for people who don't have Calamity. (used to be Calamity QOL for Vanilla)
// fyi Cal removed some of these but they were quite pog so we keep them
public class CalamityRecipes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active;
    }

    public override void AddRecipes() {
        var config = CalamityQoLConfig.Instance;
        if (config.evilBiomeSwaps) {
            evilBiomeSwaps();
        }

        if (config.earlyWeaponRecipes) {
            earlyWeapons();
        }

        if (config.accessoryRecipes) {
            accessories();
        }

if (config.armourRecipes) {
            armour();
        }

        if (config.summonRecipes) {
            summons();
        }

        if (config.miscRecipes) {
            misc();
            tombstones();
        }
    }

    public override void PostSetupContent() {
        if (CalamityQoLConfig.Instance.shimmerRecipes) {
            shimmer();
        }
    }

    #region Evil biome swaps

    private static void evilBiomeSwaps() {
        swap(ItemID.CrimsonRod, ItemID.Vilethorn, TileID.Anvils);
        swap(ItemID.TheRottedFork, ItemID.BallOHurt, TileID.Anvils);
        swap(ItemID.TheUndertaker, ItemID.Musket, TileID.Anvils);
        swap(ItemID.DartPistol, ItemID.DartRifle, TileID.Anvils);
        swap(ItemID.ChainGuillotines, ItemID.FetidBaghnakhs, TileID.Anvils);
        swap(ItemID.ClingerStaff, ItemID.SoulDrain, TileID.Anvils);
        swap(ItemID.CrimsonHeart, ItemID.ShadowOrb, TileID.TinkerersWorkbench);
        swap(ItemID.BrainOfConfusion, ItemID.WormScarf, TileID.TinkerersWorkbench);
        swap(ItemID.TendonHook, ItemID.WormHook, TileID.TinkerersWorkbench);
        swap(ItemID.PutridScent, ItemID.FleshKnuckles, TileID.TinkerersWorkbench);
    }

    // okay I think this is pretty clever ngl
    private static void swap(int crimson, int corruption, int tile) {
        oneWaySwap(crimson, corruption, tile);
        oneWaySwap(corruption, crimson, tile);
    }

    private static void oneWaySwap(int result, int ingredient, int tile) {
        Recipe.Create(result)
            .AddIngredient(ingredient)
            .AddTile(tile)
            .AddCondition(Condition.InGraveyard)
            .Register()
            .DisableDecraft();
    }

    #endregion

    #region Early weapons

    private static void earlyWeapons() {
        Recipe.Create(ItemID.WoodenBoomerang)
            .AddRecipeGroup(RecipeGroupID.Wood, 7)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.WandofSparking)
            .AddRecipeGroup(RecipeGroupID.Wood, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddIngredient(ItemID.FallenStar)
            .AddCondition(Condition.NotRemixWorld)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.BabyBirdStaff)
            .AddIngredient(ItemID.Bird)
            .AddRecipeGroup(RecipeGroupID.Wood, 8)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.SlimeStaff)
            .AddRecipeGroup(RecipeGroupID.Wood, 6)
            .AddIngredient(ItemID.Gel, 40)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.WaterBolt)
            .AddIngredient(ItemID.SpellTome)
            .AddIngredient(ItemID.Waterleaf, 3)
            .AddIngredient(ItemID.WaterCandle)
            .AddTile(TileID.Bookcases)
            .Register()
            .DisableDecraft();

        // Calamity uses Pearl Shards, which we ain't got
        Recipe.Create(ItemID.EnchantedSword)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 12)
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Ruby)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        // same, theirs wants Aerialite Bars
        starfury(ItemID.GoldBroadsword);
        starfury(ItemID.PlatinumBroadsword);

Recipe.Create(ItemID.RocketI, 100)
            .AddRecipeGroup(RecipeGroupID.IronBar)
            .AddIngredient(ItemID.EmptyBullet, 100)
            .AddIngredient(ItemID.ExplosivePowder, 4)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.RocketII, 100)
            .AddRecipeGroup(RecipeGroupID.IronBar)
            .AddIngredient(ItemID.EmptyBullet, 100)
            .AddIngredient(ItemID.ExplosivePowder, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.PulseBow)
            .AddIngredient(ItemID.ShroomiteBar, 16)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft();
    }

    private static void starfury(int sword) {
        Recipe.Create(ItemID.Starfury)
            .AddIngredient(sword)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();
    }

    #endregion

    #region Accessories

    private static void accessories() {
        Recipe.Create(ItemID.PortableStool)
            .AddRecipeGroup(RecipeGroupID.Wood, 10)
            .AddTile(TileID.Sawmill)
            .Register();

        Recipe.Create(ItemID.HermesBoots)
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ItemID.SwiftnessPotion, 5)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.Aglet)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.AnkletoftheWind)
            .AddIngredient(ItemID.JungleSpores, 15)
            .AddIngredient(ItemID.Cloud, 5)
            .AddIngredient(ItemID.PinkGel, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.IceSkates)
            .AddIngredient(ItemID.FlinxFur, 3)
            .AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.WaterWalkingBoots)
            .AddIngredient(ItemID.Leather, 5)
            .AddIngredient(ItemID.WaterWalkingPotion, 5)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.LavaCharm)
            .AddIngredient(ItemID.LavaBucket, 3)
            .AddIngredient(ItemID.Obsidian, 5)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 5)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        Recipe.Create(ItemID.ObsidianRose)
            .AddIngredient(ItemID.JungleRose)
            .AddIngredient(ItemID.Obsidian, 5)
            .AddIngredient(ItemID.Hellstone, 5)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.BlizzardinaBottle)
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.Cloud, 15)
            .AddRecipeGroup(RecipeGroups.anySnowBlock, 30)
            .AddIngredient(ItemID.Feather, 3)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.CloudinaBottle)
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.Cloud, 30)
            .AddIngredient(ItemID.Feather, 2)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.SandstorminaBottle)
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.Cloud, 15)
            .AddRecipeGroup(RecipeGroupID.Sand, 40)
            .AddIngredient(ItemID.Feather, 3)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        Recipe.Create(ItemID.FrogLeg)
            .AddIngredient(ItemID.Frog, 6)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        Recipe.Create(ItemID.LuckyHorseshoe)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 8)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.ShinyRedBalloon)
            .AddIngredient(ItemID.WhiteString)
            .AddIngredient(ItemID.Cloud, 10)
            .AddTile(TileID.Solidifier)
            .Register();

        Recipe.Create(ItemID.CobaltShield)
            .AddRecipeGroup(RecipeGroups.anyCobaltBar, 5)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        Recipe.Create(ItemID.FlameWakerBoots)
            .AddIngredient(ItemID.Silk, 8)
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.FlowerBoots)
            .AddIngredient(ItemID.Silk, 7)
            .AddIngredient(ItemID.JungleRose)
            .AddIngredient(ItemID.JungleGrassSeeds, 5)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.HandWarmer)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.Radar)
            .AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddTile(TileID.Anvils)
            .Register();

Recipe.Create(ItemID.BouncingShield)
            .AddRecipeGroup(RecipeGroups.anyCobaltBar, 12)
            .AddIngredient(ItemID.SoulofLight, 4)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

}

    #endregion

    #region Armour

    private static void armour() {
        eskimo(ItemID.EskimoHood, 4, 1);
        eskimo(ItemID.EskimoCoat, 8, 2);
        eskimo(ItemID.EskimoPants, 6, 1);

}

    private static void eskimo(int piece, int silk, int fur) {
        Recipe.Create(piece)
            .AddIngredient(ItemID.Silk, silk)
            .AddIngredient(ItemID.FlinxFur, fur)
            .AddTile(TileID.Loom)
            .Register();
    }

    #endregion

    #region Summons and keys

    private static void summons() {
        // Calamity buys this with Blood Orbs, which don't exist here
        Recipe.Create(ItemID.BloodMoonStarter)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 20)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

        Recipe.Create(ItemID.SnowGlobe)
            .AddRecipeGroup(RecipeGroups.anySnowBlock, 10)
            .AddIngredient(ItemID.Glass, 5)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddTile(TileID.Anvils)
            .Register()
            .DisableDecraft();

}

    #endregion

    #region Miscellaneous

    private static void misc() {
        Recipe.Create(ItemID.Leather)
            .AddIngredient(ItemID.Vertebrae, 2)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.Umbrella)
            .AddIngredient(ItemID.Silk, 5)
            .AddRecipeGroup(RecipeGroups.anyCopperBar, 2)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.CatBast)
            .AddRecipeGroup(RecipeGroupID.IronBar, 7)
            .AddRecipeGroup(RecipeGroups.anyGoldBar, 3)
            .AddIngredient(ItemID.Ruby)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.EncumberingStone)
            .AddRecipeGroup(RecipeGroups.anyStoneBlock, 100)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.LifeCrystal)
            .AddRecipeGroup(RecipeGroups.anyStoneBlock, 5)
            .AddIngredient(ItemID.Ruby, 2)
            .AddIngredient(ItemID.HealingPotion)
            .AddTile(TileID.Anvils)
            .Register();

        Recipe.Create(ItemID.MagicConch)
            .AddIngredient(ItemID.ShellPileBlock, 20)
            .AddIngredient(ItemID.WhitePearl)
            .AddTile(TileID.Anvils)
            .Register();

}

    private static readonly short[] woodenTombstones = [ItemID.CrossGraveMarker, ItemID.GraveMarker];

    private static readonly short[] stoneTombstones =
        [ItemID.Gravestone, ItemID.Headstone, ItemID.Obelisk, ItemID.Tombstone];

    private static readonly short[] goldenTombstones = [
        ItemID.RichGravestone1, ItemID.RichGravestone2, ItemID.RichGravestone3, ItemID.RichGravestone4,
        ItemID.RichGravestone5
    ];

    private static void tombstones() {
        foreach (var stone in woodenTombstones) {
            Recipe.Create(stone)
                .AddRecipeGroup(RecipeGroupID.Wood, 15)
                .AddTile(TileID.Sawmill)
                .Register()
                .DisableDecraft();
        }

        foreach (var stone in stoneTombstones) {
            Recipe.Create(stone)
                .AddRecipeGroup(RecipeGroups.anyStoneBlock, 15)
                .AddTile(TileID.HeavyWorkBench)
                .Register()
                .DisableDecraft();
        }

        foreach (var stone in goldenTombstones) {
            Recipe.Create(stone)
                .AddRecipeGroup(RecipeGroups.anyStoneBlock, 15)
                .AddRecipeGroup(RecipeGroups.anyGoldBar)
                .AddTile(TileID.HeavyWorkBench)
                .Register()
                .DisableDecraft();
        }
    }

    #endregion

    #region Shimmer

    private static void shimmer() {
        var convert = ItemID.Sets.ShimmerTransformToItem;
        // the three info accessories cycle
        convert[ItemID.DPSMeter] = ItemID.LifeformAnalyzer;
        convert[ItemID.LifeformAnalyzer] = ItemID.Stopwatch;
        convert[ItemID.Stopwatch] = ItemID.DPSMeter;

        convert[ItemID.EnchantedSword] = ItemID.Terragrim;
        convert[ItemID.Terragrim] = ItemID.EnchantedSword;
    }

    #endregion
}

// Calamity recipe anti-annoyance
public class CalamityRecipeTweaks : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active && CalamityQoLConfig.Instance.vanillaRecipeTweaks;
    }

    // moved off the Mythril Anvil so early hardmode isn't gated behind it
    private static readonly int[] begoneFromMythrilAnvil = [
        ItemID.MechanicalEye, ItemID.MechanicalWorm, ItemID.MechanicalSkull, ItemID.MechdusaSummon,
        ItemID.DaoofPow, ItemID.Chik, ItemID.MeteorStaff, ItemID.CoolWhip,
        ItemID.AngelWings, ItemID.DemonWings, ItemID.FairyWings, ItemID.FairyBell,
        ItemID.CursedArrow, ItemID.CursedBullet, ItemID.IchorArrow, ItemID.IchorBullet
    ];

    // the bars go second, it's the wrong way round and it's triggering my OCD
    private static readonly int[] wrongIngredientOrder = [
        ItemID.Flamarang, ItemID.PhoenixBlaster, ItemID.FireproofBugNet,
        ItemID.BeetleHelmet, ItemID.BeetleScaleMail, ItemID.BeetleShell, ItemID.BeetleLeggings
    ];

    private readonly List<(Func<Recipe, bool> which, Action<Recipe> edit)> edits = [];

    public override void PostAddRecipes() {
        cheaper();
        untangle();

        foreach (var recipe in Main.recipe) {
            foreach (var (which, edit) in edits) {
                if (which(recipe)) {
                    edit(recipe);
                }
            }
        }

        edits.Clear();
    }

    private void cheaper() {
        stack(ItemID.Leather, ItemID.RottenChunk, 2);
        stack(ItemID.GoblinBattleStandard, ItemID.TatteredCloth, 5);
        stack(ItemID.ChlorophyteBar, ItemID.ChlorophyteOre, 4);
        stack(ItemID.OrichalcumAnvil, ItemID.OrichalcumBar, 10);
        stack(ItemID.ShroomiteBar, ItemID.GlowingMushroom, 5);
        stack(ItemID.TrueExcalibur, ItemID.ChlorophyteBar, 12);

        // yield 50 arrows per star instead of a measly 10
        edit(ItemID.JestersArrow, r => bulk(r, ItemID.WoodenArrow, 50));
        edit(ItemID.TeleportationPotion, r => bulk(r, ItemID.BottledWater, 5));
        edit(ItemID.Beenade, r => bulk(r, ItemID.Grenade, 4));

        edit(ItemID.WormFood, r => {
            restack(r, ItemID.VilePowder, 20);
            restack(r, ItemID.RottenChunk, 10);
        });
        edit(ItemID.BloodySpine, r => {
            restack(r, ItemID.ViciousPowder, 20);
            restack(r, ItemID.Vertebrae, 10);
        });
        // don't require so much of the souls, it's a pain to farm
        edit(ItemID.TrueNightsEdge, r => {
            restack(r, ItemID.SoulofSight, 3);
            restack(r, ItemID.SoulofMight, 3);
            restack(r, ItemID.SoulofFright, 3);
        });

        drop(ItemID.OpticStaff, ItemID.HallowedBar);
        drop(ItemID.PumpkinMoonMedallion, ItemID.HallowedBar);
        drop(ItemID.NaughtyPresent, ItemID.SoulofFright);
        drop(ItemID.FairyBell, ItemID.SoulofSight);
    }

    private void untangle() {
        edits.Add((r => r.Mod is null && Array.IndexOf(begoneFromMythrilAnvil, r.createItem.type) != -1, r => {
            var index = r.requiredTile.IndexOf(TileID.MythrilAnvil);
            if (index != -1) {
                r.requiredTile[index] = TileID.Anvils;
            }
        }));

        edits.Add((r => r.Mod is null && Array.IndexOf(wrongIngredientOrder, r.createItem.type) != -1, r => {
            if (r.requiredItem.Count >= 2) {
                // Calamity has a comment swearing blind that the tuple swap doesn't work here.
                // I can't see why it wouldn't, but I also can't test it right now and they clearly
                // got bitten by something, so: the boring version
                var first = r.requiredItem[0];
                r.requiredItem[0] = r.requiredItem[1];
                r.requiredItem[1] = first;
            }
        }));
    }

    private void edit(int item, Action<Recipe> what) {
        edits.Add((r => r.Mod is null && r.HasResult(item), what));
    }

    private void stack(int item, int ingredient, int count) {
        edit(item, r => restack(r, ingredient, count));
    }

    private void drop(int item, int ingredient) {
        edit(item, r => r.RemoveIngredient(ingredient));
    }

    // tML has no "just change the count" so we have to fish the ingredient out and put it back
    private static void restack(Recipe recipe, int ingredient, int count) {
        if (recipe.TryGetIngredient(ingredient, out var item)) {
            item.stack = count;
        }
    }

    // craft n at a time out of n of the ingredient, rather than making you click n times
    private static void bulk(Recipe recipe, int ingredient, int count) {
        if (recipe.createItem.stack < count) {
            recipe.createItem.stack = count;
        }

        restack(recipe, ingredient, count);
    }
}
