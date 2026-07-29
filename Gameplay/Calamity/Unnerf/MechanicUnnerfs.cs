using System;
using System.Reflection;
using CalamityMod.ILEditing;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class ShimmerUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.shimmerGating;
    }

    public override void OnModLoad() {
        DetourUtils.passthru("AdjustShimmerRequirements", typeof(On_ShimmerTransforms.orig_IsItemTransformLocked), 2);
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class HallowedDodgeUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.hallowedDodge;
    }

    public override void OnModLoad() {
        DetourUtils.passthru("AddHolyProtectionCooldown",
            typeof(On_Player.orig_PutHallowedArmorSetBonusOnCooldown), 2);
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class ShieldBonkUnnerf : ModSystem {
    private const int calamitySafetyCutoff = 4;
    private const int vanillaSafetyCutoff = 0;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.shieldBonkSafety;
    }

    public override void Load() {
        IL_Player.Update_NPCCollision += restoreBonkSafety;
    }

    public override void Unload() {
        IL_Player.Update_NPCCollision -= restoreBonkSafety;
    }

    private void restoreBonkSafety(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("eocDash")) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(out _))) {
            warn("the Shield of Cthulhu dash counter", "Player.Update_NPCCollision");
            return;
        }

        if (!ilCursor.Next!.MatchLdcI4(calamitySafetyCutoff)) {
            warn("Calamity's bonk safety cutoff", "Player.Update_NPCCollision");
            return;
        }

        setInt(ilCursor.Next, vanillaSafetyCutoff);
    }

    /// small ints have a short ldc with no operand, so writing Operand alone silently does nothing, so replace it with a long ldc
    /// todo write a "conditional emit" helper somewhere?
    internal static void setInt(Instruction instruction, int value) {
        instruction.OpCode = OpCodes.Ldc_I4;
        instruction.Operand = value;
    }

    internal static void warn(string wat, string where) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in {where}!");
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class BloodMoonUnnerf : ModSystem {
    private const int calamityCrystals = 4;
    private const int vanillaCrystals = 1;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.bloodMoonRequirement;
    }

    public override void Load() {
        IL_Main.UpdateTime_StartNight += restoreRequirement;
    }

    public override void Unload() {
        IL_Main.UpdateTime_StartNight -= restoreRequirement;
    }

    private void restoreRequirement(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("moonPhase")) ||
            !ilCursor.TryGotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Player>("get_ConsumedLifeCrystals")) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(out _))) {
            ShieldBonkUnnerf.warn("the Life Crystal check", "Main.UpdateTime_StartNight");
            return;
        }

        if (!ilCursor.Next!.MatchLdcI4(calamityCrystals)) {
            ShieldBonkUnnerf.warn("Calamity's Life Crystal requirement", "Main.UpdateTime_StartNight");
            return;
        }

        ShieldBonkUnnerf.setInt(ilCursor.Next, vanillaCrystals);
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class FossilShatterUnnerf : ModSystem {
    private const int calamityNonsenseTile = 40000;
    private const int desertFossil = 404;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.fossilShattering;
    }

    public override void Load() {
        IL_WorldGen.AttemptFossilShattering += restoreShattering;
    }

    public override void Unload() {
        IL_WorldGen.AttemptFossilShattering -= restoreShattering;
    }

    private void restoreShattering(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(calamityNonsenseTile))) {
            ShieldBonkUnnerf.warn("Calamity's fossil tile ID", "WorldGen.AttemptFossilShattering");
            return;
        }

        ShieldBonkUnnerf.setInt(ilCursor.Next!, desertFossil);
    }
}

// todo move this to some shared IL helpers?
[JITWhenModsEnabled("CalamityMod")]
internal static class DetourUtils {
    internal static void passthru(string name, Type origType, int args) {
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

            for (var i = 0; i < args; i++) {
                ilCursor.EmitLdarg(i);
            }

            ilCursor.EmitCallvirt(invoke);
            ilCursor.EmitRet();
        });
    }
}
