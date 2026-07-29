using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class ExpertDropUnnerfs : GlobalNPC {
    private static readonly Dictionary<int, int[]> normalDrops = new() {
        [NPCID.KingSlime] = [ItemID.RoyalGel],
        [NPCID.EyeofCthulhu] = [ItemID.EoCShield],
        [NPCID.BrainofCthulhu] = [ItemID.BrainOfConfusion],
        [NPCID.QueenBee] = [ItemID.HiveBackpack],
        [NPCID.SkeletronHead] = [ItemID.BoneGlove],
        [NPCID.Deerclops] = [ItemID.BoneHelm],
        [NPCID.QueenSlimeBoss] = [ItemID.VolatileGelatin],
        [NPCID.TheDestroyer] = [ItemID.MechanicalWagonPiece],
        [NPCID.Spazmatism] = [ItemID.MechanicalWheelPiece],
        [NPCID.SkeletronPrime] = [ItemID.MechanicalBatteryPiece],
        [NPCID.Plantera] = [ItemID.SporeSac],
        [NPCID.Golem] = [ItemID.ShinyStone],
        [NPCID.DukeFishron] = [ItemID.ShrimpyTruffle],
        [NPCID.HallowBoss] = [ItemID.EmpressFlightBooster],
        [NPCID.MoonLordCore] = [
            ItemID.GravityGlobe, ItemID.SuspiciousLookingTentacle, ItemID.LongRainbowTrailWings
        ]
    };

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.expertDrops;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.type == NPCID.EaterofWorldsTail) {
            var last = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
            last.OnSuccess(perPlayer(ItemID.WormScarf));
            npcLoot.Add(last);
            return;
        }

        if (npc.type == NPCID.MourningWood) {
            var pumpkinMoon = new LeadingConditionRule(new Conditions.PumpkinMoonDropGatingChance());
            pumpkinMoon.OnSuccess(ItemDropRule.ByCondition(new Conditions.NotExpert(), ItemID.WitchBroom, 5));
            npcLoot.Add(pumpkinMoon);
            return;
        }

        if (!normalDrops.TryGetValue(npc.type, out var items)) {
            return;
        }

        foreach (var item in items) {
            npcLoot.Add(perPlayer(item));
        }
    }

    /// one each, everybody gets one, Normal only - Expert still has the items in the treasure bag
    private static IItemDropRule perPlayer(int item) {
        return new DropPerPlayerOnThePlayer(item, 1, 1, 1, new Conditions.NotExpert());
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class ExpertFlagUnnerf : GlobalItem {
    private static HashSet<int> notExpert = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.expertDrops;
    }

    public override void Load() {
        notExpert = [
            ItemID.RoyalGel, ItemID.EoCShield, ItemID.WormScarf, ItemID.BrainOfConfusion, ItemID.HiveBackpack,
            ItemID.BoneHelm, ItemID.BoneGlove, ItemID.VolatileGelatin, ItemID.MechanicalBatteryPiece,
            ItemID.MechanicalWagonPiece, ItemID.MechanicalWheelPiece, ItemID.SporeSac, ItemID.WitchBroom,
            ItemID.EmpressFlightBooster, ItemID.ShinyStone, ItemID.ShrimpyTruffle, ItemID.GravityGlobe,
            ItemID.SuspiciousLookingTentacle, ItemID.LongRainbowTrailWings
        ];
    }

    public override void SetDefaults(Item item) {
        if (notExpert.Contains(item.type)) {
            item.expert = false;
        }
    }
}
