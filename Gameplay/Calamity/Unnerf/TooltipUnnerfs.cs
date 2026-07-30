using System.Reflection;
using CalamityMod.Items;
using MonoMod.Cil;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class TooltipUnnerfs : ModSystem {
    private const int none = -1;

    private static CalamityUnnerfConfig cfg => CalamityUnnerfConfig.Instance;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.vanillaEquipStats || cfg.scopes || cfg.meleeSpeedStacking ||
                                                 cfg.buffs || cfg.blackBelt || cfg.brainOfConfusion ||
                                                 cfg.yoyoGlove || cfg.jungleArmour || cfg.classConversions ||
                                                 cfg.soaringInsignia || cfg.projYoyoStats);
    }

    private static readonly int[] yoyos = [
        ItemID.WoodYoyo, ItemID.Rally, ItemID.CorruptYoyo, ItemID.CrimsonYoyo, ItemID.JungleYoyo,
        ItemID.Code1, ItemID.Valor, ItemID.Cascade, ItemID.HiveFive, ItemID.FormatC, ItemID.Gradient,
        ItemID.Chik, ItemID.HelFire, ItemID.Amarok, ItemID.Code2, ItemID.Yelets, ItemID.RedsYoyo,
        ItemID.ValkyrieYoyo, ItemID.Kraken, ItemID.TheEyeOfCthulhu, ItemID.Terrarian
    ];

    public override void OnModLoad() {
        var modifyVanillaTooltips = typeof(CalamityGlobalItem).GetMethod("ModifyVanillaTooltips",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (modifyVanillaTooltips == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityGlobalItem.ModifyVanillaTooltips!");
            return;
        }

        MonoModHooks.Modify(modifyVanillaTooltips, honestTooltips);
    }

    private void honestTooltips(ILContext il) {
        if (cfg.vanillaEquipStats) {
            unreplace(il, "13%", "8%");     // Shroomite Breastplate ranged damage
            unreplace(il, "16%", "10%");    // Vortex Helmet ranged damage
            unreplace(il, "7%", "5%");      // Vortex Helmet ranged crit
            unreplace(il, "26%", "20%");    // Solar Flare Helmet melee crit
            skipItem(il, 2275, 1);          // Magic Hat
            skipItem(il, 1282, 1);          // Amethyst Robe
            skipItem(il, 1283, 1);          // Topaz Robe
            skipItem(il, 1284, 1);          // Sapphire Robe
            skipItem(il, 1285, 1);          // Emerald Robe
            skipItem(il, 1286, 1);          // Ruby Robe
            skipItem(il, 4256, 1);          // Amber Robe, shares the Ruby if
            skipItem(il, 1287, 1);          // Diamond Robe
            skipItem(il, 1215, 1);          // Titanium Mask
            skipItem(il, 2277, 1);          // Gi
            skipItem(il, 3871, 1);          // Valhalla Knight's Helm
            skipItem(il, 3872, 2);          // Valhalla Knight's Breastplate
            skipItem(il, 3873, 1);          // Valhalla Knight's Greaves
        }

        if (cfg.scopes) {
            skipItem(il, 1858, 1);          // Sniper Scope
            skipItem(il, 4005, 1);          // Recon Scope
            skipItem(il, 1300, 1);          // Rifle Scope
        }

        if (cfg.meleeSpeedStacking) {
            skipItem(il, 1343, 1);          // Fire Gauntlet
            // the first 3110 block is the Abyss breath line, which is Calamity's text
            skipItem(il, 3110, 2);
            skipItem(il, 211, 1);           // Feral Claws
            skipItem(il, 897, 1);           // Power Glove
            skipItem(il, 3992, 1);          // Berserker's Glove
            skipItem(il, 936, 1);           // Mechanical Glove
            skipItem(il, 899, 1);           // Sun Stone
            skipItem(il, 900, 1);           // Moon Stone
            skipItem(il, 1865, 1);          // Celestial Stone
        }

        // "Increases wing flight time by 33%", says the tooltip LOL
        if (cfg.soaringInsignia) {
            skipItem(il, 4989, 1);          // Soaring Insignia
        }

        if (cfg.projYoyoStats) {
            foreach (var yoyo in yoyos) {
                skipItem(il, yoyo, 1);
            }
        }

        // Not unreplace - "25%" -> "15%" is also the Ancient Chisel's rewrite
        if (cfg.buffs) {
            skipItem(il, 290, 1);           // Swiftness Potion
            skipItem(il, 294, 1);           // Magic Power Potion
            skipItem(il, 2322, 1);          // Mining Potion
            skipItem(il, 303, 1);           // Archery Potion
            skipItem(il, 353, 1);           // Ale
            skipItem(il, 2266, 1);          // Sake
        }

        // the dodge blurb spells out Calamity's unified 90 second cooldown. Vanilla's own text is right again
        // once the dodges are vanilla
        if (cfg.blackBelt) {
            skipItem(il, 963, 1);           // Black Belt
            skipItem(il, 984, 1);           // Master Ninja Gear
        }

        if (cfg.brainOfConfusion) {
            skipItem(il, 3223, 1);          // Brain of Confusion
        }

        if (cfg.yoyoGlove) {
            skipItem(il, 3366, 1);          // Yoyo Bag, and the glove shares its if
            skipItem(il, 3334, 1);          // Yo-yo Glove
        }

        // the mana and crit these knock off come from JungleArmorSetChange, which the toggle deletes whole
        if (cfg.jungleArmour) {
            skipItem(il, 228, 1);           // Jungle Hat
            skipItem(il, 960, 1);           // Ancient Cobalt Helmet, shares the Jungle Hat if
            skipItem(il, 230, 1);           // Jungle Pants
            skipItem(il, 962, 1);           // Ancient Cobalt Leggings
        }

        // these describe rogue velocity and rogue damage on armour we've put back in its own class
        if (cfg.classConversions) {
            skipItem(il, 3806, 1);          // Monk's Bushy Brow Bald Cap
            skipItem(il, 3807, 1);          // Monk's Shirt
            skipItem(il, 3808, 1);          // Monk's Pants
            skipItem(il, 3880, 1);          // Shinobi Infiltrator's Helmet
            skipItem(il, 3881, 1);          // Shinobi Infiltrator's Torso
            skipItem(il, 3882, 1);          // Shinobi Infiltrator's Pants
        }
    }

    /// point Replace's second argument at its first argument so text = text
    /// kinda hacky, we might replace it with something better like deleting the whole assignment?
    private static void unreplace(ILContext il, string search, string replacement) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdstr(search), i => i.MatchLdstr(replacement),
                i => i.MatchCallOrCallvirt(out var m) && m.Name == "Replace")) {
            warn($"the \"{search}\" -> \"{replacement}\" tooltip rewrite");
            return;
        }

        ilCursor.Index++;
        ilCursor.Next!.Operand = search;
    }

    /// same crap, we just skip it but we might consider removing it entirely?
    private static void skipItem(ILContext il, int type, int occurrence) {
        var ilCursor = new ILCursor(il);

        for (var i = 0; i < occurrence; i++) {
            if (!ilCursor.TryGotoNext(MoveType.Before, x => x.MatchLdcI4(type))) {
                warn($"tooltip check {occurrence} for item {type}");
                return;
            }

            if (i < occurrence - 1) {
                ilCursor.Index++;
            }
        }

        ShieldBonkUnnerf.setInt(ilCursor.Next!, none);
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn(
            $"Couldn't find {wat} in CalamityGlobalItem.ModifyVanillaTooltips, that tooltip stays wrong.");
    }
}
