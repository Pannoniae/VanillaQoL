using System;
using System.Reflection;
using CalamityMod.CalPlayer;
using CalamityMod.ILEditing;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Vanilla regenerates the hidden lifeSteal counter at 0.6 a frame in Classic and 0.5 in Expert. Calamity subtracts
 * another 0.45 / 0.4 on top every frame. The wiki is lying LOL, and the Cal code is full of unused / misleading variables here :\ anyway here it is, this MIGHT change in the future
 * if they fix their stuff up
 *
 * todo also do Spectre and Vampire Knives in the future? it's mixed with fixes so be careful out there
 */
[JITWhenModsEnabled("CalamityMod")]
public class LifestealUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.lifesteal;
    }

    public override void OnModLoad() {
        var miscEffects = typeof(CalamityPlayer).GetMethod("MiscEffects",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (miscEffects == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityPlayer.MiscEffects, lifesteal stays at a quarter speed.");
            return;
        }

        MonoModHooks.Modify(miscEffects, restoreRecoveryRate);
    }

    // ldfld lifeSteal
    // ldloc reduction
    // sub
    // stfld lifeSteal
    private void restoreRecoveryRate(ILContext il) {
        var ilCursor = new ILCursor(il);

        // more stable to match sub + stfld
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchSub(), i => i.MatchStfld<Player>("lifeSteal"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the lifesteal recovery reduction in CalamityPlayer.MiscEffects!");
            return;
        }

        // just remove the code in a straightforward manner, no need to "restore" or any of that crap
        ilCursor.Index--;
        ilCursor.EmitPop();
        ilCursor.EmitPop();
        ilCursor.Remove();
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class SpectreVampireUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.spectreAndVampire;
    }

    public override void OnModLoad() {
        passthru("AdjustSpectreHealing", typeof(On_Projectile.orig_ghostHeal));
        passthru("AdjustVampireHealing", typeof(On_Projectile.orig_vampireHeal));
    }

    private static void passthru(string name, Type origType) {
        var detour = typeof(ILChanges).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (detour == null) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find ILChanges.{name}!");
            return;
        }

        var invoke = origType.GetMethod("Invoke");
        if (invoke == null) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find {origType.Name}.Invoke!");
            return;
        }

        MonoModHooks.Modify(detour, il => {
            var ilCursor = new ILCursor(il);

            // orig, self, dmg, Position, victim
            for (var i = 0; i < 5; i++) {
                ilCursor.EmitLdarg(i);
            }

            ilCursor.EmitCallvirt(invoke);
            ilCursor.EmitRet();
        });
    }
}
