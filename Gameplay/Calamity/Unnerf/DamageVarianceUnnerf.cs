using System.Reflection;
using CalamityMod.ILEditing;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity detours Main.DamageVar, detaching their detour fucking with tML's internal hook bookkeeping and lying to it.
 * This is funny and memes but it's easier to gut the detour body instead so it doesn't do anything.
 */
[JITWhenModsEnabled("CalamityMod")]
public class DamageVarianceUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.damageVariance;
    }

    public override void OnModLoad() {
        var adjust = typeof(ILChanges).GetMethod("AdjustDamageVariance",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (adjust == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find ILChanges.AdjustDamageVariance, damage stays at ±5%.");
            return;
        }

        MonoModHooks.Modify(adjust, passThrough);
    }

    // ldarg.2      percent
    // ldsfld       Main::DefaultDamageVariationPercent
    // bne.un.s     -> the call
    // ldc.i4.5
    // starg.s      percent
    // ldarg.0/1/2
    // ldc.r4       0.0
    // callvirt     orig
    private void passThrough(ILContext il) {
        var ilCursor = new ILCursor(il);

        // percent = 5 -> percent = percent. Kinda hacky but oh well
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(5))) {
            warn("the -+5% spread constant");
            return;
        }

        ilCursor.Next!.OpCode = OpCodes.Ldarg_2;

        // ...same for the luck
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0f))) {
            warn("the luck constant");
            return;
        }

        ilCursor.Next!.OpCode = OpCodes.Ldarg_3;
        ilCursor.Next.Operand = null;
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Calamity's damage variance detour!");
    }
}
