using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class DefenseDamageUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.defenseDamage;
    }

    public override void OnModLoad() {
        CalamityMod.CalamityMod.ExternalFlag_DisableDefenseDamage = true;
    }

    public override void Unload() {
        CalamityMod.CalamityMod.ExternalFlag_DisableDefenseDamage = false;
    }
}

/**
 * We remove DR from the vanilla bosses. TODO maybe from the Cal ones too? not sure
 */
[JITWhenModsEnabled("CalamityMod")]
public class BossDRUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() &&
               (CalamityUnnerfConfig.Instance.bossDR || CalamityUnnerfConfig.Instance.calamityBossDR);
    }

    public override void PostSetupContent() {
        var drValues = CalamityGlobalNPC.DRValues;
        if (drValues == null) {
            VanillaQoL.instance.Logger.Warn("CalamityGlobalNPC.DRValues was null!");
            return;
        }

        if (CalamityUnnerfConfig.Instance.bossDR) {
            strip(drValues, type => type < NPCID.Count, "vanilla");
        }

        if (CalamityUnnerfConfig.Instance.calamityBossDR) {
            strip(drValues, isCalamitys, "Calamity");
        }
    }

    private static bool isCalamitys(int type) {
        return type >= NPCID.Count && NPCLoader.GetNPC(type)?.Mod.Name == "CalamityMod";
    }

    private static void strip(SortedDictionary<int, float> drValues, Func<int, bool> matches, string whose) {
        var doomed = drValues.Keys.Where(matches).ToList();
        foreach (var type in doomed) {
            drValues.Remove(type);
        }

        VanillaQoL.instance.Logger.Info($"Took Calamity's bonus DR off {doomed.Count} {whose} NPCs.");
    }
}


/**
 * Calamity rescales boss health in multiplayer, 1.75x for two players and 2.25x for three (vanilla 1.35x and 1.9166x).
 * So we just cancel it out!
 */
[JITWhenModsEnabled("CalamityMod")]
public class MultiplayerHealthUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.multiplayerBossHealth;
    }

    public override void OnModLoad() {
        BalancingConstants.ExpertHealthScalingOverride_2Players = vanillaBalance(2);
        BalancingConstants.ExpertHealthScalingOverride_3Players = vanillaBalance(3);
    }

    public override void Unload() {
        BalancingConstants.ExpertHealthScalingOverride_2Players = 1.75f;
        BalancingConstants.ExpertHealthScalingOverride_3Players = 2.25f;
    }

    private static float vanillaBalance(int players) {
        NPC.GetStatScalingFactors(players, out var balance, out _);
        return balance;
    }
}


[JITWhenModsEnabled("CalamityMod")]
public class PierceResistUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.pierceResist;
    }

    public override void PostSetupContent() {
        var field = typeof(PierceResistNPC).GetField("pierceResistNPC",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is not HashSet<int> resistant) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find PierceResistNPC.pierceResistNPC!");
            return;
        }

        var removed = resistant.RemoveWhere(type => type < NPCID.Count);
        VanillaQoL.instance.Logger.Info($"Removed pierce resistance from {removed} vanilla NPCs.");
    }
}
