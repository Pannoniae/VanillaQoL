using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity used to give autoswing to 153 vanilla items that never had it. Then removed it on the grounds that 1.4.4 has an "autoswing on all weapons and tools" setting now. Which is true, and which
 * does absolutely nothing for the Life Crystal, the Life Fruit, the Mana Crystal or a stack of paper aeroplanes. What the fuck.
 */
[JITWhenModsEnabled("CalamityMod")]
public class AutoswingRestore : GlobalItem {
    private static HashSet<int> swingers = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.autoswing;
    }

    public override void Load() {
        swingers = [
            ItemID.AdamantiteGlaive, ItemID.BloodyMachete, ItemID.BluePhaseblade, ItemID.BreakerBlade, ItemID.Cascade,
            ItemID.ChainKnife, ItemID.ChlorophytePartisan, ItemID.ChristmasTreeSword, ItemID.CobaltNaginata,
            ItemID.Code2, ItemID.CorruptYoyo, ItemID.CrimsonYoyo, ItemID.DarkLance, ItemID.DemonBow,
            ItemID.DemonScythe, ItemID.DyeTradersScimitar, ItemID.EmpressBlade, ItemID.FlowerofFire,
            ItemID.FlowerofFrost, ItemID.GoldShortsword, ItemID.Gradient, ItemID.GreenPhaseblade, ItemID.Gungnir,
            ItemID.HiveFive, ItemID.HornetStaff, ItemID.IceSickle, ItemID.ImpStaff, ItemID.IronShortsword,
            ItemID.Kraken, ItemID.LeadShortsword, ItemID.MoltenFury, ItemID.MonkStaffT2, ItemID.MushroomSpear,
            ItemID.MythrilHalberd, ItemID.NorthPole, ItemID.OrangePhaseblade, ItemID.OrichalcumHalberd,
            ItemID.PalladiumPike, ItemID.PearlwoodBow, ItemID.PhoenixBlaster, ItemID.PlatinumShortsword,
            ItemID.PurplePhaseblade, ItemID.PygmyStaff, ItemID.Rally, ItemID.RavenStaff, ItemID.RedPhaseblade,
            ItemID.RedsYoyo, ItemID.Shotgun, ItemID.SilverShortsword, ItemID.Smolstar, ItemID.Spear,
            ItemID.StardustDragonStaff, ItemID.StormTigerStaff, ItemID.StylistKilLaKillScissorsIWish,
            ItemID.Swordfish, ItemID.TaxCollectorsStickOfDoom, ItemID.TendonBow, ItemID.Terrarian,
            ItemID.TheEyeOfCthulhu, ItemID.TheRottedFork, ItemID.TheUndertaker, ItemID.ThunderSpear,
            ItemID.TitaniumTrident, ItemID.Trident, ItemID.TungstenShortsword, ItemID.ValkyrieYoyo,
            ItemID.WhitePhaseblade, ItemID.Yelets, ItemID.YellowPhaseblade, ItemID.ZombieArm,
            ItemID.CopperShortsword, ItemID.Gladius, ItemID.ObsidianSwordfish, ItemID.TinShortsword,
            ItemID.Revolver, ItemID.AbigailsFlower, ItemID.BabyBirdStaff, ItemID.BlandWhip, ItemID.BoneWhip,
            ItemID.CoolWhip, ItemID.DD2BallistraTowerT1Popper, ItemID.DD2BallistraTowerT2Popper,
            ItemID.DD2BallistraTowerT3Popper, ItemID.DD2ExplosiveTrapT1Popper, ItemID.DD2ExplosiveTrapT2Popper,
            ItemID.DD2ExplosiveTrapT3Popper, ItemID.DD2FlameburstTowerT1Popper, ItemID.DD2FlameburstTowerT2Popper,
            ItemID.DD2FlameburstTowerT3Popper, ItemID.DD2LightningAuraT1Popper, ItemID.DD2LightningAuraT2Popper,
            ItemID.DD2LightningAuraT3Popper, ItemID.DeadlySphereStaff, ItemID.FireWhip, ItemID.FlinxStaff,
            ItemID.MaceWhip, ItemID.OpticStaff, ItemID.PirateStaff, ItemID.RainbowWhip, ItemID.SanguineStaff,
            ItemID.ScytheWhip, ItemID.SlimeStaff, ItemID.SpiderStaff, ItemID.StardustCellStaff, ItemID.SwordWhip,
            ItemID.TempestStaff, ItemID.ThornWhip, ItemID.VampireFrogStaff, ItemID.XenoStaff, ItemID.Amarok,
            ItemID.BatBat, ItemID.BladeofGrass, ItemID.BloodButcherer, ItemID.BoneSword, ItemID.BorealWoodSword,
            ItemID.CactusSword, ItemID.CandyCaneSword, ItemID.Chik, ItemID.Code1, ItemID.CopperBroadsword,
            ItemID.EbonwoodSword, ItemID.FieryGreatsword, ItemID.FormatC, ItemID.GoldBroadsword, ItemID.HelFire,
            ItemID.IronBroadsword, ItemID.JungleYoyo, ItemID.LifeCrystal, ItemID.LeadBroadsword, ItemID.LifeFruit,
            ItemID.LightsBane, ItemID.ManaCrystal, ItemID.PalmWoodSword, ItemID.PaperAirplaneA,
            ItemID.PaperAirplaneB, ItemID.PlatinumBroadsword, ItemID.RichMahoganySword, ItemID.ShadewoodSword,
            ItemID.SilverBroadsword, ItemID.Starfury, ItemID.TentacleSpike, ItemID.TinBroadsword,
            ItemID.TragicUmbrella, ItemID.TungstenBroadsword, ItemID.Umbrella, ItemID.Valor, ItemID.WandofFrosting,
            ItemID.WandofSparking, ItemID.WeatherPain, ItemID.WoodenSword, ItemID.WoodYoyo, ItemID.ZapinatorGray,
            ItemID.ZapinatorOrange
        ];
    }

    public override void SetDefaults(Item item) {
        if (swingers.Contains(item.type)) {
            item.autoReuse = true;
        }
    }
}
