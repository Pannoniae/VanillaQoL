using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

// It's the same starting kit everyone crafts in the first ten minutes anyway, so you may as well skip it
public class StarterBag : ModItem {
    // TODO steal a sprite from ourselves at some point
    public override string Texture => $"Terraria/Images/Item_{ItemID.GoodieBag}";

    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot loot) {
        // the world decides whether you're a tin world or a copper world
        var tin = new LeadingConditionRule(new TinWorld());
        tin.OnSuccess(ItemDropRule.Common(ItemID.TinBroadsword));
        tin.OnSuccess(ItemDropRule.Common(ItemID.TinBow));
        tin.OnSuccess(ItemDropRule.Common(ItemID.TopazStaff));
        tin.OnSuccess(ItemDropRule.Common(ItemID.TinHammer));
        tin.OnFailedConditions(ItemDropRule.Common(ItemID.CopperBroadsword));
        tin.OnFailedConditions(ItemDropRule.Common(ItemID.CopperBow));
        tin.OnFailedConditions(ItemDropRule.Common(ItemID.AmethystStaff));
        tin.OnFailedConditions(ItemDropRule.Common(ItemID.CopperHammer));
        loot.Add(tin);

        loot.Add(ItemDropRule.Common(ItemID.WoodenArrow, 1, 100, 100));
        loot.Add(ItemDropRule.Common(ItemID.ManaCrystal));
        loot.Add(ItemDropRule.Common(ItemID.Rope, 1, 50, 50));
        loot.Add(ItemDropRule.Common(ItemID.Torch, 1, 25, 25));
        loot.Add(ItemDropRule.Common(ItemID.RecallPotion, 1, 3, 3));

        // no point handing out wormholes when there's nobody to warp to
        var multiplayer = new LeadingConditionRule(new InMultiplayer());
        multiplayer.OnSuccess(ItemDropRule.Common(ItemID.WormholePotion, 1, 3, 3));
        loot.Add(multiplayer);
    }
}

public class TinWorld : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        return WorldGen.SavedOreTiers.Copper == TileID.Tin;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return Language.GetTextValue("Mods.VanillaQoL.NPCDropConditions.TinWorld");
    }
}

public class InMultiplayer : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        return Main.netMode == NetmodeID.MultiplayerClient;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return Language.GetTextValue("Mods.VanillaQoL.NPCDropConditions.Multiplayer");
    }
}

public class StarterBagGiver : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active;
    }

    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
        // mediumcore deaths don't get a second one, obviously
        if (mediumCoreDeath || !CalamityQoLConfig.Instance.starterBag) {
            yield break;
        }

        var bag = new Item();
        bag.SetDefaults(ModContent.ItemType<StarterBag>());
        yield return bag;
    }
}
