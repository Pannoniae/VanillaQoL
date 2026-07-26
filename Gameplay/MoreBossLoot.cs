using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class MoreBossLoot : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        if (VanillaQoL.isCalamityLoaded()) {
            VanillaQoL.instance.Logger.Info(
                "Refusing to load More Boss Loot because Calamity overwrote the Queen Bee/Plantera/Golem bag droprules...");
            return false;
        }

        return true;
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (!QoLConfig.Instance.moreBossLoot) {
            return;
        }

        if (item.type is ItemID.QueenBeeBossBag or ItemID.PlanteraBossBag or ItemID.GolemBossBag) {
            var rules = itemLoot.Get();

            switch (item.type) {
                case ItemID.QueenBeeBossBag:
                    var queenBee = (OneFromOptionsNotScaledWithLuckDropRule?)rules.FirstOrDefault(x =>
                        x is OneFromOptionsNotScaledWithLuckDropRule r && r.dropIds.Contains(ItemID.BeeKeeper));

                    if (queenBee is null) {
                        ruleGone("Queen Bee");
                        break;
                    }

                    var originalBeeDrops = queenBee.dropIds ?? [];

                    if (originalBeeDrops.Length > 0) {
                        var newQueenBee = new FewFromOptionsNotScaledWithLuckDropRule(2, 1, 1, originalBeeDrops);
                        itemLoot.Remove(queenBee);
                        itemLoot.Add(newQueenBee);
                    }

                    break;
                case ItemID.PlanteraBossBag:
                    var plantera = (OneFromRulesRule?)rules.FirstOrDefault(x =>
                        x is OneFromRulesRule r &&
                        r.options.Any(c => c is CommonDrop cd && cd.itemId == ItemID.Seedler));

                    if (plantera is null) {
                        ruleGone("Plantera");
                        break;
                    }

                    var originalPlantDrops = plantera.options ?? [];

                    if (originalPlantDrops.Length > 0) {
                        var newPlantera = new FewFromRulesRule(2, 1, originalPlantDrops);
                        itemLoot.Remove(plantera);
                        itemLoot.Add(newPlantera);
                    }

                    break;
                case ItemID.GolemBossBag:
                    var golem = (OneFromRulesRule?)rules.FirstOrDefault(x =>
                        x is OneFromRulesRule r &&
                        r.options.Any(c => c is CommonDrop cd && cd.itemId == ItemID.SunStone));

                    if (golem is null) {
                        ruleGone("Golem");
                        break;
                    }

                    var originalGolemDrops = golem.options ?? [];

                    if (originalGolemDrops.Length > 0) {
                        var newGolem = new FewFromRulesRule(2, 1, originalGolemDrops);
                        itemLoot.Remove(golem);
                        itemLoot.Add(newGolem);
                    }

                    break;
            }
        }
    }

    private static void ruleGone(string boss) {
        VanillaQoL.instance.Logger.Warn(
            $"More Boss Loot: no vanilla weapon rule in the {boss} bag, either the vanilla drop rules changed or another mod replaced them?");
    }
}