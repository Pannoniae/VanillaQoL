using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class LockOn : ModSystem {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.enableLockOn;
    }


    public override void Load() {
        LockOnHelper.ForceUsability = true;
        IL_UIManageControls.AssembleBindPanels += showLockOnKeyboard;
    }

    public override void Unload() {
        IL_LockOnHelper.Update -= addInput;
    }

    public override void PreUpdatePlayers() {
        // update with config
        LockOnHelper.ForceUsability = QoLConfig.Instance.lockOn;
    }


    // [349 7 - 349 34]
    // IL_0404: ldarg.0      // this
    // IL_0405: call         instance void Terraria.GameContent.UI.States.UIManageControls::OnAssembleBindPanels()
    public void showLockOnKeyboard(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg0(),
                i => i.MatchCall<UIManageControls>("OnAssembleBindPanels"))) {
            // load bindings1
            ilCursor.EmitLdloc0();
            ilCursor.Emit<LockOn>(OpCodes.Call, "addLockOnToUI");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Couldn't match OnAssembleBindPanels in UIManageControls.AssembleBindPanels!");
        }
    }

    public static void addLockOnToUI(List<string> bindings) {
        bindings.AddAfter("QuickBuff", "LockOn");
    }

    public void addInput(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.Emit<LockOn>(OpCodes.Call, "lockOnInput");
    }
}