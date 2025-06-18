using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

// Thank you Terraria Overhaul!
internal sealed class SoyMasterMode : ModSystem {
    private static bool isEnabled;

    public override void Load() {
        updateMasterMode();
    }

    public override void Unload() {
        // Reset everything to vanilla
        if (isEnabled) {
            updateMasterMode(false);
        }
    }

    public override void PreUpdateEntities() {
        updateMasterMode();
    }

    private static void updateMasterMode() {
        isEnabled = QoLConfig.Instance.soyMasterMode;

        updateMasterMode(isEnabled);
    }

    private static void updateMasterMode(bool shouldBeEnabled) {
        // This will unfortunately reset any changes from other mods

        Span<GameModeData> presets = [
            GameModeData.NormalMode,
            GameModeData.ExpertMode,
            GameModeData.MasterMode,
            GameModeData.CreativeMode
        ];

        if (shouldBeEnabled) {
            updateMasterMode(presets);
        }

        for (int i = 0; i < presets.Length; i++) {
            Main.RegisteredGameModes[i] = presets[i];
        }

        Main.GameMode = Main.GameMode; // Reloads some stupid cache
        isEnabled = shouldBeEnabled;
    }

    private static void updateMasterMode(Span<GameModeData> presets) {
        ref var master = ref presets[GameModeID.Master];
        master.EnemyMaxLifeMultiplier = 2f;
        master.EnemyDamageMultiplier = 2f;
        master.DebuffTimeMultiplier = 2f;
        master.KnockbackToEnemiesMultiplier = 0.9f;
        master.TownNPCDamageMultiplier = 1.5f;
    }
}

public class ExpertOnlyDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription {
    public bool CanDrop(DropAttemptInfo info) {
        if (info.npc.boss && QoLConfig.Instance.masterModeRelicsInExpert && !Main.masterMode)
            return Main.expertMode;

        return false;
    }

    public bool CanShowItemDropInUI() {
        return Main.expertMode && QoLConfig.Instance.masterModeRelicsInExpert && !Main.masterMode;
    }

    public string GetConditionDescription() {
        return Language.GetTextValue("Mods.VanillaQoL.NPCDropConditions.ExpertNotMaster");
    }
}

public class SoyMasterModeNPC : GlobalNPC {
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        int allowedRecursionDepth = 10;
        foreach (var item in npcLoot.Get()) {
            CheckMasterDropRule(item);
        }

        return;

        void AddDrop(IItemDropRule dropRule) {
            switch (npc.type) {
                case NPCID.Retinazer or NPCID.Spazmatism: {
                    var noTwin = new LeadingConditionRule(new Conditions.MissingTwin());
                    noTwin.OnSuccess(dropRule);
                    npcLoot.Add(noTwin);
                    break;
                }
                case NPCID.EaterofWorldsBody or NPCID.EaterofWorldsHead or NPCID.EaterofWorldsTail: {
                    var lastEater = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
                    lastEater.OnSuccess(dropRule);
                    npcLoot.Add(lastEater);
                    break;
                }
                default:
                    npcLoot.Add(dropRule);
                    break;
            }
        }

        void CheckMasterDropRule(IItemDropRule? dropRule) {
            if (allowedRecursionDepth-- > 0) {
                if (dropRule?.ChainedRules != null) {
                    foreach (var chain in dropRule.ChainedRules) {
                        if (chain?.RuleToChain != null) {
                            CheckMasterDropRule(chain.RuleToChain);
                        }
                    }
                }

                var dropBasedOnMasterMode =
                    (DropBasedOnMasterMode)(object)((dropRule is DropBasedOnMasterMode) ? dropRule : null);
                if (dropBasedOnMasterMode?.ruleForMasterMode != null) {
                    CheckMasterDropRule(dropBasedOnMasterMode.ruleForMasterMode);
                }
            }

            allowedRecursionDepth++;
            var itemDropWithCondition =
                (ItemDropWithConditionRule)(object)((dropRule is ItemDropWithConditionRule) ? dropRule : null);
            if (itemDropWithCondition?.condition is Conditions.IsMasterMode) {
                AddDrop(ItemDropRule.ByCondition(new ExpertOnlyDropCondition(), itemDropWithCondition.itemId,
                    itemDropWithCondition.chanceDenominator, itemDropWithCondition.amountDroppedMinimum,
                    itemDropWithCondition.amountDroppedMaximum, itemDropWithCondition.chanceNumerator));
            }
            else {
                var dropPerPlayer =
                    (DropPerPlayerOnThePlayer)(object)((dropRule is DropPerPlayerOnThePlayer) ? dropRule : null);
                if (dropPerPlayer?.condition is Conditions.IsMasterMode) {
                    AddDrop(ItemDropRule.ByCondition(new ExpertOnlyDropCondition(), dropPerPlayer.itemId,
                        dropPerPlayer.chanceDenominator, dropPerPlayer.amountDroppedMinimum,
                        dropPerPlayer.amountDroppedMaximum, dropPerPlayer.chanceNumerator));
                }
            }
        }
    }
}