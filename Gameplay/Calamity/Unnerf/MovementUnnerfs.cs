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
            if (ilCursor.Next!.Operand is float f && f == vanillaSoaringInsigniaRunAccel) {
                // still vanilla's number, so Calamity hasn't run yet and we're patching nothing???
                warn("Calamity's run acceleration nerf");
            }

            ilCursor.Next.Operand = vanillaSoaringInsigniaRunAccel;
        }

        // Shadow Armor. They leave vanilla's code in the stream and branch over it so we can just undo this skippery
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("shadowArmor"))) {
            warn("the Shadow Armor field");
            return;
        }

        if (shadow) {
            clearInsertions(ilCursor, il, "Shadow Armor");
        }

        // Soaring Insignia again, the infinite rocket boots flight. Same field, second use, ANDed with false so it
        // reads as never equipped.
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch"))) {
            warn("the Soaring Insignia field for infinite flight");
            return;
        }

        if (insignia) {
            clearInsertions(ilCursor, il, "Soaring Insignia's infinite flight");
        }
    }

    private static void clearInsertions(ILCursor ilCursor, ILContext il, string name) {
        var removed = 0;
        while (ilCursor.Next != null && !ilCursor.Next.MatchBrfalse(out _) && !ilCursor.Next.MatchBrtrue(out _)) {
            if (removed >= 8) {
                warn($"the branch after {name} (we won't eat the entire method....)");
                return;
            }

            ilCursor.Remove();
            removed++;
        }

        if (removed == 0) {
            warn($"Calamity's patch on {name}");
            // dump the stream for debugging, something is fucked?
            MonoModHooks.DumpIL(VanillaQoL.instance, il);
        }
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Player.Update, can't apply movement unnerf.");
    }
}
