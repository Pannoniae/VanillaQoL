using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class ToolRestore : GlobalItem {
    private record struct Tool(
        int? pick = null,
        int? axe = null,
        int? hammer = null,
        int? tileBoost = null,
        int? useTime = null,
        int? damage = null);

    private static Dictionary<int, Tool> tools = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.toolPower;
    }

    public override void Load() {
        tools = new Dictionary<int, Tool> {
            { ItemID.AcornAxe, new(axe: 125) },
            { ItemID.AdamantiteChainsaw, new(useTime: 4, damage: 75) },
            { ItemID.AdamantiteDrill, new(pick: 180, useTime: 4, tileBoost: 1, damage: 32) },
            { ItemID.AdamantitePickaxe, new(pick: 180, useTime: 8) },
            { ItemID.BloodLustCluster, new(tileBoost: 0) },
            { ItemID.BonePickaxe, new(pick: 55) },
            { ItemID.BorealWoodHammer, new(tileBoost: 0) },
            { ItemID.ButchersChainsaw, new(axe: 150, damage: 177) },
            { ItemID.CactusPickaxe, new(pick: 34) },
            { ItemID.CnadyCanePickaxe, new(pick: 55) },
            { ItemID.ChlorophyteChainsaw, new(tileBoost: 0, damage: 112) },
            { ItemID.ChlorophyteDrill, new(pick: 200, useTime: 4, tileBoost: 2, damage: 43) },
            { ItemID.ChlorophyteGreataxe, new(useTime: 7) },
            { ItemID.ChlorophyteJackhammer, new(hammer: 90, tileBoost: 0) },
            { ItemID.ChlorophytePickaxe, new(pick: 200, useTime: 7) },
            { ItemID.ChlorophyteWarhammer, new(hammer: 90) },
            { ItemID.CobaltChainsaw, new(axe: 70, tileBoost: -1, damage: 51) },
            { ItemID.CobaltDrill, new(damage: 21) },
            { ItemID.CobaltWaraxe, new(tileBoost: 0) },
            { ItemID.CopperHammer, new(hammer: 35) },
            { ItemID.CopperPickaxe, new(pick: 35) },
            { ItemID.DeathbringerPickaxe, new(pick: 70) },
            { ItemID.Drax, new(pick: 200, axe: 110, useTime: 4, tileBoost: 1, damage: 80) },
            { ItemID.EbonwoodHammer, new(tileBoost: 0) },
            { ItemID.FleshGrinder, new(tileBoost: 0) },
            { ItemID.GoldAxe, new(tileBoost: 0) },
            { ItemID.GoldHammer, new(tileBoost: 0) },
            { ItemID.GoldPickaxe, new(pick: 55) },
            { ItemID.Hammush, new(hammer: 85) },
            { ItemID.IronAxe, new(tileBoost: 0) },
            { ItemID.IronHammer, new(tileBoost: 0) },
            { ItemID.IronPickaxe, new(pick: 40) },
            { ItemID.LaserDrill, new(axe: 120, damage: 54) },
            { ItemID.LeadAxe, new(tileBoost: 0) },
            { ItemID.LeadHammer, new(tileBoost: 0) },
            { ItemID.LucyTheAxe, new(axe: 150) },
            { ItemID.LunarHamaxeNebula, new(hammer: 100, tileBoost: 4) },
            { ItemID.LunarHamaxeSolar, new(hammer: 100, tileBoost: 4) },
            { ItemID.LunarHamaxeStardust, new(hammer: 100, tileBoost: 4) },
            { ItemID.LunarHamaxeVortex, new(hammer: 100, tileBoost: 4) },
            { ItemID.MeteorHamaxe, new(axe: 100, useTime: 16, tileBoost: 0) },
            { ItemID.MoltenHamaxe, new(useTime: 14, tileBoost: 0) },
            { ItemID.MoltenPickaxe, new(pick: 100) },
            { ItemID.MythrilChainsaw, new(tileBoost: -1, damage: 63) },
            { ItemID.MythrilDrill, new(damage: 26) },
            { ItemID.MythrilWaraxe, new(tileBoost: 0) },
            { ItemID.NebulaDrill, new(pick: 225, tileBoost: 4, damage: 95) },
            { ItemID.NebulaPickaxe, new(pick: 225, useTime: 6, tileBoost: 4) },
            { ItemID.OrichalcumChainsaw, new(tileBoost: -1, damage: 63) },
            { ItemID.OrichalcumDrill, new(damage: 26) },
            { ItemID.OrichalcumWaraxe, new(tileBoost: 0) },
            { ItemID.PalladiumChainsaw, new(tileBoost: -1, damage: 51) },
            { ItemID.PalladiumDrill, new(pick: 130, damage: 21) },
            { ItemID.PalladiumPickaxe, new(pick: 130) },
            { ItemID.PalladiumWaraxe, new(useTime: 12, tileBoost: 0) },
            { ItemID.PalmWoodHammer, new(tileBoost: 0) },
            { ItemID.PearlwoodHammer, new(tileBoost: 0) },
            { ItemID.PickaxeAxe, new(pick: 200, axe: 110, useTime: 7) },
            { ItemID.Picksaw, new(pick: 210, axe: 125, useTime: 6, tileBoost: 1) },
            { ItemID.PlatinumAxe, new(tileBoost: 0) },
            { ItemID.PlatinumHammer, new(tileBoost: 0) },
            { ItemID.Pwnhammer, new(hammer: 80) },
            { ItemID.ReaverShark, new(pick: 100, useTime: 16) },
            { ItemID.RichMahoganyHammer, new(tileBoost: 0) },
            { ItemID.Rockfish, new(tileBoost: 0) },
            { ItemID.SawtoothShark, new(useTime: 4, tileBoost: -1) },
            { ItemID.ShadewoodHammer, new(tileBoost: 0) },
            { ItemID.ShroomiteDiggingClaw, new(pick: 200, axe: 125, useTime: 4, tileBoost: -1) },
            { ItemID.SilverAxe, new(tileBoost: 0) },
            { ItemID.SilverHammer, new(tileBoost: 0) },
            { ItemID.SilverPickaxe, new(useTime: 11) },
            { ItemID.SolarFlareDrill, new(pick: 225, tileBoost: 4, damage: 95) },
            { ItemID.SolarFlarePickaxe, new(pick: 225, useTime: 6, tileBoost: 4) },
            { ItemID.SpectreHamaxe, new(hammer: 90) },
            { ItemID.SpectrePickaxe, new(pick: 200) },
            { ItemID.StardustDrill, new(pick: 225, tileBoost: 4, damage: 95) },
            { ItemID.StardustPickaxe, new(pick: 225, useTime: 6, tileBoost: 4) },
            { ItemID.TheAxe, new(hammer: 100, axe: 175, useTime: 7, tileBoost: 1) },
            { ItemID.TheBreaker, new(tileBoost: 0) },
            { ItemID.TinAxe, new(tileBoost: 0) },
            { ItemID.TinHammer, new(tileBoost: 0) },
            { ItemID.TinPickaxe, new(pick: 35, tileBoost: 0) },
            { ItemID.TitaniumChainsaw, new(useTime: 4, damage: 75) },
            { ItemID.TitaniumDrill, new(useTime: 4, tileBoost: 1, damage: 32) },
            { ItemID.TungstenAxe, new(tileBoost: 0) },
            { ItemID.TungstenHammer, new(tileBoost: 0) },
            { ItemID.TungstenPickaxe, new(pick: 50) },
            { ItemID.VortexDrill, new(pick: 225, tileBoost: 4, damage: 95) },
            { ItemID.VortexPickaxe, new(pick: 225, useTime: 6, tileBoost: 4) },
            { ItemID.WarAxeoftheNight, new(tileBoost: 0) },
            { ItemID.WoodenHammer, new(hammer: 25) }
        };
    }

    public override void SetDefaults(Item item) {
        if (!tools.TryGetValue(item.type, out var tool)) {
            return;
        }

        if (tool.pick is { } pick) {
            item.pick = pick;
        }

        if (tool.axe is { } axe) {
            item.axe = axe / 5;
        }

        if (tool.hammer is { } hammer) {
            item.hammer = hammer;
        }

        if (tool.tileBoost is { } tileBoost) {
            item.tileBoost = tileBoost;
        }

        if (tool.useTime is { } useTime) {
            item.useTime = useTime;
        }

        if (tool.damage is { } damage) {
            item.damage = damage;
        }
    }
}
