using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class VanillaQoLPlayer : ModPlayer {
    public override void OnEnterWorld() {
        // only on client
        if (Main.netMode == NetmodeID.MultiplayerClient && QoLConfig.Instance.autoJoinTeam) {
            var team = QoLConfig.Instance.teamToAutoJoin;
            Main.LocalPlayer.team = (int)team;
            NetMessage.SendData(MessageID.PlayerTeam, number: Main.myPlayer);
        }
    }
}