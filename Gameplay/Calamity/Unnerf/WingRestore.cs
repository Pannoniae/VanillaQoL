using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Accessories.Wings;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class WingRestore : GlobalItem {
    private static int elysian;
    private static HashSet<int> fallSafe = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.wingBonuses;
    }

    public override void SetStaticDefaults() {
        elysian = ModContent.ItemType<ElysianWings>();

        fallSafe = [
            ItemID.AngelWings, ItemID.DemonWings, ItemID.BeeWings, ItemID.ButterflyWings, ItemID.FairyWings,
            ItemID.BatWings, ItemID.HarpyWings, ItemID.MothronWings, ItemID.FrozenWings, ItemID.FlameWings,
            ItemID.GhostWings, ItemID.BeetleWings, ItemID.Hoverboard, ItemID.LeafWings, ItemID.FestiveWings,
            ItemID.SpookyWings, ItemID.TatteredFairyWings, ItemID.SteampunkWings, ItemID.WingsSolar,
            ItemID.WingsVortex, ItemID.WingsNebula, ItemID.WingsStardust, ItemID.FishronWings, ItemID.BetsyWings,
            ItemID.Yoraiz0rWings, ItemID.JimsWings, ItemID.SkiphsWings, ItemID.LokisWings, ItemID.ArkhalisWings,
            ItemID.LeinforsWings, ItemID.BejeweledValkyrieWing, ItemID.RedsWings, ItemID.DTownsWings,
            ItemID.WillsWings, ItemID.CrownosWings, ItemID.CenxsWings, ItemID.CreativeWings,
            ItemID.FoodBarbarianWings, ItemID.GroxTheGreatWings, ItemID.GhostarsWings, ItemID.SafemanWings,
            ItemID.RainbowWings, ItemID.LongRainbowTrailWings
        ];
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        if (item.type == elysian) {
            player.lavaImmune = true;
            player.moveSpeed += 0.1f;
            player.noFallDmg = true;
            return;
        }

        switch (item.type) {
            case ItemID.AngelWings:
                player.statLifeMax2 += 20;
                player.statDefense += 10;
                player.lifeRegen += 2;
                break;

            case ItemID.DemonWings:
                player.GetDamage<GenericDamageClass>() += 0.05f;
                player.GetCritChance<GenericDamageClass>() += 5;
                break;

            case ItemID.FinWings:
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) {
                    player.gills = true;
                    if (!player.mount.Active) {
                        player.maxFallSpeed = 12f;
                    }
                }

                player.ignoreWater = true;
                break;

            case ItemID.BeeWings:
                player.AddBuff(BuffID.Honey, 2);
                break;

            case ItemID.ButterflyWings:
                player.statManaMax2 += 20;
                player.GetDamage<MagicDamageClass>() += 0.05f;
                player.manaCost *= 0.95f;
                player.GetCritChance<MagicDamageClass>() += 5;
                break;

            case ItemID.FairyWings:
                player.statLifeMax2 += 60;
                break;

            case ItemID.BatWings:
                if (!Main.dayTime || Main.eclipse) {
                    player.GetDamage<GenericDamageClass>() += 0.07f;
                    player.GetCritChance<GenericDamageClass>() += 3;
                }

                break;

            case ItemID.HarpyWings:
                player.moveSpeed += 0.2f;
                break;

            case ItemID.MothronWings:
                player.statDefense += 5;
                player.GetDamage<GenericDamageClass>() += 0.05f;
                break;

            case ItemID.FrozenWings:
                if (player.head == ArmorIDs.Head.FrostHelmet && player.body == ArmorIDs.Body.FrostBreastplate &&
                    player.legs == ArmorIDs.Legs.FrostLeggings) {
                    player.GetDamage<MeleeDamageClass>() += 0.02f;
                    player.GetDamage<RangedDamageClass>() += 0.02f;
                    player.GetCritChance<MeleeDamageClass>() += 1;
                    player.GetCritChance<RangedDamageClass>() += 1;
                }

                break;

            case ItemID.FlameWings:
                player.GetDamage<MeleeDamageClass>() += 0.05f;
                player.GetCritChance<MeleeDamageClass>() += 5;
                break;

            case ItemID.GhostWings:
                if (player.body == ArmorIDs.Body.SpectreRobe && player.legs == ArmorIDs.Legs.SpectrePants) {
                    if (player.head == ArmorIDs.Head.SpectreHood) {
                        player.statDefense += 10;
                        player.endurance += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.SpectreMask) {
                        player.GetDamage<MagicDamageClass>() += 0.05f;
                        player.GetCritChance<MagicDamageClass>() += 5;
                    }
                }

                break;

            case ItemID.BeetleWings:
                if (player.head == ArmorIDs.Head.BeetleHelmet && player.legs == ArmorIDs.Legs.BeetleLeggings) {
                    if (player.body == ArmorIDs.Body.BeetleShell) {
                        player.statDefense += 10;
                        player.endurance += 0.05f;
                    }
                    else if (player.body == ArmorIDs.Body.BeetleScaleMail) {
                        player.GetDamage<MeleeDamageClass>() += 0.05f;
                        player.GetCritChance<MeleeDamageClass>() += 5;
                    }
                }

                break;

            case ItemID.Hoverboard:
                if (player.body == ArmorIDs.Body.ShroomiteBreastplate &&
                    player.legs == ArmorIDs.Legs.ShroomiteLeggings) {
                    if (player.head == ArmorIDs.Head.ShroomiteHeadgear) {
                        player.arrowDamage += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.ShroomiteMask) {
                        player.bulletDamage += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.ShroomiteHelmet) {
                        player.specialistDamage += 0.05f;
                    }
                }

                break;

            case ItemID.LeafWings:
                if (player.head == ArmorIDs.Head.TikiMask && player.body == ArmorIDs.Body.TikiShirt &&
                    player.legs == ArmorIDs.Legs.TikiPants) {
                    player.statDefense += 5;
                    player.endurance += 0.05f;
                    player.AddBuff(BuffID.DryadsWard, 5, true);
                }

                break;

            case ItemID.FestiveWings:
                player.statLifeMax2 += 40;
                dropOrnament(item, player);
                break;

            case ItemID.SpookyWings:
                if (player.head == ArmorIDs.Head.SpookyHelmet && player.body == ArmorIDs.Body.SpookyBreastplate &&
                    player.legs == ArmorIDs.Legs.SpookyLeggings) {
                    player.GetKnockback(DamageClass.Summon) += 2f;
                    player.GetDamage<SummonDamageClass>() += 0.05f;
                }

                break;

            case ItemID.TatteredFairyWings:
                player.GetDamage<GenericDamageClass>() += 0.05f;
                player.GetCritChance<GenericDamageClass>() += 5;
                break;

            case ItemID.SteampunkWings:
                player.statDefense += 8;
                player.GetDamage<GenericDamageClass>() += 0.04f;
                player.GetCritChance<GenericDamageClass>() += 2;
                player.moveSpeed += 0.1f;
                break;

            case ItemID.WingsSolar:
                if (player.head == ArmorIDs.Head.SolarFlareHelmet &&
                    player.body == ArmorIDs.Body.SolarFlareBreastplate &&
                    player.legs == ArmorIDs.Legs.SolarFlareLeggings) {
                    player.GetDamage<MeleeDamageClass>() += 0.07f;
                    player.GetCritChance<MeleeDamageClass>() += 3;
                }

                break;

            case ItemID.WingsVortex:
                if (player.head == ArmorIDs.Head.VortexHelmet && player.body == ArmorIDs.Body.VortexBreastplate &&
                    player.legs == ArmorIDs.Legs.VortexLeggings) {
                    player.GetDamage<RangedDamageClass>() += 0.03f;
                    player.GetCritChance<RangedDamageClass>() += 7;
                }

                break;

            case ItemID.WingsNebula:
                if (player.head == ArmorIDs.Head.NebulaHelmet && player.body == ArmorIDs.Body.NebulaBreastplate &&
                    player.legs == ArmorIDs.Legs.NebulaLeggings) {
                    player.GetDamage<MagicDamageClass>() += 0.05f;
                    player.GetCritChance<MagicDamageClass>() += 5;
                    player.statManaMax2 += 20;
                    player.manaCost *= 0.95f;
                }

                break;

            case ItemID.WingsStardust:
                if (player.head == ArmorIDs.Head.StardustHelmet && player.body == ArmorIDs.Body.StardustPlate &&
                    player.legs == ArmorIDs.Legs.StardustLeggings) {
                    player.GetDamage<SummonDamageClass>() += 0.1f;
                }

                break;
        }

        if (fallSafe.Contains(item.type)) {
            player.noFallDmg = true;
        }
    }

    private static void dropOrnament(Item item, Player player) {
        var modPlayer = player.GetModPlayer<WingRestorePlayer>();
        if (modPlayer.icicleCooldown > 0 || player.whoAmI != Main.myPlayer) {
            return;
        }

        if (!player.controlJump || player.jump != 0 || player.velocity.Y == 0f || player.mount.Active ||
            player.mount.Cart) {
            return;
        }

        var damage = (int)player.GetBestClassDamage().ApplyTo(50);
        var p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.UnitY * 2f,
            ProjectileID.OrnamentFriendly, damage, 5f, player.whoAmI);

        if (p >= 0 && p < Main.maxProjectiles) {
            Main.projectile[p].DamageType = DamageClass.Generic;
            modPlayer.icicleCooldown = 15;
        }
    }
}

/// Calamity deleted the field this used to be on, so we have our own....
[JITWhenModsEnabled("CalamityMod")]
public class WingRestorePlayer : ModPlayer {
    public int icicleCooldown;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.wingBonuses;
    }

    public override void PostUpdateEquips() {
        if (icicleCooldown > 0) {
            icicleCooldown--;
        }
    }
}
