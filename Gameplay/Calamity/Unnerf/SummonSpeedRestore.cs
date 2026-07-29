using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * todo should this just be the non-consumable summons setting or this is good as a separate one?
 */
[JITWhenModsEnabled("CalamityMod")]
public class BossSummonRestore : GlobalItem {
    private static Dictionary<int, bool> summons = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.summonSpeed;
    }

    public override void Load() {
        summons = new Dictionary<int, bool> {
            { ItemID.Abeemination, false },
            { ItemID.BloodMoonStarter, true },
            { ItemID.BloodySpine, false },
            { ItemID.CelestialSigil, false },
            { ItemID.DeerThing, false },
            { ItemID.GoblinBattleStandard, true },
            { ItemID.MechanicalEye, false },
            { ItemID.MechanicalSkull, false },
            { ItemID.MechanicalWorm, false },
            { ItemID.MechdusaSummon, false },
            { ItemID.NaughtyPresent, true },
            { ItemID.PirateMap, true },
            { ItemID.PumpkinMoonMedallion, true },
            { ItemID.QueenSlimeCrystal, false },
            { ItemID.SlimeCrown, false },
            { ItemID.SnowGlobe, true },
            { ItemID.SolarTablet, true },
            { ItemID.SuspiciousLookingEye, false },
            { ItemID.WormFood, false }
        };
    }

    public override void SetDefaults(Item item) {
        if (!summons.TryGetValue(item.type, out var isEvent)) {
            return;
        }

        item.useTime = 10;

        if (isEvent) {
            item.maxStack = 1;
            item.consumable = false;
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class SummonStaffRestore : GlobalItem {
    private static Dictionary<int, int> speeds = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.summonSpeed;
    }

    public override void Load() {
        speeds = new Dictionary<int, int> {
            { ItemID.BabyBirdStaff, 35 },
            { ItemID.DD2BallistraTowerT2Popper, 25 },
            { ItemID.DD2BallistraTowerT3Popper, 20 },
            { ItemID.DD2ExplosiveTrapT2Popper, 25 },
            { ItemID.DD2ExplosiveTrapT3Popper, 20 },
            { ItemID.DD2FlameburstTowerT2Popper, 25 },
            { ItemID.DD2FlameburstTowerT3Popper, 20 },
            { ItemID.DD2LightningAuraT2Popper, 25 },
            { ItemID.DD2LightningAuraT3Popper, 20 },
            { ItemID.DeadlySphereStaff, 20 },
            { ItemID.EmpressBlade, 20 },
            { ItemID.FlinxStaff, 35 },
            { ItemID.HornetStaff, 30 },
            { ItemID.ImpStaff, 30 },
            { ItemID.MoonlordTurretStaff, 15 },
            { ItemID.OpticStaff, 25 },
            { ItemID.PirateStaff, 25 },
            { ItemID.PygmyStaff, 20 },
            { ItemID.QueenSpiderStaff, 25 },
            { ItemID.RainbowCrystalStaff, 15 },
            { ItemID.RavenStaff, 20 },
            { ItemID.SanguineStaff, 25 },
            { ItemID.SlimeStaff, 30 },
            { ItemID.Smolstar, 25 },
            { ItemID.SpiderStaff, 25 },
            { ItemID.StaffoftheFrostHydra, 20 },
            { ItemID.StardustCellStaff, 20 },
            { ItemID.StardustDragonStaff, 19 },
            { ItemID.StormTigerStaff, 20 },
            { ItemID.TempestStaff, 20 },
            { ItemID.VampireFrogStaff, 30 },
            { ItemID.XenoStaff, 20 }
        };
    }

    public override void SetDefaults(Item item) {
        if (speeds.TryGetValue(item.type, out var speed)) {
            item.useTime = speed;
            item.useAnimation = speed;
        }
    }
}
