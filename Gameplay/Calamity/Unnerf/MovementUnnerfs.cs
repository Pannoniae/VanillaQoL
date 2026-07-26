using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class MovementUnnerfs : ModSystem {
    private const float vanillaSoaringInsigniaRunAccel = 1.75f;

    private static bool insignia => CalamityUnnerfConfig.Instance.soaringInsignia;
    private static bool shadow => CalamityUnnerfConfig.Instance.shadowArmour;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (insignia || shadow);
    }

    public override void Load() {
        IL_Player.Update += movementPatch;
    }

    public override void Unload() {
        IL_Player.Update -= movementPatch;
    }

    private void movementPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        // Soaring Insignia's run acceleration
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch"))) {
            warn("the Soaring Insignia field for run acceleration");
            return;
        }

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
            warn("the Soaring Insignia run acceleration multiplier");
            return;
        }

        if (insignia) {
            ilCursor.Next!.Operand = vanillaSoaringInsigniaRunAccel;
        }

        // Shadow Armor. They leave vanilla's code in the stream and branch over it so we can just undo this skippery
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("shadowArmor"))) {
            warn("the Shadow Armor field");
            return;
        }

        // sorry, this is a mess, should have been a proper TryGotoNext...
        if (ilCursor.Next != null && ilCursor.Next.MatchLdarg(0) && ilCursor.Next.Next != null &&
            ilCursor.Next.Next.MatchCall(out _)) {
            if (shadow) {
                ilCursor.RemoveRange(2);
            }
        }
        else {
            warn("Shadow Armor's replacement delegate");
        }

        // Soaring Insignia again, the infinite rocket boots flight. Same field, second use, ANDed with false so it
        // reads as never equipped.
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch"))) {
            warn("the Soaring Insignia field for infinite flight");
            return;
        }

        // sorry, this is a mess, should have been a proper TryGotoNext...
        if (ilCursor.Next != null && ilCursor.Next.MatchLdcI4(0) && ilCursor.Next.Next != null &&
            ilCursor.Next.Next.OpCode == OpCodes.And) {
            if (insignia) {
                ilCursor.RemoveRange(2);
            }
        }
        else {
            warn("the infinite flight AND opcode");
        }
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Player.Update, can't apply movement unnerf.");
    }
}
