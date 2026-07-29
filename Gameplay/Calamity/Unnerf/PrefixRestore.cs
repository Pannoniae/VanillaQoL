using CalamityMod;
using CalamityMod.Prefixes.VanillaPrefixChanges;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class PrefixRestore : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void PostSetupContent() {
        foreach (var prefix in new[] { PrefixID.Hard, PrefixID.Guarding, PrefixID.Armored, PrefixID.Warding }) {
            if (!VanillaPrefixChangeSystem.PrefixChanges.Remove(prefix)) {
                VanillaQoL.instance.Logger.Warn(
                    $"Haven't found Calamity's{prefix} rework!");
            }
        }
    }
}

/// Copypaste
public static class ProgressionScaling {
    public static int get() {
        if (DownedBossSystem.downedDoG) {
            return 6;
        }

        if (NPC.downedMoonlord) {
            return 4;
        }

        return Main.hardMode ? 2 : 0;
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class PrefixScalingPlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void UpdateEquips() {
        if (Player.HasBuff(BuffID.Ironskin)) {
            Player.statDefense += 2 * ProgressionScaling.get();
        }
    }

}

[JITWhenModsEnabled("CalamityMod")]
public class PrefixScalingItem : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        var scale = ProgressionScaling.get();

        switch (item.prefix) {
            case PrefixID.Hard:
                player.statDefense += scale;
                player.endurance += 0.0025f;
                break;
            case PrefixID.Guarding:
                player.statDefense += scale;
                player.endurance += 0.005f;
                break;
            case PrefixID.Armored:
                player.statDefense += scale;
                player.endurance += 0.0075f;
                break;
            case PrefixID.Warding:
                player.statDefense += scale;
                player.endurance += 0.01f;
                break;
            case PrefixID.Lucky:
                player.luck += 0.05f;
                break;
        }
    }
}
