using System.Collections.Generic;
using CalamityMod.Items.Accessories.Wings;
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

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.wingBonuses || cfg.fireImmunity || cfg.armourBonuses ||
                                                 cfg.prefixScaling || cfg.summonSpeed);
    }

    public override void SetStaticDefaults() {
        elysian = ModContent.ItemType<ElysianWings>();

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
