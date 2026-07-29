using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Items;
using CalamityMod.Items.VanillaArmorChanges;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class ArmourUnnerfs : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (CalamityUnnerfConfig.Instance.meteorArmour ||
                                                 CalamityUnnerfConfig.Instance.jungleArmour ||
                                                 CalamityUnnerfConfig.Instance.frostArmour ||
                                                 CalamityUnnerfConfig.Instance.nebulaArmour);
    }

    public override void PostSetupContent() {
        var field = typeof(VanillaArmorChangeManager).GetField("ArmorChanges",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is not List<VanillaArmorChange> changes) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find VanillaArmorChangeManager.ArmorChanges!, can't apply the Meteor Armour unnerf :(");
            return;
        }

        if (CalamityUnnerfConfig.Instance.meteorArmour) {
            remove<MeteorArmorSetChange>(changes, "Meteor");
        }

        if (CalamityUnnerfConfig.Instance.jungleArmour) {
            remove<JungleArmorSetChange>(changes, "Jungle");
        }

        if (CalamityUnnerfConfig.Instance.frostArmour) {
            remove<FrostArmorSetChange>(changes, "Frost");
        }

        if (CalamityUnnerfConfig.Instance.nebulaArmour) {
            remove<NebulaArmorSetChange>(changes, "Nebula");
        }
    }

    private static void remove<T>(List<VanillaArmorChange> changes, string name) where T : VanillaArmorChange {
        if (changes.RemoveAll(change => change is T) == 0) {
            VanillaQoL.instance.Logger.Warn($"No {name} armour change to remove, someone else removed it?");
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class NebulaUnnerf : ModSystem {
    private const int vanillaManaRegenThreshold = 6;
    private const int vanillaLifeRegenPerBooster = 6;
    private const float vanillaDamagePerBooster = 0.15f;

    private const int nebulaLifeBuff = 173;
    private const int nebulaDamageBuff = 179;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.nebulaArmour;
    }

    public override void Load() {
        IL_Player.UpdateLifeRegen += restoreDotIgnoring;
        IL_Player.UpdateManaRegen += restoreManaRegen;
        IL_Player.UpdateBuffs += restoreBoosterStrength;
    }

    public override void Unload() {
        IL_Player.UpdateLifeRegen -= restoreDotIgnoring;
        IL_Player.UpdateManaRegen -= restoreManaRegen;
        IL_Player.UpdateBuffs -= restoreBoosterStrength;
    }

    // they poison the count with "pop; ldc.i4.0", time to un-pop this
    private void restoreDotIgnoring(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("nebulaLevelLife"))) {
            warn("the Nebula life booster", "UpdateLifeRegen");
            return;
        }

        if (ilCursor.Next == null || !ilCursor.Next.MatchPop() ||
            ilCursor.Next.Next == null || !ilCursor.Next.Next.MatchLdcI4(0)) {
            warn("Calamity's damage over time nerf", "UpdateLifeRegen");
            return;
        }

        ilCursor.Remove();
        ilCursor.Remove();
    }

    private void restoreManaRegen(ILContext il) {
        var ilCursor = new ILCursor(il);

        // anchor on the booster field, a bare ldc.i4 12 is far too common to trust on its own
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("nebulaLevelMana"))) {
            warn("the Nebula mana booster", "UpdateManaRegen");
            return;
        }

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(out _))) {
            warn("the Nebula mana regen threshold", "UpdateManaRegen");
            return;
        }

        ShieldBonkUnnerf.setInt(ilCursor.Next!, vanillaManaRegenThreshold);
    }

    private void restoreBoosterStrength(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(nebulaLifeBuff)) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("lifeRegen")) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(out _))) {
            warn("the Nebula life regen per booster", "UpdateBuffs");
            return;
        }

        ShieldBonkUnnerf.setInt(ilCursor.Next!, vanillaLifeRegenPerBooster);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(nebulaDamageBuff)) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
            warn("the Nebula damage per booster", "UpdateBuffs");
            return;
        }

        ilCursor.Next!.Operand = vanillaDamagePerBooster;
    }

    private static void warn(string wat, string where) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Player.{where}!");
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class ClassConversionUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.classConversions;
    }

    public override void OnModLoad() {
        var updateArmorSet = typeof(CalamityGlobalItem).GetMethod("UpdateArmorSet");
        if (updateArmorSet == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.UpdateArmorSet!");
            return;
        }

        MonoModHooks.Modify(updateArmorSet, noCrystalAssassinRogue);
    }

    public override void PostSetupContent() {
        var field = typeof(VanillaArmorChangeManager).GetField("ArmorChanges",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is not List<VanillaArmorChange> changes) {
            VanillaQoL.instance.Logger.Warn("Couldn't find VanillaArmorChangeManager.ArmorChanges!");
            return;
        }

        if (changes.RemoveAll(change => change is MonkT2ArmorSetChange or MonkT3ArmorSetChange) == 0) {
            VanillaQoL.instance.Logger.Warn("No Monk armour changes to remove, someone else got there first?");
        }
    }

    // ldstr "CrystalAssassin"
    // call  string::op_Equality
    private void noCrystalAssassinRogue(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchLdstr("CrystalAssassin"), i => i.MatchCallOrCallvirt(out _))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find the Crystal Assassin set in CalamityGlobalItem.UpdateArmorSet!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitLdcI4(0);
    }
}
