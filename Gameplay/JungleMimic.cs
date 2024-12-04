using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class JungleMimic : GlobalNPC {
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        // thank you Fargowiltas!
        if (QoLConfig.Instance.jungleMimic) {
            int y = spawnInfo.SpawnTileY;
            bool hardMode = Main.hardMode;
            bool jungle = spawnInfo.Player.ZoneJungle;
            bool surface = y < Main.worldSurface && !spawnInfo.Sky;
            if (hardMode && jungle && !surface) {
                pool[NPCID.BigMimicJungle] = .0025f;
            }
        }
    }
}