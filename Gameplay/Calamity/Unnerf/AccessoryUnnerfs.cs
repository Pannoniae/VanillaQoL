using System.Reflection;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Projectiles;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class YoyoGloveUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.yoyoGlove;
    }

    public override void OnModLoad() {
        var preAI = typeof(CalamityGlobalProjectile).GetMethod("PreAI");
        if (preAI == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityGlobalProjectile.PreAI!");
            return;
        }

        MonoModHooks.Modify(preAI, fullDamage);
    }

    // ldfld  Player::yoyoGlove
    // brfalse -> skip the halving
    private void fullDamage(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("yoyoGlove"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find the Yo-yo Glove check in CalamityGlobalProjectile.PreAI!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitLdcI4(0);
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class MeleeSpeedStackingUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.meleeSpeedStacking;
    }

    public override void OnModLoad() {
        var updateAccessory = typeof(CalamityGlobalItem).GetMethod("UpdateAccessory");
        if (updateAccessory == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityGlobalItem.UpdateAccessory!");
            return;
        }

        var otherBuffEffects = typeof(CalamityPlayer).GetMethod("OtherBuffEffects",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (otherBuffEffects == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityPlayer.OtherBuffEffects!");
            return;
        }

        MonoModHooks.Modify(updateAccessory, patchSpeed);
        MonoModHooks.Modify(otherBuffEffects, noLadder);
    }

    // call   Player::GetAttackSpeed<MeleeDamageClass>()
    // dup / ldind.r4
    // ldc.r4 0.12
    // sub
    // stind.r4
    private void patchSpeed(ILContext il) {
        var ilCursor = new ILCursor(il);
        var x = 0;

        while (ilCursor.TryGotoNext(MoveType.After, i => i.MatchCallOrCallvirt(out var m) && m.Name == "GetAttackSpeed")) {
            if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
                break;
            }

            // only the subtractions, we're not here to eat anything they add
            if (ilCursor.Next!.Next == null || !ilCursor.Next.Next.MatchSub()) {
                continue;
            }

            ilCursor.Next.Operand = 0f;
            x++;
        }

        if (x == 0) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find any melee speed subtractions in CalamityGlobalItem.UpdateAccessory!");
            return;
        }

        VanillaQoL.instance.Logger.Info($"Gave {x} accessories their vanilla melee speed back.");
        ScopeUnnerf.zero(new ILCursor(il), 1343, [0.02f]);
    }

    // ldfld gloveLevel
    // ldc.i4.0
    // ble -> past the ladder
    private void noLadder(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<CalamityPlayer>("gloveLevel"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find the gloveLevel ladder in CalamityPlayer.OtherBuffEffects!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitLdcI4(0);
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class ScopeUnnerf : ModSystem {
    private const int reconScope = 4005;
    private const int sniperScope = 1858;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.scopes;
    }

    public override void Load() {
        // the zoom half lives in vanilla's own method, the stat half in Calamity's
        IL_Player.ApplyEquipFunctional += restoreZoom;
    }

    public override void Unload() {
        IL_Player.ApplyEquipFunctional -= restoreZoom;
    }

    public override void OnModLoad() {
        var updateAccessory = typeof(CalamityGlobalItem).GetMethod("UpdateAccessory");
        if (updateAccessory == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.UpdateAccessory!");
            return;
        }

        MonoModHooks.Modify(updateAccessory, restoreScopes);
    }

    // they splice "pop; ldarg.2; call !hideVisual" in the front of the store, so the scope only works while it's showing
    private void restoreZoom(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchStfld<Player>("scope"))) {
            warnZoom(il);
            return;
        }

        var removed = 0;
        while (ilCursor.Index > 0 && !ilCursor.Instrs[ilCursor.Index - 1].MatchLdcI4(1)) {
            if (removed >= 8) {
                warnZoom(il);
                return;
            }

            ilCursor.Index--;
            ilCursor.Remove();
            removed++;
        }

        if (removed == 0) {
            warnZoom(il);
        }
    }

    private static void warnZoom(ILContext il) {
        VanillaQoL.instance.Logger.Warn(
            "Couldn't find the scope visibility check in Player.ApplyEquipFunctional!");
        // wtf is wrong with this?
        MonoModHooks.DumpIL(VanillaQoL.instance, il);
    }

    private void restoreScopes(ILContext il) {
        var ilCursor = new ILCursor(il);

        // GetDamage += 0.05f, GetCritChance -= 5f
        if (!zero(ilCursor, reconScope, [0.05f, 5f])) {
            return;
        }

        // GetDamage -= 0.1f, GetCritChance += 2f, then critDamage += 0.15f which isn't ours to touch
        zero(ilCursor, sniperScope, [0.1f, 2f]);
    }

    internal static bool zero(ILCursor ilCursor, int type, float[] expected) {
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(type))) {
            warn($"the item {type} check");
            return false;
        }

        foreach (var value in expected) {
            if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
                warn($"item {type}'s stat changes");
                return false;
            }

            if (ilCursor.Next!.Operand is not float f || f != value) {
                warn($"item {type}'s {value} adjustment");
                return false;
            }

            ilCursor.Next.Operand = 0f;
            ilCursor.Index++;
        }

        return true;
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in CalamityGlobalItem.UpdateAccessory!");
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class WingAscentUnnerf : ModSystem {
    private const float butterflyCap = 0.6667f;
    private const float ghostCap = 0.6025f;
    private const float calamityRamp = 5f;
    private const float vanilla = 1f;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.wingAscent;
    }

    public override void OnModLoad() {
        var verticalWingSpeeds = typeof(CalamityGlobalItem).GetMethod("VerticalWingSpeeds");
        if (verticalWingSpeeds == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.VerticalWingSpeeds!");
            return;
        }

        MonoModHooks.Modify(verticalWingSpeeds, restoreAscent);
    }

    // maxAscentMultiplier *= 0.6667f; constantAscend *= 5f;   (and 0.6025f for the Ghost ones)
    private void restoreAscent(ILContext il) {
        restore(il, butterflyCap, "Butterfly");
        restore(il, ghostCap, "Ghost");
    }

    private static void restore(ILContext il, float cap, string wing) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(cap))) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find the {wing} Wings ascent cap!");
            return;
        }

        ilCursor.Next!.Operand = vanilla;
        ilCursor.Index++;

        // the ramp is the very next constant...
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _)) ||
            ilCursor.Next!.Operand is not float ramp || ramp != calamityRamp) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find the {wing} Wings ascent ramp!");
            return;
        }

        ilCursor.Next.Operand = vanilla;
    }
}
