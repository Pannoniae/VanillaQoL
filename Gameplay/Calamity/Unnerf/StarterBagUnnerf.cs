using CalamityMod.Items.TreasureBags.MiscGrabBags;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class StarterBagUnnerf : GlobalItem {
    private static readonly (int item, int stack)[] trimmed = [
        (ItemID.Bomb, 10),
        (ItemID.MiningPotion, 1),
        (ItemID.SpelunkerPotion, 2),
        (ItemID.SwiftnessPotion, 3),
        (ItemID.GillsPotion, 2),
        (ItemID.ShinePotion, 1),
        (ItemID.Chest, 3)
    ];

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.starterBag;
    }

    // no AppliesToEntity because this runs once per item type while the loot tables are built, not per instance
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type != ModContent.ItemType<StarterBag>()) {
            return;
        }

        foreach (var (drop, stack) in trimmed) {
            itemLoot.Add(ItemDropRule.Common(drop, 1, stack, stack));
        }
    }
}
