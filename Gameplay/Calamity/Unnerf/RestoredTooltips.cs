using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Potions.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class RestoredTooltips : GlobalItem {
    private static CalamityUnnerfConfig cfg => CalamityUnnerfConfig.Instance;

    private static Dictionary<int, string> wingBonuses = null!;
    private static HashSet<int> fireproof = null!;
    private static HashSet<int> eventSummons = null!;
    private static int elysian;
    private static int bloodflareCore;
    private static int moonshine;

    private static readonly string ichorLine = "CalamityMod:AltExpandTooltip" + BuffID.Ichor;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.wingBonuses || cfg.fireImmunity || cfg.armourBonuses ||
                                                 cfg.prefixScaling || cfg.summonSpeed || cfg.scopes ||
                                                 cfg.meleeSpeedStacking || cfg.buffs || cfg.defenseDamage ||
                                                 cfg.summonerCrossClass || cfg.ichorDefence);
    }

    public override void SetStaticDefaults() {
        elysian = ModContent.ItemType<ElysianWings>();
        bloodflareCore = ModContent.ItemType<BloodflareCore>();
        moonshine = ModContent.ItemType<Moonshine>();

        wingBonuses = new Dictionary<int, string> {
            { ItemID.AngelWings, "AngelWings" },
            { ItemID.DemonWings, "DemonWings" },
            { ItemID.ButterflyWings, "ButterflyWings" },
            { ItemID.FairyWings, "FairyWings" },
            { ItemID.BeeWings, "BeeWings" },
            { ItemID.HarpyWings, "HarpyWings" },
            { ItemID.FlameWings, "FlameWings" },
            { ItemID.FrozenWings, "FrozenWings" },
            { ItemID.GhostWings, "GhostWings" },
            { ItemID.BeetleWings, "BeetleWings" },
            { ItemID.FinWings, "FinWings" },
            { ItemID.SteampunkWings, "SteampunkWings" },
            { ItemID.LeafWings, "LeafWings" },
            { ItemID.BatWings, "BatWings" },
            { ItemID.TatteredFairyWings, "TatteredFairyWings" },
            { ItemID.SpookyWings, "SpookyWings" },
            { ItemID.Hoverboard, "Hoverboard" },
            { ItemID.FestiveWings, "FestiveWings" },
            { ItemID.MothronWings, "MothronWings" },
            { ItemID.WingsSolar, "WingsSolar" },
            { ItemID.WingsStardust, "WingsStardust" },
            { ItemID.WingsVortex, "WingsVortex" },
            { ItemID.WingsNebula, "WingsNebula" }
        };

        fireproof = [
            ItemID.ObsidianSkull, ItemID.ObsidianHorseshoe, ItemID.ObsidianShield,
            ItemID.ObsidianWaterWalkingBoots, ItemID.LavaWaders, ItemID.ObsidianSkullRose, ItemID.MoltenCharm,
            ItemID.LavaSkull, ItemID.MoltenSkullRose, ItemID.AnkhShield, ItemID.TerrasparkBoots
        ];

        eventSummons = [
            ItemID.BloodMoonStarter, ItemID.GoblinBattleStandard, ItemID.NaughtyPresent, ItemID.PirateMap,
            ItemID.PumpkinMoonMedallion, ItemID.SnowGlobe, ItemID.SolarTablet
        ];
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (cfg.wingBonuses) {
            if (item.type == elysian) {
                append(tooltips, text("elysianLava"));
            }
            else if (wingBonuses.TryGetValue(item.type, out var key)) {
                append(tooltips, text($"Wings.{key}"));
            }
        }

        if (cfg.fireImmunity && fireproof.Contains(item.type)) {
            append(tooltips, text("onFire"));
        }

        if (cfg.summonSpeed && eventSummons.Contains(item.type)) {
            append(tooltips, text("notConsumable"));
        }

        if (cfg.armourBonuses) {
            armour(item, tooltips);
        }

        if (cfg.prefixScaling) {
            prefix(item, tooltips);
        }

        if (cfg.scopes && item.type == ItemID.SniperScope) {
            append(tooltips, text("critDamage"));
        }

        if (cfg.meleeSpeedStacking && item.type is ItemID.PowerGlove or ItemID.BerserkerGlove
                or ItemID.MechanicalGlove or ItemID.FireGauntlet) {
            append(tooltips, text("trueMelee"));
        }

        if (cfg.buffs && item.type is ItemID.Ale or ItemID.Sake) {
            append(tooltips, text("alcohol"));
        }

        if (cfg.defenseDamage) {
            noDefenceDamage(item, tooltips);
        }

        if (cfg.summonerCrossClass && item.type is ItemID.AncientBattleArmorHat
                or ItemID.AncientBattleArmorShirt or ItemID.AncientBattleArmorPants) {
            removeLastLine(tooltips.Find(l => l.Name == "SetBonus"));
        }

        if (cfg.ichorDefence) {
            fixIchor(tooltips);
        }
    }

    private static void noDefenceDamage(Item item, List<TooltipLine> tooltips) {
        if (item.type is >= ItemID.AdamantiteHeadgear and <= ItemID.AdamantiteLeggings) {
            removeLastLine(tooltips.Find(l => l.Name == "SetBonus"));
            return;
        }

        if (item.type == bloodflareCore || item.type == moonshine) {
            append(tooltips, text("noDefenceDamage"));
        }
    }

    private static void removeLastLine(TooltipLine? line) {
        if (line == null) {
            return;
        }

        var cut = line.Text.LastIndexOf('\n');
        if (cut > 0) {
            line.Text = line.Text[..cut];
        }
    }

    private static void fixIchor(List<TooltipLine> tooltips) {
        var line = tooltips.Find(l => l.Name == ichorLine);
        if (line == null) {
            return;
        }

        var calamity = Language.GetTextValue("Mods.Terraria.Buffs.Ichor.ItemTooltipEnemy");
        line.Text = line.Text.Replace(calamity, calamity.Replace("10", "15"));
    }

    private static void armour(Item item, List<TooltipLine> tooltips) {
        switch (item.type) {
            case ItemID.CrimsonHelmet:
            case ItemID.CrimsonGreaves:
                replaceAdded(tooltips, text("crimsonPiece"));
                return;
            case ItemID.CrimsonScalemail:
                replaceAdded(tooltips, text("crimsonChest"));
                return;

            case ItemID.PalladiumBreastplate:
                append(tooltips, text("palladiumChest"));
                return;
            case ItemID.PalladiumLeggings:
                append(tooltips, text("palladiumLegs"));
                return;
            case ItemID.OrichalcumBreastplate:
                append(tooltips, text("orichalcumChest"));
                return;

            case ItemID.CobaltHat:
            case ItemID.MythrilHood:
            case ItemID.AdamantiteHeadgear:
                append(tooltips, text("manaBoost"));
                break;
        }

        var set = tooltips.Find(line => line.Name == "SetBonus");
        if (set == null) {
            return;
        }

        switch (item.type) {
            case ItemID.AdamantiteHelmet:
            case ItemID.AdamantiteHeadgear:
            case ItemID.AdamantiteMask:
            case ItemID.AdamantiteBreastplate:
            case ItemID.AdamantiteLeggings:
                set.Text = set.Text.Replace("25%", "50%").Replace("maximum of 10", "maximum of 15");
                break;

            case ItemID.GladiatorHelmet:
            case ItemID.GladiatorBreastplate:
            case ItemID.GladiatorLeggings:
                set.Text += "\n" + text("gladiatorDefence");
                break;

            case ItemID.SpectreHood:
            case ItemID.SpectreRobe:
            case ItemID.SpectrePants:
                set.Text = text("spectreHealing");
                break;
        }
    }

    private static void prefix(Item item, List<TooltipLine> tooltips) {
        var (defence, dr) = ProgressionScaling.prefixStats(item.prefix);

        if (defence > 0) {
            var line = tooltips.Find(l => l.Name == "PrefixAccDefense");
            if (line != null) {
                line.Text = Language.GetTextValue(key("prefixDefence"), defence) +
                            "\n" + Language.GetTextValue(key("prefixDR"), dr * 100f);
            }
        }

        if (item.prefix == PrefixID.Lucky) {
            var line = tooltips.Find(l => l.Name == "PrefixAccCritChance");
            if (line != null) {
                line.Text += "\n" + text("prefixLuck");
            }
        }
    }

    private static string key(string name) {
        return $"Mods.VanillaQoL.Tooltips.Restored.{name}";
    }

    private static string text(string name) {
        return Language.GetTextValue(key(name));
    }

    private static void replaceAdded(List<TooltipLine> tooltips, string replacement) {
        var line = tooltips.Find(l => l.Name == "Tooltip0");
        if (line == null) {
            append(tooltips, replacement);
            return;
        }

        var newline = line.Text.IndexOf('\n');
        line.Text = (newline < 0 ? line.Text : line.Text[..newline]) + "\n" + replacement;
    }

    /// after the item's own lines, not dumped at the bottom past the material and price junk
    private static void append(List<TooltipLine> tooltips, string line) {
        var index = tooltips.FindLastIndex(l => l.Name.StartsWith("Tooltip"));
        if (index < 0) {
            index = tooltips.Count - 1;
        }

        tooltips.Insert(index + 1, new TooltipLine(VanillaQoL.instance, "restored", line));
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class IronskinTooltip : GlobalBuff {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
        if (type == BuffID.Ironskin) {
            tip = tip.Replace("8", (8 + ProgressionScaling.ironskinBonus()).ToString());
        }
    }
}
