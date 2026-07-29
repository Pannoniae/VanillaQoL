using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Restore the shield defense stuff
 */
[JITWhenModsEnabled("CalamityMod")]
public class DefenceRestore : GlobalItem {
    private static Dictionary<int, int> bonuses = null!;
    private static Dictionary<int, int> flats = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.defenceBonuses;
    }

    public override void Load() {
        bonuses = new Dictionary<int, int> {
            { ItemID.AncientHallowedGreaves, 2 },
            { ItemID.AncientHallowedPlateMail, 3 },
            { ItemID.AnkhShield, 4 },
            { ItemID.CobaltShield, 3 },
            { ItemID.EoCShield, 1 },
            { ItemID.FrozenShield, 4 },
            { ItemID.HallowedGreaves, 2 },
            { ItemID.HallowedPlateMail, 3 },
            { ItemID.HeroShield, 5 },
            { ItemID.ObsidianShield, 4 },
            { ItemID.ObsidianSkull, 1 },
            { ItemID.OrichalcumBreastplate, 3 },
            { ItemID.OrichalcumHeadgear, 2 },
            { ItemID.OrichalcumHelmet, 3 },
            { ItemID.OrichalcumLeggings, 4 },
            { ItemID.OrichalcumMask, 3 },
            { ItemID.PaladinsShield, 2 },
            { ItemID.PalladiumBreastplate, 3 },
            { ItemID.PalladiumHeadgear, 2 },
            { ItemID.PalladiumHelmet, 3 },
            { ItemID.PalladiumLeggings, 3 },
            { ItemID.PalladiumMask, 1 },
            { ItemID.Shackle, 2 }
        };

        flats = new Dictionary<int, int> {
            { ItemID.FrozenTurtleShell, 6 },
            { ItemID.LavaSkull, 4 },
            { ItemID.MoltenSkullRose, 8 },
            { ItemID.ObsidianSkullRose, 4 }
        };
    }

    public override void SetDefaults(Item item) {
        if (bonuses.TryGetValue(item.type, out var bonus)) {
            item.defense += bonus;
        }

        if (flats.TryGetValue(item.type, out var flat)) {
            item.defense = flat;
        }
    }
}

/**
 * Fifteen vanilla magic weapons used to cost less mana than they do in vanilla. 2.2 "reverted nearly all mana cost
 * edits", which is a polite way of saying every one of those discounts is gone.
 */
[JITWhenModsEnabled("CalamityMod")]
public class ManaCostRestore : GlobalItem {
    private static Dictionary<int, int> costs = null!;
    private static int trident;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.manaDiscounts;
    }

    public override void Load() {
        trident = ItemID.UnholyTrident;

        costs = new Dictionary<int, int> {
            { ItemID.BeeGun, 4 },
            { ItemID.BlizzardStaff, 7 },
            { ItemID.BookofSkulls, 12 },
            { ItemID.BookStaff, 14 },
            { ItemID.FlowerofFire, 7 },
            { ItemID.FlowerofFrost, 7 },
            { ItemID.LaserRifle, 4 },
            { ItemID.LastPrism, 10 },
            { ItemID.MagicMissile, 10 },
            { ItemID.MedusaHead, 6 },
            { ItemID.MeteorStaff, 7 },
            { ItemID.NettleBurst, 10 },
            { ItemID.RainbowRod, 15 },
            { ItemID.SpiritFlame, 11 }
        };
    }

    public override void SetDefaults(Item item) {
        if (costs.TryGetValue(item.type, out var mana)) {
            item.mana = mana;
        }
        // the only one that was a ratio rather than a number
        else if (item.type == trident) {
            item.mana = (int)(item.mana * 0.78f);
        }
    }
}
