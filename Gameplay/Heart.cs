using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class Heart : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) =>
        QoLConfig.Instance.noFullHealPickup && item.type is ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane
            or ItemID.Star or ItemID.SoulCake or ItemID.SugarPlum;

    public override bool CanPickup(Item item, Player player) {
        if (!QoLConfig.Instance.noFullHealPickup) {
            return true;
        }

        return item.type switch {
            ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane => player.statLife <= player.statLifeMax2 - 20,
            ItemID.Star or ItemID.SoulCake or ItemID.SugarPlum => player.statMana <= player.statManaMax2 - 100,
            // fuckup
            _ => throw new InvalidOperationException()
        };
    }
}