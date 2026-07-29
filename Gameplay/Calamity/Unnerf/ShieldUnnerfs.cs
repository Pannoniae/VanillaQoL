using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class ShieldUnnerfs : GlobalItem {
    private static int[] ornate = null!;
    private static int[] elysian = null!;
    private static int[] valor = null!;
    private static int[] asgardian = null!;

    private static int ornateType;
    private static int elysianType;
    private static int valorType;
    private static int asgardianType;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.shieldImmunities;
    }

    public override void SetStaticDefaults() {
        ornateType = ModContent.ItemType<OrnateShield>();
        elysianType = ModContent.ItemType<ElysianAegis>();
        valorType = ModContent.ItemType<AsgardsValor>();
        asgardianType = ModContent.ItemType<AsgardianAegis>();

        ornate = [BuffID.Chilled, BuffID.Frozen, BuffID.Frostburn, BuffID.Frostburn2];

        elysian = [
            BuffID.OnFire, BuffID.OnFire3, BuffID.CursedInferno, BuffID.ShadowFlame,
            ModContent.BuffType<BrimstoneFlames>(), BuffID.Daybreak, ModContent.BuffType<HolyFlames>()
        ];

        valor = [
            BuffID.Weak, BuffID.BrokenArmor, BuffID.Bleeding, BuffID.Poisoned, BuffID.Slow, BuffID.Confused,
            BuffID.Silenced, BuffID.Cursed, BuffID.Darkness, BuffID.WindPushed, BuffID.Stoned,
            BuffID.OnFire, BuffID.OnFire3, ModContent.BuffType<BrimstoneFlames>(),
            ..ornate
        ];

        asgardian = [
            ..valor,
            ModContent.BuffType<ArmorCrunch>(), ModContent.BuffType<BrainRot>(),
            ModContent.BuffType<BurningBlood>(), BuffID.Venom, ModContent.BuffType<SulphuricPoisoning>(),
            BuffID.Webbed, BuffID.Blackout,
            BuffID.CursedInferno, BuffID.ShadowFlame, BuffID.Daybreak,
            ModContent.BuffType<Nightwither>(), ModContent.BuffType<HolyFlames>(),
            ModContent.BuffType<GodSlayerInferno>()
        ];
    }

    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return lateInstantiation && Constants.postSetupDone && isShield(item.type);
    }

    private static bool isShield(int type) {
        return type == ornateType || type == elysianType || type == valorType || type == asgardianType;
    }

    private static int[]? immunities(int type) {
        if (type == ornateType) {
            return ornate;
        }

        if (type == elysianType) {
            return elysian;
        }

        if (type == valorType) {
            return valor;
        }

        return type == asgardianType ? asgardian : null;
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        var immune = immunities(item.type);
        if (immune == null) {
            return;
        }

        // Ornate Shield never walked on fire blocks, the other three did
        if (item.type != ornateType) {
            player.fireWalk = true;
        }

        foreach (var buff in immune) {
            player.buffImmune[buff] = true;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.type == ornateType) {
            insert(tooltips, text("coldImmunity"));
        }
        else if (item.type == elysianType) {
            insert(tooltips, text("fireBlocks"), text("fireDebuffs"));
        }
        else if (item.type == valorType) {
            insert(tooltips, text("fireBlocks"), text("mostDebuffs"));
        }
        else if (item.type == asgardianType) {
            insert(tooltips, text("fireBlocks"), text("vastDebuffs"));
        }
    }

    private static string text(string key) {
        return Language.GetTextValue($"Mods.VanillaQoL.Tooltips.Shields.{key}");
    }

    /// after the item's own tooltip lines, not dumped at the bottom past the material/price junk
    private void insert(List<TooltipLine> tooltips, params string[] lines) {
        var index = tooltips.FindLastIndex(line => line.Name.StartsWith("Tooltip"));
        if (index < 0) {
            index = tooltips.Count - 1;
        }

        for (var i = 0; i < lines.Length; i++) {
            tooltips.Insert(index + 1 + i, new TooltipLine(Mod, $"shieldUnnerf{i}", lines[i]));
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class ShieldRecipeUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.shieldImmunities;
    }

    public override void PostAddRecipes() {
        var valor = ModContent.ItemType<AsgardsValor>();
        var patched = 0;

        foreach (var recipe in Main.recipe) {
            if (recipe != null && recipe.HasResult(valor)) {
                recipe.AddIngredient(ItemID.AnkhShield);
                patched++;
            }
        }

        if (patched == 0) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find Asgard's Valor's recipe... enjoy?");
        }
    }
}
