using System;
using CalamityMod.Buffs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity's buff changes are all about nerfing the vanilla buffs, so we revert instead of "applying it in reverse" so if they change the numbers it won't silently break...
 */
[JITWhenModsEnabled("CalamityMod")]
public class BuffUnnerfs : ModSystem {
    private static bool[] reverted = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.buffs;
    }

    public override void OnModLoad() {
        reverted = BuffID.Sets.Factory.CreateBoolSet(BuffID.Archery, BuffID.MagicPower, BuffID.Clairvoyance,
            BuffID.SugarRush, BuffID.Mining, BuffID.Swiftness, BuffID.Tipsy, BuffID.WellFed, BuffID.WellFed2,
            BuffID.WellFed3);

        var type = typeof(CalamityGlobalBuff);

        var update = type.GetMethod(nameof(CalamityGlobalBuff.Update),
            [typeof(int), typeof(Player), typeof(int).MakeByRefType()]);
        patch(update, "Update", skipReverted);

        var buffText = type.GetMethod(nameof(CalamityGlobalBuff.ModifyBuffText));
        patch(buffText, "ModifyBuffText", skipReverted);
    }

    private static void patch(System.Reflection.MethodInfo? method, string name, ILContext.Manipulator manipulator) {
        if (method == null) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find CalamityGlobalBuff.{name}, not applying unnerf");
            return;
        }

        MonoModHooks.Modify(method, manipulator);
    }

    /// bail out at the top for the buffs we're reverting. Both methods have the same sig so we can patch both w the same patcher
    private static void skipReverted(ILContext il) {
        var ilCursor = new ILCursor(il);
        var carryOn = ilCursor.DefineLabel();

        ilCursor.EmitLdarg1();
        ilCursor.EmitCall(typeof(BuffUnnerfs).GetMethod(nameof(isReverted))!);
        ilCursor.Emit(OpCodes.Brfalse, carryOn);
        ilCursor.EmitRet();
        ilCursor.MarkLabel(carryOn);
    }

    public static bool isReverted(int type) {
        return type >= 0 && type < reverted.Length && reverted[type];
    }
}
