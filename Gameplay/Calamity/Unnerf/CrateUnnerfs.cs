using System;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class CrateUnnerfs : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.crateLoot;
    }

    public override void ModifyItemLoot(Item item, ItemLoot loot) {
        var type = item.type;

        if (type == ModContent.ItemType<AstralCrate>()) {
            astral(loot, true);
        }
        else if (type == ModContent.ItemType<MonolithCrate>()) {
            astral(loot, false);
        }
        else if (type == ModContent.ItemType<BrimstoneCrate>()) {
            crag(loot, true);
        }
        else if (type == ModContent.ItemType<SlagCrate>()) {
            crag(loot, false);
        }
        else if (type == ModContent.ItemType<EutrophicCrate>()) {
            sunkenSea(loot, false);
        }
        else if (type == ModContent.ItemType<PrismCrate>()) {
            sunkenSea(loot, true);
        }
        else if (type == ModContent.ItemType<SulphurousCrate>()) {
            sulphur(loot, false);
        }
        else if (type == ModContent.ItemType<HydrothermalCrate>()) {
            sulphur(loot, true);
        }
        else {
            vanilla(loot, type);
        }
    }

    private static void vanilla(ItemLoot loot, int type) {
        switch (type) {
            case ItemID.WoodenCrate:
            case ItemID.WoodenCrateHard:
                loot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumMetalScrap>(), 4, 3, 5));
                break;

            case ItemID.IronCrate:
            case ItemID.IronCrateHard:
                loot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumMetalScrap>(), 4, 5, 8));
                loot.Add(ItemDropRule.Common(ModContent.ItemType<AncientBoneDust>(), 4, 5, 8));
                break;

            // the plain Golden Crate stops dropping in hardmode, so these three never needed gating
            case ItemID.GoldenCrate:
                loot.Add(ItemDropRule.Common(ItemID.FlareGun, 10));
                loot.Add(ItemDropRule.Common(ItemID.ShoeSpikes, 10));
                loot.Add(ItemDropRule.Common(ItemID.BandofRegeneration, 10));
                break;

            case ItemID.GoldenCrateHard:
                cond(loot, () => DownedBossSystem.downedYharon,
                    new CommonDrop(ModContent.ItemType<AuricOre>(), 100, 30, 40, 15));
                break;

            case ItemID.CorruptFishingCrate:
            case ItemID.CrimsonFishingCrate:
            case ItemID.CorruptFishingCrateHard:
            case ItemID.CrimsonFishingCrateHard:
                loot.Add(new CommonDrop(ModContent.ItemType<BlightedGel>(), 100, 5, 8, 15));
                break;

            case ItemID.HallowedFishingCrate:
            case ItemID.HallowedFishingCrateHard:
                cond(loot, () => DownedBossSystem.downedProvidence,
                    new CommonDrop(ModContent.ItemType<UnholyEssence>(), 100, 5, 10, 15));
                break;

            case ItemID.DungeonFishingCrate:
            case ItemID.DungeonFishingCrateHard:
                cond(loot, () => NPC.downedPlantBoss, ItemDropRule.Common(ItemID.Ectoplasm, 10, 1, 5));
                cond(loot, () => DownedBossSystem.downedPolterghast,
                    ItemDropRule.Common(ModContent.ItemType<Necroplasm>(), 10, 1, 5));
                break;

            case ItemID.JungleFishingCrate:
            case ItemID.JungleFishingCrateHard:
                // Murky Paste was 20% 1-3 here, but the item is gone from Calamity entirely
                // todo restore?
                cond(loot, () => NPC.downedPlantBoss,
                    ItemDropRule.Common(ModContent.ItemType<PerennialOre>(), 5, 16, 28),
                    new CommonDrop(ModContent.ItemType<PerennialBar>(), 100, 4, 7, 15));
                cond(loot, () => NPC.downedGolemBoss,
                    ItemDropRule.Common(ModContent.ItemType<PlagueCellCanister>(), 5, 3, 6));
                cond(loot, () => DownedBossSystem.downedProvidence,
                    ItemDropRule.Common(ModContent.ItemType<UelibloomOre>(), 5, 16, 28),
                    new CommonDrop(ModContent.ItemType<UelibloomBar>(), 100, 4, 7, 15));
                break;

            case ItemID.FloatingIslandFishingCrate:
            case ItemID.FloatingIslandFishingCrateHard:
                cond(loot, () => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator,
                    ItemDropRule.Common(ModContent.ItemType<AerialiteOre>(), 5, 16, 28),
                    new CommonDrop(ModContent.ItemType<AerialiteBar>(), 100, 4, 7, 15));
                cond(loot, () => Main.hardMode,
                    ItemDropRule.Common(ModContent.ItemType<EssenceofSunlight>(), 5, 2, 4));
                cond(loot, () => NPC.downedMoonlord,
                    ItemDropRule.Common(ModContent.ItemType<ExodiumCluster>(), 5, 16, 28));
                break;

            case ItemID.FrozenCrate:
            case ItemID.FrozenCrateHard:
                cond(loot, cryonicAvailable,
                    ItemDropRule.Common(ModContent.ItemType<CryonicOre>(), 5, 16, 28),
                    new CommonDrop(ModContent.ItemType<CryonicBar>(), 100, 4, 7, 15));
                cond(loot, () => Main.hardMode,
                    ItemDropRule.Common(ModContent.ItemType<EssenceofEleum>(), 5, 2, 4));
                break;

            case ItemID.LavaCrate:
            case ItemID.LavaCrateHard:
                cond(loot, () => Main.hardMode,
                    ItemDropRule.Common(ModContent.ItemType<EssenceofHavoc>(), 5, 2, 4));
                break;
        }
    }

    /// Cryogen alone isn't enough, Calamity wanted two of the three mechs down as well
    private static bool cryonicAvailable() {
        if (!DownedBossSystem.downedCryogen) {
            return false;
        }

        return (NPC.downedMechBoss1 ? 1 : 0) + (NPC.downedMechBoss2 ? 1 : 0) + (NPC.downedMechBoss3 ? 1 : 0) >= 2;
    }

    private static void cond(ItemLoot loot, Func<bool> check, params IItemDropRule[] drops) {
        var rule = when(check);
        foreach (var drop in drops) {
            rule.OnSuccess(drop);
        }

        loot.Add(rule);
    }

    /// Calamity kept the item but cut the odds so grab the rule and put the old numbers back, rather than adding a
    /// second roll on top.
    private static void patchDrop(ItemLoot loot, int item, IItemDropRule canonical) {
        if (!loot.Get(false).Any(rule => rule is CommonDrop drop && drop.itemId == item)) {
            VanillaQoL.instance.Logger.Warn($"No Calamity crate rule for item {item}!");
            return;
        }

        loot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == item);
        loot.Add(canonical);
    }

    private static void astral(ItemLoot loot, bool hardmode) {
        patchDrop(loot, ModContent.ItemType<AstrophageItem>(),
            ItemDropRule.Common(ModContent.ItemType<AstrophageItem>(), 10));

        if (hardmode) {
            patchDrop(loot, ModContent.ItemType<StarblightSoot>(),
                ItemDropRule.Common(ModContent.ItemType<StarblightSoot>(), 1, 5, 10));
        }

        loot.Add(ItemDropRule.Common(ItemID.FallenStar, 1, 5, 10));
        loot.Add(ItemDropRule.Common(ItemID.Meteorite, 5, 10, 20));
        loot.Add(ItemDropRule.Common(ItemID.MeteoriteBar, 10, 1, 3));
        loot.Add(ItemDropRule.Common(ModContent.ItemType<TwinklerItem>(), 5, 1, 3));
        loot.Add(ItemDropRule.Common(ItemID.EnchantedNightcrawler, 5, 1, 3));
        loot.Add(ItemDropRule.Common(ModContent.ItemType<ArcturusAstroidean>(), 5, 1, 3));
        loot.Add(ItemDropRule.Common(ItemID.Firefly, 3, 1, 3));

        if (!hardmode) {
            return;
        }

        var aureus = when(() => DownedBossSystem.downedAstrumAureus);
        aureus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AureusCell>(), 5, 2, 5));
        aureus.OnSuccess(new OneFromOptionsDropRule(10, 1,
            ModContent.ItemType<AstralScythe>(), ModContent.ItemType<TitanArm>(),
            ModContent.ItemType<StellarCannon>(), ModContent.ItemType<AstralachneaStaff>(),
            ModContent.ItemType<HivePod>(), ModContent.ItemType<StellarKnife>(),
            ModContent.ItemType<StarbusterCore>()));
        aureus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AstralInjection>(), 10, 1, 3));
        aureus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GravityNormalizerPotion>(), 10, 1, 3));
        loot.Add(aureus);

        var deus = when(() => DownedBossSystem.downedAstrumDeus);
        deus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AstralOre>(), 5, 10, 20));
        deus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AstralBar>(), 10, 1, 3));
        deus.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MeldBlob>(), 4, 5, 10));
        loot.Add(deus);
    }

    private static void crag(ItemLoot loot, bool hardmode) {
        loot.Add(ItemDropRule.Common(ItemID.Obsidian, 1, 2, 5));
        loot.Add(ItemDropRule.Common(ItemID.Hellstone, 4, 2, 5));
        loot.Add(ItemDropRule.Common(ItemID.HellstoneBar, 10, 1, 3));
        loot.Add(ItemDropRule.Common(ItemID.InfernoPotion, 10, 1, 3));
        // todo Demonic Bone Ash was 1-4 amount here, but the item itself no longer exists in Calamity at all, add it back?

        if (!hardmode) {
            return;
        }

        var brimmy = when(() => DownedBossSystem.downedBrimstoneElemental);
        brimmy.OnSuccess(ItemDropRule.Common(ModContent.ItemType<UnholyCore>(), 10, 1, 3));
        loot.Add(brimmy);

        var providence = when(() => DownedBossSystem.downedProvidence);
        providence.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Bloodstone>(), 2, 1, 3));
        loot.Add(providence);
    }

    private static void sunkenSea(ItemLoot loot, bool hardmode) {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<SeaMinnowItem>(), 5, 1, 3));
        patchDrop(loot, ModContent.ItemType<PrismShard>(),
            ItemDropRule.Common(ModContent.ItemType<PrismShard>(), 1, 5, 10));

        var scourge = when(() => DownedBossSystem.downedDesertScourge);
        scourge.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SeaPrism>(), 5, 2, 5));
        loot.Add(scourge);

        if (!hardmode) {
            return;
        }

        var clam = when(() => DownedBossSystem.downedCLAM);
        clam.OnSuccess(new CommonDrop(ModContent.ItemType<MolluskHusk>(), 100, 2, 5, 12));
        clam.OnSuccess(new OneFromOptionsNotScaledWithLuckDropRule(100, 7,
            ModContent.ItemType<ClamCrusher>(), ModContent.ItemType<ClamorRifle>(),
            ModContent.ItemType<Poseidon>(), ModContent.ItemType<ShellfishStaff>()));
        loot.Add(clam);
    }

    private static void sulphur(ItemLoot loot, bool hydrothermal) {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<Acidwood>(), 1, 5, 10));
        loot.Add(ItemDropRule.Common(ModContent.ItemType<AnechoicCoating>(), 10, 1, 3));

        var acidRain1 = when(() => DownedBossSystem.downedEoCAcidRain);
        acidRain1.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SulphuricScale>(), 10, 1, 3));
        loot.Add(acidRain1);

        if (!hydrothermal) {
            var abyss = when(() => DownedBossSystem.downedSlimeGod || Main.hardMode);
            abyss.OnSuccess(new OneFromOptionsDropRule(10, 1,
                ModContent.ItemType<BallOFugu>(), ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BlackAnurian>(), ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>()));
            abyss.OnSuccess(new OneFromOptionsDropRule(4, 1,
                ModContent.ItemType<AnechoicPlating>(), ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(), ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<TorrentialTear>()));
            loot.Add(abyss);
            return;
        }

        var acidRain2 = when(() => DownedBossSystem.downedAquaticScourgeAcidRain);
        acidRain2.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CorrodedFossil>(), 10, 1, 3));
        acidRain2.OnSuccess(new OneFromOptionsDropRule(10, 1,
            ModContent.ItemType<SulphurousGrabber>(), ModContent.ItemType<FlakToxicannon>(),
            ModContent.ItemType<BelchingSaxophone>(), ModContent.ItemType<SlitheringEels>(),
            ModContent.ItemType<SkyfinBombers>(), ModContent.ItemType<SpentFuelContainer>(),
            ModContent.ItemType<NuclearFuelRod>()));
        loot.Add(acidRain2);

        var deepAbyss = when(() => DownedBossSystem.downedLeviathan);
        deepAbyss.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DepthCells>(), 5, 2, 5));
        deepAbyss.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Lumenyl>(), 5, 2, 5));
        deepAbyss.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PlantyMush>(), 5, 2, 5));
        loot.Add(deepAbyss);

        var scoria = when(() => NPC.downedGolemBoss);
        scoria.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ScoriaOre>(), 5, 16, 28));
        scoria.OnSuccess(new CommonDrop(ModContent.ItemType<ScoriaBar>(), 100, 4, 7, 15));
        loot.Add(scoria);

        var acidRain3 = when(() => DownedBossSystem.downedPolterghast && DownedBossSystem.downedBoomerDuke);
        acidRain3.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ReaperTooth>(), 10, 1, 5));
        loot.Add(acidRain3);
    }

    private static LeadingConditionRule when(Func<bool> check) {
        return new LeadingConditionRule(new Progress(check));
    }
}

/**
 * DropHelper-less shim to bridge Func&lt;bool&gt; -> IItemDropRuleCondition.
 */
public class Progress : IItemDropRuleCondition {
    private readonly Func<bool> check;
    private readonly string description;

    public Progress(Func<bool> check, string description = "") {
        this.check = check;
        this.description = description;
    }

    public bool CanDrop(DropAttemptInfo info) {
        return check();
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    /// the bestiary skips blank ones, so an empty description is "no note" rather than a crash
    public string GetConditionDescription() {
        return description;
    }
}
