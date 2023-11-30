using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DisableTownSlimes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableTownSlimes;
    }

    public override void OnWorldLoad() {
        // todo refactor the slimes into a public list somewhere!!
        var slimes = new List<int>(Enumerable.Range(678, 688 - 678));
        slimes.Add(670);
        foreach (var npc in Main.npc) {
            if (npc.active && slimes.Contains(npc.type)) {
                npc.StrikeInstantKill();
            }
        }
    }
}