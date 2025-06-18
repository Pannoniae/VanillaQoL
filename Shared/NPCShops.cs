using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// ReSharper disable All

namespace VanillaQoL.Shared;

public class NPCShops : GlobalNPC {
    public override void ModifyShop(NPCShop shop) {
        if (!GlobalFeatures.NPCShops) {
            return;
        }

        // 2 gold at start, 4 in hardmode and 8 post-ML
        int cost = 2;
        if (Main.hardMode) {
            cost *= 2;
        }

        if (Terraria.NPC.downedMoonlord) {
            cost *= 2;
        }

        switch (shop.NpcType) {
            case NPCID.Merchant:
                addToShop(shop, ItemID.Bottle,
                    Item.buyPrice(0, 0, 0, 20));
                addToShop(shop, ItemID.WormholePotion,
                    Item.buyPrice(0, 0, 25, 0));
                addToShop(shop, ItemID.ArcheryPotion, Condition.DownedEyeOfCthulhu,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.TitanPotion, Condition.DownedSkeletron,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.ApprenticeBait, Condition.DownedEyeOfCthulhu);
                addToShop(shop, ItemID.JourneymanBait, Condition.DownedSkeletron);
                // haha funny
                addToShop(shop, ItemID.MasterBait, Condition.DownedPlantera);
                addToShop(shop, ItemID.Burger, Item.buyPrice(0, 5, 0, 0),
                    () => Terraria.NPC.downedBoss3);

                addToShop(shop, ItemID.Hotdog, Condition.Hardmode,
                    Item.buyPrice(0, 5, 0, 0));
                addToShop(shop, ItemID.CoffeeCup, Item.buyPrice(0, 2, 0, 0));
                break;
            case NPCID.Demolitionist:
                addToShop(shop, ItemID.MiningPotion, Condition.DownedEyeOfCthulhu,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.IronskinPotion, Condition.DownedEyeOfCthulhu,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.ShinePotion, Condition.DownedEyeOfCthulhu,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.SpelunkerPotion, Condition.DownedEowOrBoc,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.ObsidianSkinPotion, Condition.DownedSkeletron,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.EndurancePotion, Condition.DownedSkeletron,
                    Item.buyPrice(0, cost, 0, 0));
                break;
            case NPCID.DyeTrader:
                addToShop(shop, ItemID.DyeTradersScimitar, price: Item.buyPrice(0, 15, 0, 0));
                break;
            case NPCID.ArmsDealer:
                addToShop(shop, ItemID.Boomstick, Condition.DownedQueenBee,
                    Item.buyPrice(0, 20, 0, 0));
                addToShop(shop, ItemID.AmmoBox, Condition.Hardmode,
                    Item.buyPrice(0, 25, 0, 0));
                addToShop(shop, ItemID.Uzi, Condition.DownedPlantera,
                    Item.buyPrice(0, 45, 0, 0));
                addToShop(shop, ItemID.TacticalShotgun, Condition.DownedGolem,
                    Item.buyPrice(0, 60, 0, 0));
                addToShop(shop, ItemID.SniperRifle, Condition.DownedGolem,
                    Item.buyPrice(0, 60, 0, 0));
                addToShop(shop, ItemID.RifleScope, Condition.DownedGolem,
                    Item.buyPrice(0, 60, 0, 0));
                addToShop(shop, ItemID.AmmoReservationPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.HunterPotion,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.BattlePotion, Condition.DownedEowOrBoc,
                    Item.buyPrice(0, 2, 0, 0));
                break;
            case NPCID.Stylist:
                addToShop(shop, ItemID.ChocolateChipCookie,
                    Item.buyPrice(0, 3, 0, 0));
                break;
            case NPCID.Cyborg:
                addToShop(shop, ItemID.RocketLauncher, Condition.DownedGolem,
                    Item.buyPrice(0, 25, 0, 0));
                break;
            case NPCID.Dryad:
                addToShop(shop, ItemID.ThornsPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.FeatherfallPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.RegenerationPotion, Condition.DownedEowOrBoc,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.SwiftnessPotion, Condition.DownedEowOrBoc,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.JungleRose, Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.NaturesGift, Item.buyPrice(0, 10, 0, 0));
                addToShop(shop, ItemID.Grapes, Condition.DownedSkeletron,
                    Item.buyPrice(0, 2, 50, 0));
                break;
            case NPCID.GoblinTinkerer:
                addToShop(shop, ItemID.StinkPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.Spaghetti, Condition.DownedSkeletron,
                    Item.buyPrice(0, 5, 0, 0));
                break;
            case NPCID.Mechanic:
                addToShop(shop, ItemID.BuilderPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.CombatWrench, Item.buyPrice(0, 10, 0, 0));
                break;
            case NPCID.Clothier:
                addToShop(shop, ItemID.GoldenKey, Condition.Hardmode, Item.buyPrice(0, 5, 0, 0));
                break;
            case NPCID.Painter:
                addToShop(shop, ItemID.PainterPaintballGun, price: Item.buyPrice(0, 15, 0, 0));
                break;
            case NPCID.Steampunker:
                // TODO: add proper conditions and lockeys for this
                addToShop(shop, ItemID.PurpleSolution,
                    Item.buyPrice(0, 0, 5, 0), () => Main.LocalPlayer.ZoneGraveyard && WorldGen.crimson);
                addToShop(shop, ItemID.RedSolution,
                    Item.buyPrice(0, 0, 5, 0), () => Main.LocalPlayer.ZoneGraveyard && !WorldGen.crimson);
                break;
            case NPCID.Wizard:
                addToShop(shop, ItemID.ManaRegenerationPotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.MagicPowerPotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.LifeforcePotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.GravitationPotion,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.PotionOfReturn,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.MagicMissile, Item.buyPrice(0, 5, 0, 0));
                addToShop(shop, ItemID.RodofDiscord, Condition.InHallow,
                    Item.buyPrice(10, 0, 0, 0));
                addToShop(shop, ItemID.SpectreStaff, Condition.DownedGolem,
                    Item.buyPrice(0, 25, 0, 0));
                addToShop(shop, ItemID.InfernoFork, Condition.DownedGolem,
                    Item.buyPrice(0, 25, 0, 0));
                addToShop(shop, ItemID.ShadowbeamStaff, Condition.DownedGolem,
                    Item.buyPrice(0, 25, 0, 0));
                addToShop(shop, ItemID.MagnetSphere, Condition.DownedGolem,
                    Item.buyPrice(0, 25, 0, 0));
                break;
            case NPCID.WitchDoctor:
                addToShop(shop, ItemID.SummoningPotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.CalmingPotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.RagePotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.WrathPotion,
                    Item.buyPrice(0, cost, 0, 0));
                addToShop(shop, ItemID.InfernoPotion,
                    Item.buyPrice(0, 2, 0, 0));
                addToShop(shop, ItemID.ButterflyDust, Condition.DownedGolem,
                    Item.buyPrice(0, 10, 0, 0));
                addToShop(shop, ItemID.FriedEgg, Item.buyPrice(0, 2, 50, 0));
                break;
            case NPCID.PartyGirl:
                addToShop(shop, ItemID.GenderChangePotion,
                    Item.buyPrice(0, 1, 0, 0));
                addToShop(shop, ItemID.Pizza, Condition.DownedSkeletron,
                    Item.buyPrice(0, 5, 0, 0));
                addToShop(shop, ItemID.CreamSoda, Item.buyPrice(0, 2, 50, 0));
                break;
            case NPCID.Princess:
                addToShop(shop, ItemID.PrincessWeapon, Item.buyPrice(0, 50, 0, 0));
                addToShop(shop, ItemID.AppleJuice);
                addToShop(shop, ItemID.FruitJuice);
                addToShop(shop, ItemID.Lemonade);
                addToShop(shop, ItemID.PrismaticPunch);
                addToShop(shop, ItemID.SmoothieofDarkness);
                addToShop(shop, ItemID.TropicalSmoothie);
                break;
            case NPCID.SkeletonMerchant:
                addToShop(shop, ItemID.MilkCarton);
                addToShop(shop, ItemID.Marrow, Item.buyPrice(0, 36, 0, 0),
                    () => Main.hardMode);
                break;
            case NPCID.Golfer:
                addToShop(shop, ItemID.PotatoChips, Item.buyPrice(0, 1, 0, 0));
                break;
            // Zoologist
            case NPCID.BestiaryGirl:
                addToShop(shop, ItemID.Steak, Condition.Hardmode, Item.buyPrice(0, 5, 0, 0));
                break;
        }
    }

