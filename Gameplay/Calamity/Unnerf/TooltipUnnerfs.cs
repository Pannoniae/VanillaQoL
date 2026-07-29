using System.Reflection;
using CalamityMod.Items;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class TooltipUnnerfs : ModSystem {
    private const int none = -1;

    private static CalamityUnnerfConfig cfg => CalamityUnnerfConfig.Instance;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (cfg.vanillaEquipStats || cfg.scopes || cfg.meleeSpeedStacking);
    }

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
        }

        if (cfg.scopes) {
            skipItem(il, 1858, 1);          // Sniper Scope
            skipItem(il, 4005, 1);          // Recon Scope
        }

        if (cfg.meleeSpeedStacking) {
            skipItem(il, 1343, 1);          // Fire Gauntlet
            // the first 3110 block is the Abyss breath line, which is Calamity's text
            skipItem(il, 3110, 2);
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
