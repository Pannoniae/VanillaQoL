using System.Reflection;
using CalamityMod.CalPlayer;
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
