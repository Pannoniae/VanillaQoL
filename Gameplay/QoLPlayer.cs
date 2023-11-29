using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class QoLPlayer : ModPlayer {
    public override void OnEnterWorld() {
        // only on client
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            if (QoLConfig.Instance.autoJoinTeam) {
                var team = QoLConfig.Instance.teamToAutoJoin;
                Main.LocalPlayer.team = (int)team;
                NetMessage.SendData(MessageID.PlayerTeam, number: Main.myPlayer);
            }
        }
    }
    public override void ProcessTriggers(TriggersSet triggersSet) {
        if (QoLConfig.Instance.mapSharing && Main.netMode == NetmodeID.MultiplayerClient && Main.mapEnabled && QoLSharedMapSystem.shareKeybind.JustPressed) {
            QoLSharedMapSystem.instance.sendSyncRequestPacket();
            Main.NewText("Requested map sync.");
        }
    }
}