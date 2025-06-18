using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class LifeFruit : ModSystem {
    public static bool orig1;
    public static bool orig2;

    public override void Load() {
        orig1 = Main.tileLighted[12];
        orig2 = Main.tileLighted[236];
        Main.tileLighted[12] = QoLConfig.Instance.lifeCrystalGlow;
        Main.tileLighted[236] = QoLConfig.Instance.lifeFruitGlow;
    }


    public override void Unload() {
        Main.tileLighted[12] = orig1;
        Main.tileLighted[236] = orig2;
    }
}

public class LifeFruitGlow : GlobalTile {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.lifeCrystalGlow || QoLConfig.Instance.lifeFruitGlow;
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b) {
        // strong
        float num8 = 1f + (270 - Main.mouseTextColor) / 400f;
        // weak
        float num9 = 0.8f - (270 - Main.mouseTextColor) / 400f;
        const float coef = 0.3f; // life crystals don't glow that much, that would be too easy
        if (type == TileID.Heart && QoLConfig.Instance.lifeCrystalGlow) {
            r = 0.9f * num8 * coef;
            g = 0.15f * num9 * coef;
            b = 0.3f * num9 * coef;
        }

        if (type == TileID.LifeFruit && QoLConfig.Instance.lifeFruitGlow) {
            r = 0.1f * num9;
            g = 0.9f * num8;
            b = 0.3f * num9;
        }
    }
}