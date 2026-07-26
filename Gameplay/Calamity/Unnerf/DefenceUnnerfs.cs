using System.Reflection;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity has a DR formula of endurance = 1 - 1/(1 + endurance) to prevent DR stacking.
 * It's also kind of lame let's be real
 */
[JITWhenModsEnabled("CalamityMod")]
public class DamageReductionUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.damageReduction;
    }

    public override void OnModLoad() {
        var limits = typeof(CalamityPlayer).GetMethod("Limits", BindingFlags.Instance | BindingFlags.NonPublic);
        if (limits == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityPlayer.Limits, can't apply the damage reduction unnerf :(");
            return;
        }

        MonoModHooks.Modify(limits, removeDRScaling);
    }

    // ldc.r4 1; ldc.r4 1; ...; ldfld endurance; add; div; sub; stfld endurance
    private void removeDRScaling(ILContext il) {
        var ilCursor = new ILCursor(il);

        // match the div + sub rather than the stfld so it's more robust
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchDiv(), i => i.MatchSub())) {
            VanillaQoL.instance.Logger.Warn("Couldn't match the DR rescale arithmetic in CalamityPlayer.Limits!");
            return;
        }

        if (ilCursor.Next == null || !ilCursor.Next.MatchStfld<Player>("endurance")) {
            VanillaQoL.instance.Logger.Warn("Found the DR rescale in CalamityPlayer.Limits but it didn't stfld, very strange?");
            return;
        }

        // undo
        ilCursor.EmitPop();
        ilCursor.EmitPop();
        ilCursor.Remove();
    }
}

/**
 * Beetle Shell and Solar Flare lose their vanilla multiplicative DR and get a flat additive one instead, which is
 * worth a lot less once you're stacking anything else
 */
[JITWhenModsEnabled("CalamityMod")]
public class MeleeArmourDRUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.meleeArmourDR;
    }

    public override void Load() {
        IL_Player.ApplyVanillaHurtEffectModifiers += restoreMultiplicativeDR;

        var updateArmorSet = typeof(CalamityGlobalItem).GetMethod("UpdateArmorSet");
        if (updateArmorSet == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.UpdateArmorSet!");
            return;
        }

        MonoModHooks.Modify(updateArmorSet, restoreSolarFlareBaseDR);
    }

    public override void Unload() {
        IL_Player.ApplyVanillaHurtEffectModifiers -= restoreMultiplicativeDR;
    }

    /// they append "ldc.i4.0; and" to both setbonus so remove it
    private void restoreMultiplicativeDR(ILContext il) {
        var ilCursor = new ILCursor(il);
        unand(ilCursor, "setSolar", "Solar Flare");
        unand(ilCursor, "beetleDefense", "Beetle Shell");
    }

    private static void unand(ILCursor ilCursor, string field, string name) {
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>(field))) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find {name}'s set bonus field in Player.ApplyVanillaHurtEffectModifiers!");
            return;
        }

        if (ilCursor.Next != null && ilCursor.Next.MatchLdcI4(0) && ilCursor.Next.Next != null &&
            ilCursor.Next.Next.OpCode == OpCodes.And) {
            ilCursor.RemoveRange(2);
        }
        else {
            VanillaQoL.instance.Logger.Warn($"Couldn't find {name}'s set bonus check!");
        }
    }

    /// "player.endurance -= 0.12f", time to drop the store so it won't do anything
    private void restoreSolarFlareBaseDR(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.12f), i => i.MatchSub())) {
            VanillaQoL.instance.Logger.Warn("Couldn't find Solar Flare's 12% DR subtraction in UpdateArmorSet!");
            return;
        }

        if (ilCursor.Next == null || !ilCursor.Next.MatchStfld<Player>("endurance")) {
            VanillaQoL.instance.Logger.Warn("Found Solar Flare's 12% subtraction but not the stfld, skipping patch!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitPop();
        ilCursor.Remove();
    }
}
