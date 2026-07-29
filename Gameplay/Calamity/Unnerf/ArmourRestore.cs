using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class ArmourPieceRestore : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.armourBonuses;
    }

    public override void UpdateEquip(Item item, Player player) {
        switch (item.type) {
            // Calamity had Crimson at 6% a piece against vanilla's 3%
            case ItemID.CrimsonHelmet:
            case ItemID.CrimsonGreaves:
                player.GetDamage<GenericDamageClass>() += 0.03f;
                break;

            case ItemID.CrimsonScalemail:
                player.GetDamage<GenericDamageClass>() += 0.03f;
                player.lifeRegen += 1;
                break;

            case ItemID.PalladiumBreastplate:
                player.GetDamage<GenericDamageClass>() += 0.02f;
                break;

            case ItemID.PalladiumLeggings:
                player.GetDamage<GenericDamageClass>() += 0.03f;
                break;

            case ItemID.OrichalcumBreastplate:
                player.GetCritChance<GenericDamageClass>() += 4;
                break;

            case ItemID.CobaltHat:
            case ItemID.MythrilHood:
            case ItemID.AdamantiteHeadgear:
                player.statManaMax2 += 20;
                break;
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class ArmourSetRestore : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.armourBonuses;
    }

    public override void UpdateEquips() {
        var head = Player.armor[0].type;
        var body = Player.armor[1].type;
        var legs = Player.armor[2].type;

        if (head == ItemID.GladiatorHelmet && body == ItemID.GladiatorBreastplate &&
            legs == ItemID.GladiatorLeggings) {
            Player.statDefense += 3;
        }

        // the Hood set compensated vanilla's magic penalty by 40%, and 2.2 halved it
        if (head == ItemID.SpectreHood && body == ItemID.SpectreRobe && legs == ItemID.SpectrePants) {
            Player.GetDamage<MagicDamageClass>() += 0.2f;
        }

        if (body != ItemID.AdamantiteBreastplate || legs != ItemID.AdamantiteLeggings) {
            return;
        }

        // the stacking defense used to cap at 15 rather than 10
        var calamity = Player.GetModPlayer<CalamityPlayer>();
        Player.statDefense += calamity.AdamantiteSetDefenseBoost / 2;

        // ...and DR converted to crit at 50% rather than 25%
        var crit = (int)(MathHelper.Clamp(Player.endurance, 0f, 1f) * 25f);
        switch (head) {
            case ItemID.AdamantiteHeadgear:
                Player.GetCritChance<MagicDamageClass>() += crit;
                break;
            case ItemID.AdamantiteHelmet:
                Player.GetCritChance<MeleeDamageClass>() += crit;
                break;
            case ItemID.AdamantiteMask:
                Player.GetCritChance<RangedDamageClass>() += crit;
                break;
        }
    }
}
