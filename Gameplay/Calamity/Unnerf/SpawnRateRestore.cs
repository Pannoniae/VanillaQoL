using CalamityMod;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class SpawnRateRestore : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.spawnRates;
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
        if (Main.SceneMetrics.WaterCandleCount > 0) {
            spawnRate = (int)(spawnRate * 0.9);
            maxSpawns = (int)(maxSpawns * 1.1f);
        }

        if (player.enemySpawns) {
            spawnRate = (int)(spawnRate * 0.8);
            maxSpawns = (int)(maxSpawns * 1.2f);
        }

        if (Main.SceneMetrics.PeaceCandleCount > 0) {
            spawnRate = (int)(spawnRate * 1.1);
            maxSpawns = (int)(maxSpawns * 0.9f);
        }

        if (player.calmed) {
            spawnRate = (int)(spawnRate * 1.2);
            maxSpawns = (int)(maxSpawns * 0.8f);
        }

        if (Main.bloodMoon) {
            spawnRate = (int)(spawnRate * 1.2);
            maxSpawns = (int)(maxSpawns * 1.25f);
        }

        if (Main.eclipse) {
            spawnRate = (int)(spawnRate * 0.8);
            maxSpawns = (int)(maxSpawns * 1.25f);
        }

        // Zen used to cut the cap to 30% rather than 40%
        if (player.GetModPlayer<CalamityPlayer>().zen ||
            (CalamityServerConfig.Instance.ForceTownSafety && player.townNPCs > 1f && Main.expertMode)) {
            maxSpawns = (int)(maxSpawns * 0.75f);
        }
    }
}
