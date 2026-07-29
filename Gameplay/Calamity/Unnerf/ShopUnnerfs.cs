using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Condition = Terraria.Condition;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Restore Calamity shop items that were removed.
 * In a cruel twist of irony, I've actually implemented the *rest* of it way earlier in CalamityQOL for Vanilla, so *that* config controls the *old* removals (mostly potions)
 * but the MFs removed EVEN more stuff. So here THAT is. I might need to move the other config here too...
 *
 * Also missing the shameless self-insert / Cirrus / Drunk Princess / alcoholic stuff but that might be a bit too controversial to add back so I'll skip
 */
[JITWhenModsEnabled("CalamityMod")]
public class ShopUnnerfs : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.shopStock;
    }

    public override void ModifyShop(NPCShop shop) {
        var gold = Condition.HappyEnough;
        var skeletron = Condition.DownedSkeletron;
        var golem = Condition.DownedGolem;

        switch (shop.NpcType) {
            case NPCID.Merchant:
                sell(shop, ItemID.Bottle, Item.buyPrice(copper: 20), gold);
                sell(shop, ItemID.Burger, Item.buyPrice(gold: 5), gold, skeletron);
                sell(shop, ItemID.Hotdog, Item.buyPrice(gold: 5), gold, skeletron);
                sell(shop, ItemID.CoffeeCup, Item.buyPrice(gold: 2), gold);
                break;

            case NPCID.ArmsDealer:
                sell(shop, ItemID.TacticalShotgun, Item.buyPrice(gold: 60), golem);
                sell(shop, ItemID.SniperRifle, Item.buyPrice(gold: 60), golem);
                sell(shop, ItemID.RifleScope, Item.buyPrice(gold: 60), golem);
                break;

            case NPCID.Cyborg:
                sell(shop, ItemID.RocketLauncher, Item.buyPrice(gold: 25), golem);
                break;

            case NPCID.Wizard:
                sell(shop, ItemID.RodofDiscord, Item.buyPrice(gold: 50), Condition.Hardmode, Condition.InHallow);
                sell(shop, ItemID.SpectreStaff, Item.buyPrice(gold: 25), golem);
                sell(shop, ItemID.InfernoFork, Item.buyPrice(gold: 25), golem);
                sell(shop, ItemID.ShadowbeamStaff, Item.buyPrice(gold: 25), golem);
                sell(shop, ItemID.MagnetSphere, Item.buyPrice(gold: 25), golem);
                break;

            case NPCID.Dryad:
                sell(shop, ItemID.Grapes, Item.buyPrice(gold: 2, silver: 50), gold, skeletron);
                break;

            case NPCID.GoblinTinkerer:
                sell(shop, ItemID.StinkPotion, Item.buyPrice(silver: 25), gold);
                sell(shop, ItemID.Spaghetti, Item.buyPrice(gold: 5), gold, skeletron);
                break;

            case NPCID.WitchDoctor:
                sell(shop, ItemID.ButterflyDust, Item.buyPrice(gold: 10), golem);
                sell(shop, ItemID.FriedEgg, Item.buyPrice(gold: 2, silver: 50), gold);
                break;

            case NPCID.PartyGirl:
                sell(shop, ItemID.Pizza, Item.buyPrice(gold: 5), gold, skeletron);
                sell(shop, ItemID.CreamSoda, Item.buyPrice(gold: 2, silver: 50), gold);
                break;

            case NPCID.SkeletonMerchant:
                shop.Add(ItemID.MilkCarton);
                break;

            case NPCID.Golfer:
                sell(shop, ItemID.PotatoChips, Item.buyPrice(gold: 1), gold);
                break;

            case NPCID.BestiaryGirl:
                sell(shop, ItemID.Steak, Item.buyPrice(gold: 5), gold, Condition.Hardmode);
                break;
        }
    }

    private static void sell(NPCShop shop, int item, int price, params Condition[] conditions) {
        shop.Add(new Item(item) { shopCustomPrice = price }, conditions);
    }
}
