using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

// We add a despawn to the hearts/mana crystals else they just fucking lie around forever
public class Heart : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) =>
        QoLConfig.Instance.noFullHeartPickup && item.type is ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane
            or ItemID.Star or ItemID.SoulCake or ItemID.SugarPlum;

    public override bool CanPickup(Item item, Player player) {
        if (!QoLConfig.Instance.noFullHeartPickup) {
            return true;
        }

        return item.type switch {
            ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane => player.statLife <= player.statLifeMax2 - 20,
            ItemID.Star or ItemID.SoulCake or ItemID.SugarPlum => player.statMana <= player.statManaMax2 - 100,
            // fuckup
            _ => throw new InvalidOperationException()
        };
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        //private const int healingItemsDecayRate = 4;
        // if it applies to us, despawn once we reached 5 mins =
        var age = item.timeSinceItemSpawned;
        if (age > QoLConfig.Instance.heartTime * 60 * 4) {
            item.TurnToAir(true);
            item.active = false;
        }
    }
}