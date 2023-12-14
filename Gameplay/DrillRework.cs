using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DrillRework : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.drillRework;
    }

    public override void SetDefaults(Item item) {
        // so, tml are not the brightest, so they placed this SetDefaults hook *after* vanilla gives the 60% boost to drills.
        // how lovely, so the boost only applies to vanilla.
        // fear not, we can just use maths to undo the change.
        var isDrill = ItemID.Sets.IsDrill[item.type] || ItemID.Sets.IsChainsaw[item.type] || item.type == ItemID.ChlorophyteJackhammer;
        if (isDrill) {
            item.useTime = (int)(item.useTime / 0.6);
            item.useAnimation = (int)(item.useAnimation / 0.6);

            // apply 25% bonus
            item.useTime = (int)(item.useTime * 0.75);
            item.useAnimation = (int)(item.useAnimation * 0.75);
            // fuck off with that MeleeNoSpeed
            item.DamageType = DamageClass.Melee;
        }

    }

    public override void SetStaticDefaults() {
        // only vanilla
        for (int i = 0; i < ItemID.Count; i++) {
            var isDrill = ItemID.Sets.IsDrill[i] || ItemID.Sets.IsChainsaw[i] || i == ItemID.ChlorophyteJackhammer;
            if (isDrill) {
                PrefixLegacy.ItemSets.SwordsHammersAxesPicks[i] = true;
            }
        }
    }
}