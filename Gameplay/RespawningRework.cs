using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

public class RespawningRework : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.respawningRework;
    }

    public override void Load() {
        // sorry calamity, we do the respawning instead.
        // this one makes it configurable, so the calamity logic needs to be bypassed
        // the defaults are close to the calamity values
        if (VanillaQoL.isCalamityLoaded()) {
            CalamityLogic2.load();
        }

        IL_Player.UpdateDead += deathPatch;

    }

    public override void Unload() {
        IL_Player.UpdateDead -= deathPatch;
    }

    private void deathPatch(ILContext il) {
        ILCursor ilCursor = new ILCursor(il);
        // IL_0424: stfld        int32 Terraria.Player::respawnTimer
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchStfld<Player>("respawnTimer"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match first respawnTimer set in Player.UpdateDead!");
        }
        // we pop twice since instance + value was on the stack
        ilCursor.EmitPop();
        ilCursor.EmitPop();
        ilCursor.Remove();

        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchStfld<Player>("respawnTimer"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match second respawnTimer set in Player.UpdateDead!");
        }
        ilCursor.EmitPop();
        ilCursor.EmitPop();
        ilCursor.Remove();
    }
}