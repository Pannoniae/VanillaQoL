using Terraria;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DisableTownSlimes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableTownSlimes;
    }

    public override void OnWorldLoad() {
        foreach (var npc in Main.npc) {
            if (npc.active && Constants.slimes.Contains(npc.type)) {
                npc.StrikeInstantKill();
            }
        }
    }
}