using CalamityMod.CalPlayer;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class WhipTagCritUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.whipTagCrits;
    }

    public override void Load() {
        IL_Projectile.Damage += restoreTagCrits;
    }

    public override void Unload() {
        IL_Projectile.Damage -= restoreTagCrits;
    }

    private static void warnWhip(ILContext il, int which) {
        VanillaQoL.instance.Logger.Warn(
            $"Couldn't find whip tag crit injection {which + 1} of 2 in Projectile.Damage!");
        MonoModHooks.DumpIL(VanillaQoL.instance, il);
    }

    private void restoreTagCrits(ILContext il) {
        var ilCursor = new ILCursor(il);

        for (var i = 0; i < 2; i++) {
            if (!ilCursor.TryGotoNext(MoveType.Before, x => x.MatchLdloc(30))) {
                warnWhip(il, i);
                return;
            }

            var removed = 0;
            while (ilCursor.Next != null && !ilCursor.Next.MatchStloc(30)) {
                if (removed >= 8) {
                    warnWhip(il, i);
                    return;
                }

                ilCursor.Remove();
                removed++;
            }

            if (removed == 0) {
                warnWhip(il, i);
                return;
            }
        }
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class TerrarianUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.terrarianProjectiles;
    }

    public override void Load() {
        IL_Projectile.AI_099_2 += unlimitedYoyoWorks;
    }

    public override void Unload() {
        IL_Projectile.AI_099_2 -= unlimitedYoyoWorks;
    }

    private void unlimitedYoyoWorks(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(603))) {
            warn("the Terrarian's yoyo ID");
            return;
        }

        var removed = 0;
        while (ilCursor.Next != null && ilCursor.Next.OpCode.FlowControl != FlowControl.Cond_Branch) {
            if (removed >= 8) {
                warn("the branch after the Terrarian's yoyo ID");
                MonoModHooks.DumpIL(VanillaQoL.instance, il);
                return;
            }

            ilCursor.Remove();
            removed++;
        }

        if (removed == 0) {
            warn("Calamity's projectile cap");
            MonoModHooks.DumpIL(VanillaQoL.instance, il);
        }
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Projectile.AI_099_2!");
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class SummonerCrossClassUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.summonerCrossClass;
    }

    public override void OnModLoad() {
        var modifyHit = typeof(CalamityPlayer).GetMethod("ModifyHitNPCWithProj");
        if (modifyHit == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityPlayer.ModifyHitNPCWithProj!");
            return;
        }

        MonoModHooks.Modify(modifyHit, noPenalty);
    }

    // call    ShouldTriggerSummonPenalty
    // brfalse -> past the 0.75x
    private void noPenalty(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt(out var m) && m.Name == "ShouldTriggerSummonPenalty")) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find the summoner cross-class check in CalamityPlayer.ModifyHitNPCWithProj!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitLdcI4(0);
    }
}