    /// <summary>
    /// Compatibilty wrapper for adding items to the shop.
    /// </summary>
    /// <param name="shop">The shop to add to.</param>
    /// <param name="itemID">Which (vanilla) itemID to add.</param>
    /// <param name="price">What the sell price should be.</param>
    /// <param name="cond">The condition to be registered for selling.</param>
    /// <param name="description">The description to use for the condition.</param>
    private void addToShop(NPCShop shop, int itemID, int? price = null, Func<bool>? cond = null,
        LocalizedText? description = null) {
        if (alreadyHasEntry(shop, itemID)) {
            return;
        }

        description ??= LocalizedText.Empty;

        var shopItem = new Item(itemID) {
            shopCustomPrice = price
        };
        if (cond != null) {
            shop.Add(shopItem, new Condition(description, cond));
        }
        else {
            shop.Add(shopItem);
        }
    }

    private static bool alreadyHasEntry(NPCShop shop, int item) {
        return shop.ActiveEntries.Any(entry => entry.Item.type == item);
    }

    private void addToShop(NPCShop shop, int itemID, Condition cond, int? price = null) {
        if (alreadyHasEntry(shop, itemID)) {
            return;
        }

        var shopItem = new Item(itemID) {
            shopCustomPrice = price
        };
        shop.Add(shopItem, cond);
    }

