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

public static class ProgressionScaling {
    // base, hardmode, Golem, Moon Lord, Providence, Polterghast, Devourer, Yharon
    private static readonly int[] hard = [1, 2, 2, 2, 2, 3, 3, 4];
    private static readonly int[] guarding = [2, 3, 4, 4, 5, 5, 6, 7];
    private static readonly int[] armored = [3, 5, 5, 6, 7, 8, 9, 10];
    private static readonly int[] warding = [4, 6, 7, 8, 9, 10, 11, 12];

    // Ironskin only ever had four tiers so let's make things up
    private static readonly int[] ironskin = [8, 12, 12, 16, 16, 16, 20, 20];

    public static int tier() {
        if (DownedBossSystem.downedYharon) {
            return 7;
        }

        if (DownedBossSystem.downedDoG) {
            return 6;
        }

        if (DownedBossSystem.downedPolterghast) {
            return 5;
        }

        if (DownedBossSystem.downedProvidence) {
            return 4;
        }

        if (NPC.downedMoonlord) {
            return 3;
        }

        if (NPC.downedGolemBoss) {
            return 2;
        }

        return Main.hardMode ? 1 : 0;
    }

    public static int ironskinBonus() {
        return ironskin[tier()] - ironskin[0];
    }

    public static (int defence, float dr) prefixStats(int prefix) {
        var rung = tier();

        return prefix switch {
            PrefixID.Hard => (hard[rung], 0.0025f),
            PrefixID.Guarding => (guarding[rung], 0.005f),
            PrefixID.Armored => (armored[rung], 0.0075f),
            PrefixID.Warding => (warding[rung], 0.01f),
            _ => (0, 0f)
        };
    }

    public static int prefixBonus(int prefix) {
        return prefix switch {
            PrefixID.Hard => prefixStats(prefix).defence - hard[0],
            PrefixID.Guarding => prefixStats(prefix).defence - guarding[0],
            PrefixID.Armored => prefixStats(prefix).defence - armored[0],
            PrefixID.Warding => prefixStats(prefix).defence - warding[0],
            _ => 0
        };
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class PrefixScalingPlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void UpdateEquips() {
        if (Player.HasBuff(BuffID.Ironskin)) {
            Player.statDefense += ProgressionScaling.ironskinBonus();
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class PrefixScalingItem : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.prefixScaling;
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        if (item.prefix == PrefixID.Lucky) {
            player.luck += 0.05f;
            return;
        }

        var (defence, dr) = ProgressionScaling.prefixStats(item.prefix);
        if (defence == 0) {
            return;
        }

        player.statDefense += ProgressionScaling.prefixBonus(item.prefix);
        player.endurance += dr;
    }
}
