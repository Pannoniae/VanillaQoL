using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.DesertScourge;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

[JITWhenModsEnabled("CalamityMod")]
public class SandyAnglingKit : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.anglingKits;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Blue;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        itemLoot.Add(AnglingKitDrops.table(new Conditions.NotExpert(), 20, 3, 1, 2));
        itemLoot.Add(AnglingKitDrops.table(new Conditions.IsExpert(), 16, 2, 2, 3));
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class BleachedAnglingKit : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.anglingKits;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SandyAnglingKit>();
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Pink;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        itemLoot.Add(AnglingKitDrops.table(new Conditions.NotExpert(), 18, 3, 3, 4));
        itemLoot.Add(AnglingKitDrops.table(new Conditions.IsExpert(), 14, 2, 4, 5));
    }
}

/// where the kits come from, and the shared contents - both bags hold the same seven things, only the odds
/// and the amt of coins differ
[JITWhenModsEnabled("CalamityMod")]
public class AnglingKitDrops : GlobalNPC {
    internal static LeadingConditionRule table(IItemDropRuleCondition when, int infoChance, int potionChance,
        int min, int max) {
        var rule = new LeadingConditionRule(when);
        rule.OnSuccess(ItemDropRule.Common(ItemID.FishermansGuide, infoChance));
        rule.OnSuccess(ItemDropRule.Common(ItemID.WeatherRadio, infoChance));
        rule.OnSuccess(ItemDropRule.Common(ItemID.Sextant, infoChance));
        rule.OnSuccess(ItemDropRule.Common(ItemID.FishingPotion, potionChance, 2, 3));
        rule.OnSuccess(ItemDropRule.Common(ItemID.SonarPotion, potionChance, 2, 3));
        rule.OnSuccess(ItemDropRule.Common(ItemID.CratePotion, potionChance, 2, 3));
        rule.OnSuccess(ItemDropRule.Common(ItemID.GoldCoin, 1, min, max));
        return rule;
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.anglingKits;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.type == ModContent.NPCType<DesertScourgeHead>()) {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SandyAnglingKit>()));
        }

        if (npc.type == ModContent.NPCType<AquaticScourgeHead>()) {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(),
                ModContent.ItemType<BleachedAnglingKit>()));
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class AnglingKitBagDrops : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.anglingKits;
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ModContent.ItemType<DesertScourgeBag>()) {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SandyAnglingKit>()));
        }

        if (item.type == ModContent.ItemType<AquaticScourgeBag>()) {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BleachedAnglingKit>()));
        }
    }
}
