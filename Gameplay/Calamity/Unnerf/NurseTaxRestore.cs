using System;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class NurseRestore : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.nurseAndTaxes;
    }

    public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price) {
        if (price <= 0) {
            return;
        }

        price -= Item.buyPrice(0, 0, 3, 0);
        price += tier();

        if (CalamityPlayer.areThereAnyDamnBosses) {
            price *= 5;
        }
    }

    private static int tier() {
        if (DownedBossSystem.downedYharon) {
            return Item.buyPrice(0, 9, 0, 0);
        }

        if (DownedBossSystem.downedDoG) {
            return Item.buyPrice(0, 6, 0, 0);
        }

        if (DownedBossSystem.downedProvidence) {
            return Item.buyPrice(0, 3, 20, 0);
        }

        if (NPC.downedMoonlord) {
            return Item.buyPrice(0, 2, 0, 0);
        }

        if (NPC.downedFishron || DownedBossSystem.downedPlaguebringer || DownedBossSystem.downedRavager) {
            return Item.buyPrice(0, 1, 20, 0);
        }

        if (NPC.downedGolemBoss) {
            return Item.buyPrice(0, 0, 90, 0);
        }

        if (NPC.downedPlantBoss || DownedBossSystem.downedCalamitasClone) {
            return Item.buyPrice(0, 0, 60, 0);
        }

        if (NPC.downedMechBossAny) {
            return Item.buyPrice(0, 0, 40, 0);
        }

        if (Main.hardMode) {
            return Item.buyPrice(0, 0, 24, 0);
        }

        if (NPC.downedBoss3) {
            return Item.buyPrice(0, 0, 12, 0);
        }

        return NPC.downedBoss1 ? Item.buyPrice(0, 0, 6, 0) : Item.buyPrice(0, 0, 3, 0);
    }

    public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText) {
        if ((CalamityWorld.death || BossRushEvent.BossRushActive) && CalamityPlayer.areThereAnyDamnBosses) {
            chatText = Language.GetTextValue("Mods.VanillaQoL.NurseChat.HealNotAllowed");
            return false;
        }

        return true;
    }
}

/**
 * Their price ladder has to go before ours means anything, and it's a single method that does nothing else.
 * The Tax Collector's two numbers are static properties Calamity's own IL edit already reads, so we just answer
 * differently: 1s50c a head instead of 1s, and the old post-Plantera curve that kept climbing past the Devourer.
 */
[JITWhenModsEnabled("CalamityMod")]
public class NurseTaxPatch : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.nurseAndTaxes;
    }

    public override void OnModLoad() {
        var price = typeof(CalamityPlayer).GetMethod(nameof(CalamityPlayer.ModifyNursePrice));
        if (price == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityPlayer.ModifyNursePrice, so the Nurse charges twice now. Sorry.");
        }
        else {
            MonoModHooks.Modify(price, dontTouchThePrice);
        }

        hook(nameof(CalamityGlobalTownNPC.TotalTaxesPerNPC), () => (int)(Item.buyPrice(0, 0, 1, 50) * yield()));
        hook(nameof(CalamityGlobalTownNPC.TaxesToCollectLimit), () => (int)(Item.buyPrice(0, 50, 0, 0) * yield()));
    }

    private static void hook(string property, Func<int> replacement) {
        var getter = typeof(CalamityGlobalTownNPC).GetProperty(property)?.GetGetMethod();
        if (getter == null) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find CalamityGlobalTownNPC.{property}, taxes stay cut.");
            return;
        }

        MonoModHooks.Add(getter, (Func<Func<int>, int>)(_ => replacement()));
    }

    private static float yield() {
        if (DownedBossSystem.downedYharon) {
            return 40f;
        }

        if (DownedBossSystem.downedDoG) {
            return 20f;
        }

        if (NPC.downedMoonlord) {
            return 10f;
        }

        return NPC.downedPlantBoss ? 4f : 1f;
    }

    private void dontTouchThePrice(ILContext il) {
        new ILCursor(il).EmitRet();
    }
}
