using System.Reflection;
using CalamityMod.CalPlayer;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Black Belt and Brain of Confusion don't have vanilla cooldowns anymore and put on one shared 90 second cooldown and
 * "Dodges no longer trigger if an attack deals less than 5% of the player's maximum health, and now have a varying cooldown based on how much damage was dodged, ranging from 20 seconds at 5% maximum health to 90 seconds at 50% maximum health."
 * WHAT IS THIS SHIT
 * literally touhou, time to fix :(
 */
[JITWhenModsEnabled("CalamityMod")]
public class DodgeUnnerfs : ModSystem {
    private static bool belt => CalamityUnnerfConfig.Instance.blackBelt;
    private static bool brain => CalamityUnnerfConfig.Instance.brainOfConfusion;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (belt || brain);
    }

    public override void Load() {
        IL_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += restoreVanillaDodges;

        // UpdateEquips, not MiscEffects - the dodge registration lives in CalamityPlayer.cs and the two are
        // partials of the same class, so it's easy to grab the wrong one and get a silent half-revert
        var updateEquips = typeof(CalamityPlayer).GetMethod("UpdateEquips",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (updateEquips == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityPlayer.UpdateEquips - not touching the dodges at all, half an unnerf is worse than none.");
            return;
        }

        MonoModHooks.Modify(updateEquips, stopRegisteringDodges);
    }

    public override void Unload() {
        IL_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float -= restoreVanillaDodges;
    }

    /// Cal reverts with "pop; ldc.i4.0" so vanilla thinks you have neither equipped, yeet
    private void restoreVanillaDodges(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (belt) {
            unskip(ilCursor, "blackBelt", "Black Belt");
        }

        if (brain) {
            unskip(new ILCursor(il), "brainOfConfusionItem", "Brain of Confusion");
        }
    }

    private static void unskip(ILCursor ilCursor, string field, string name) {
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>(field))) {
            warn($"the {name} field in Player.Hurt");
            return;
        }

        if (ilCursor.Next != null && ilCursor.Next.OpCode == OpCodes.Pop && ilCursor.Next.Next != null &&
            ilCursor.Next.Next.MatchLdcI4(0)) {
            ilCursor.RemoveRange(2);
        }
        else {
            warn($"{name}'s skipped check in Player.Hurt");
        }
    }

    /// stop registering the Cal dodges
    private void stopRegisteringDodges(ILContext il) {
        if (belt) {
            var ilCursor = new ILCursor(il);
            if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("blackBelt"))) {
                ilCursor.Emit(OpCodes.Pop);
                ilCursor.Emit(OpCodes.Ldc_I4_0);
            }
            else {
                warn("the Black Belt field in UpdateEquips");
            }
        }

        if (brain) {
            var ilCursor = new ILCursor(il);
            if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("brainOfConfusionItem"))) {
                // this one's an object compared against null rather than a bool, so hand it a null
                ilCursor.Emit(OpCodes.Pop);
                ilCursor.Emit(OpCodes.Ldnull);
            }
            else {
                warn("the Brain of Confusion field in MiscEffects");
            }
        }
    }

    private static void warn(string what) {
        VanillaQoL.instance.Logger.Warn(
            $"Couldn't find {what}. This is VERY wrong, the dodge unnerfs are completely fucked!!!");
    }
}