    public static void removeBiomeRequirements(string shopName, AbstractNPCShop shop, NPC? npc) {
        // nope out if not enabled
        if (!QoLConfig.Instance.removeBiomeRequirementsShops) {
            return;
        }

        var toRemove = new Dictionary<AbstractNPCShop.Entry, List<Condition>>();
        

        // get biome conditions
        foreach (var entry in shop.ActiveEntries) {
            toRemove[entry] = new List<Condition>();
            foreach (var condition in entry.Conditions) {
                // we actually don't know! but we can hack a bit.
                Item obj = entry.Item;
                if (!(TileID.Sets.CountsAsPylon).Contains<int>(obj.createTile)) {
                    if (Constants.biomeConditions.Contains(condition)) {
                        toRemove[entry].Add(condition);
                    }
                }
            }
            
            // actually remove the conditions!
            if (shop is TravellingMerchantShop ts) {
                // get conditions list
                var conditionsList = (List<Condition>)typeof(TravellingMerchantShop.Entry).GetField("<Conditions>k__BackingField",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(entry)!;
                // remove the conditions
                foreach (var condition in toRemove[entry]) {
                    conditionsList.Remove(condition);
                }
                
            }
            else if (shop is NPCShop ns) {
                // get conditions list
                // private readonly List<Condition> conditions;
                var conditionsList = (List<Condition>)typeof(NPCShop.Entry).GetField("conditions",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(entry as NPCShop.Entry)!;
                // remove the conditions
                foreach (var condition in toRemove[entry]) {
                    conditionsList.Remove(condition);
                }
                
            }
            else {
                throw new NotSupportedException(
                    $"Shop type {shop.GetType()} is not supported for removing biome requirements. WTF has happened?");
            }
        }
    }
}