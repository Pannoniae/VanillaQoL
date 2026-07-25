using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class CalamityMovement : ModPlayer {
    private static bool hasOverhaul;

    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active;
    }

    public override void Load() {
        hasOverhaul = ModLoader.HasMod("TerrariaOverhaul");
    }

    public override void UpdateEquips() {
        // stacks with the Brick Layer and the cement mixer, yes
        if (CalamityQoLConfig.Instance.fasterTilePlacement) {
            Player.tileSpeed += 0.5f;
            Player.wallSpeed += 0.5f;
        }
    }

    public override void PostUpdateMiscEffects() {
        if (CalamityQoLConfig.Instance.fasterMovement && !hasOverhaul) {
            Player.moveSpeed += 0.5f;
        }

        if (CalamityQoLConfig.Instance.fasterJumpSpeed) {
            // 4% is enough for 7 tiles, which is the point. with wings you get the silly number
            Player.jumpSpeedBoost += Player.wingsLogic > 0 ? 1.2f : 0.2f;
        }

        if (CalamityQoLConfig.Instance.fasterFall) {
            fastFall();
        }
    }

    private void fastFall() {
        var airborne = Player.velocity.Y != 0;
        var holdingDown = Player.controlDown && !Player.controlJump;
        // CCed covers frozen, webbed and stoned
        var canMove = !Player.CCed && !Player.tongued;
        var unhindered = !Player.wet && !Player.pulley && Player.ropeCount == 0 && Player.grappling[0] == -1;
        if (!airborne || !holdingDown || !canMove || !unhindered) {
            return;
        }

        // double gravity, but the max fall speed stays where it was so it does't look silly
        Player.velocity.Y += Player.gravity * Player.gravDir;
        if (Player.velocity.Y * Player.gravDir > Player.maxFallSpeed) {
            Player.velocity.Y = Player.maxFallSpeed * Player.gravDir;
        }
    }
}

public class HigherJumps : ModSystem {
    private const float vanillaJumpSpeed = 5.01f;
    private const float jumpBoost = 0.7f;

    public override bool IsLoadingEnabled(Mod mod) {
        return CalamityQoLConfig.active && CalamityQoLConfig.Instance.higherJumpHeight;
    }

    public override void Load() {
        IL_Player.Update += higherJumpPatch;
    }

    // // [23334 4 - 23334 25]
    // IL_c31f: ldc.r4       5.01
    // IL_c324: stsfld       float32 Terraria.Player::jumpSpeed
    private static void higherJumpPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(vanillaJumpSpeed),
                i => i.MatchStsfld<Player>("jumpSpeed"))) {
            VanillaQoL.instance.Logger.Warn("Failed to locate the base jump speed (Player.jumpSpeed)");
            return;
        }

        // half a tile's worth more
        ilCursor.Remove();
        ilCursor.EmitLdcR4(vanillaJumpSpeed + jumpBoost);
    }
}
