using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;


// Thank you Terraria Overhaul!
internal sealed class SoyMasterMode : ModSystem {

    private static bool isEnabled;

    public override void Load() {
        updateMasterMode();
    }

    public override void Unload() {
        // Reset everything to vanilla
        if (isEnabled) {
            updateMasterMode(false);
        }
    }

    public override void PreUpdateEntities() {
        updateMasterMode();
    }

    private static void updateMasterMode() {
        isEnabled = QoLConfig.Instance.soyMasterMode;
        
        updateMasterMode(isEnabled);
    }

    private static void updateMasterMode(bool shouldBeEnabled) {
        // This will unfortunately reset any changes from other mods

        Span<GameModeData> presets = [
            GameModeData.NormalMode,
            GameModeData.ExpertMode,
            GameModeData.MasterMode,
            GameModeData.CreativeMode
        ];

        if (shouldBeEnabled) {
            updateMasterMode(presets);
        }

        for (int i = 0; i < presets.Length; i++) {
            Main.RegisteredGameModes[i] = presets[i];
        }

        Main.GameMode = Main.GameMode; // Reloads some stupid cache
        isEnabled = shouldBeEnabled;
    }

    private static void updateMasterMode(Span<GameModeData> presets) {
        ref var master = ref presets[GameModeID.Master];
        master.EnemyMaxLifeMultiplier = 2f;
        master.EnemyDamageMultiplier = 2f;
        master.DebuffTimeMultiplier = 2f;
        master.KnockbackToEnemiesMultiplier = 0.9f;
        master.TownNPCDamageMultiplier = 1.5f;
    }
}