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


[JITWhenModsEnabled("CalamityMod")]
public class RunSpeedUnnerf : ModSystem {
    private const float vanillaAsphaltTopSpeed = 3.5f;
    private const float vanillaAsphaltSlowdown = 2f;
    private const float vanillaIceSkateAccel = 3.5f;

    private const float calamityAsphaltTopSpeed = 2.25f;
    private const float calamityAsphaltSlowdown = 1f;
    private const float calamityIceSkateAccel = 2.1f;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.asphaltAndSkates;
    }

    public override void Load() {
        IL_Player.Update += restoreRunSpeeds;
    }

    public override void Unload() {
        IL_Player.Update -= restoreRunSpeeds;
    }

    private void restoreRunSpeeds(ILContext il) {
        var ilCursor = new ILCursor(il);

        // maxRunSpeed *= 3.5f; runAcceleration *= 1f; runSlowdown *= 2f;
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("powerrun"))) {
            warn("the asphalt check");
            return;
        }

        var asphalt = new Instruction[3];
        for (var i = 0; i < asphalt.Length; i++) {
            if (!ilCursor.TryGotoNext(MoveType.Before, x => x.MatchLdcR4(out _))) {
                warn("asphalt's speed constants");
                return;
            }

            asphalt[i] = ilCursor.Next!;
            ilCursor.Index++;
        }

        // the middle one is runAcceleration and vanilla already leaves it at 1f, so we only check end
        if (asphalt[0].Operand is not float top || top != calamityAsphaltTopSpeed ||
            asphalt[2].Operand is not float slowdown || slowdown != calamityAsphaltSlowdown) {
            warn("Calamity's asphalt numbers");
            return;
        }

        asphalt[0].Operand = vanillaAsphaltTopSpeed;
        asphalt[2].Operand = vanillaAsphaltSlowdown;

        // once on frozen slime blocks, once on plain ice
        for (var i = 0; i < 2; i++) {
            if (!ilCursor.TryGotoNext(MoveType.After, x => x.MatchLdfld<Player>("iceSkate"))) {
                warn($"Ice Skates check {i + 1}");
                return;
            }

            if (!ilCursor.TryGotoNext(MoveType.Before, x => x.MatchLdcR4(out _))) {
                warn($"Ice Skates acceleration {i + 1}");
                return;
            }

            if (ilCursor.Next!.Operand is not float accel || accel != calamityIceSkateAccel) {
                warn($"Calamity's Ice Skates number {i + 1}");
                return;
            }

            ilCursor.Next.Operand = vanillaIceSkateAccel;
            ilCursor.Index++;
        }
    }

    private static void warn(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Player.Update!");
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class JumpUnnerf : ModSystem {
    private const int vanillaBalloonJumpHeight = 20;
    private const float vanillaBalloonJumpSpeed = 6.51f;
    private const float vanillaInsigniaJumpBoost = 1.8f;

    private const float calamityBalloonJumpSpeed = 0.75f;
    private const float calamityInsigniaJumpBoost = 0.5f;

    private static bool balloon => CalamityUnnerfConfig.Instance.balloonJump;
    private static bool insignia => CalamityUnnerfConfig.Instance.soaringInsignia;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && (balloon || insignia);
    }

    public override void Load() {
        IL_Player.UpdateJumpHeight += restoreJumps;
    }

    public override void Unload() {
        IL_Player.UpdateJumpHeight -= restoreJumps;
    }

    private void restoreJumps(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (balloon && !restoreBalloon(ilCursor)) {
            return;
        }

        if (!insignia) {
            return;
        }

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
            warnJump("the Soaring Insignia jump boost");
            return;
        }

        if (ilCursor.Next!.Operand is not float boost || boost != calamityInsigniaJumpBoost) {
            warnJump("Calamity's Soaring Insignia jump number");
            return;
        }

        ilCursor.Next.Operand = vanillaInsigniaJumpBoost;
    }

    private static bool restoreBalloon(ILCursor ilCursor) {
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("jumpBoost")) ||
            !ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(out _))) {
            warnJump("the balloon jump speed");
            return false;
        }

        if (ilCursor.Next!.Operand is not float speed || speed != calamityBalloonJumpSpeed) {
            warnJump("Calamity's balloon jump number");
            return false;
        }

        ilCursor.EmitLdcI4(vanillaBalloonJumpHeight);
        ilCursor.Emit<Player>(OpCodes.Stsfld, "jumpHeight");

        ilCursor.Next!.Operand = vanillaBalloonJumpSpeed;
        ilCursor.Index++;

        // and the "+ jumpSpeed" they bolted onto it comes back off
        if (ilCursor.Next != null && ilCursor.Next.MatchLdsfld<Player>("jumpSpeed") &&
            ilCursor.Next.Next != null && ilCursor.Next.Next.MatchAdd()) {
            ilCursor.Remove();
            ilCursor.Remove();
        }

        return true;
    }

    private static void warnJump(string wat) {
        VanillaQoL.instance.Logger.Warn($"Couldn't find {wat} in Player.UpdateJumpHeight!");
    }
}
