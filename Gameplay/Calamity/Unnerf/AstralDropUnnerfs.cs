using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Astral;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class AstralDropUnnerfs : GlobalNPC {
    private const int canonicalRate = 7;

    private static Dictionary<int, int[]> changes = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.astralDrops;
    }

    public override void SetStaticDefaults() {
        changes = new Dictionary<int, int[]> {
            [ModContent.NPCType<Aries>()] = [ModContent.ItemType<StellarKnife>()],
            [ModContent.NPCType<Astraglomerate>()] = [ModContent.ItemType<HivePod>()],
            [ModContent.NPCType<AstralSlime>()] = [ModContent.ItemType<AbandonedSlimeStaff>()],
            [ModContent.NPCType<AstralachneaGround>()] = [ModContent.ItemType<AstralachneaStaff>()],
            [ModContent.NPCType<AstralachneaWall>()] = [ModContent.ItemType<AstralachneaStaff>()],
            [ModContent.NPCType<Atlas>()] = [ModContent.ItemType<TitanArm>()],
            [ModContent.NPCType<Mantis>()] = [ModContent.ItemType<AstralScythe>()],
            [ModContent.NPCType<StellarCulex>()] = [ModContent.ItemType<StarbusterCore>()]
        };
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (!changes.TryGetValue(npc.type, out var items)) {
            return;
        }

        foreach (var item in items) {
            if (!npcLoot.Get(false).Any(rule => rule is ItemDropWithConditionRule c && c.itemId == item)) {
                VanillaQoL.instance.Logger.Warn(
                    $"No Calamity drop rule for item {item} on NPC {npc.type}!");
                continue;
            }

            npcLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule c && c.itemId == item);
            npcLoot.Add(ItemDropRule.ByCondition(
                new Progress(() => DownedBossSystem.downedAstrumAureus,
                    Language.GetTextValue("Mods.VanillaQoL.NPCDropConditions.PostAstrumAureus")), item,
                canonicalRate));
        }
    }
}
