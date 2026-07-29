using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace VanillaQoL.Config;

/// Undoing Calamity's "vanilla changes" (https://calamitymod.wiki.gg/wiki/Vanilla_changes)
[BackgroundColor(16, 0, 2, 1)]
[EnabledIf(typeof(CalamityLoaded))]
public class CalamityUnnerfConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618
    public static CalamityUnnerfConfig Instance;
#pragma warning restore CS8618

    public override ConfigScope Mode => ConfigScope.ServerSide;

    #region Systems

    [Header("systems")]

    // Cobalt Shield
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool defenseDamage { get; set; }

    // Worm Scarf
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool damageReduction { get; set; }

    // Ale
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool buffs { get; set; }

    // Beetle Husk
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool meleeArmourDR { get; set; }

    // Vampire Knives
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool lifesteal { get; set; }

    // Shield of Cthulhu
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool bossDR { get; set; }

    // Shield of Cthulhu, same as its vanilla sibling above
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool calamityBossDR { get; set; }

    // Daedalus Stormbow
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool pierceResist { get; set; }

    // DPS Meter
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool damageVariance { get; set; }

    // Life Crystal
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool multiplayerBossHealth { get; set; }

    // Cross Necklace
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool immunityFrames { get; set; }

    // Ichor
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ichorDefence { get; set; }

    // Night's Edge
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool meleeResist { get; set; }

    #endregion

    #region Items

    [Header("items")]

    // Soaring Insignia
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool soaringInsignia { get; set; }

    // Shadow Scale
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shadowArmour { get; set; }

    // Black Belt
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool blackBelt { get; set; }

    // Brain of Confusion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool brainOfConfusion { get; set; }

    // Meteor Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool meteorArmour { get; set; }

    // Yo-yo Glove
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool yoyoGlove { get; set; }

    // Jungle Hat
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool jungleArmour { get; set; }

    // Frost Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool frostArmour { get; set; }

    // Mini Nuke I
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool vanillaRecipes { get; set; }

    // Morning Star
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool whipTagCrits { get; set; }

    // Terrarian
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool terrarianProjectiles { get; set; }

    // Nebula Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool nebulaArmour { get; set; }

    // Fire Gauntlet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool meleeSpeedStacking { get; set; }

    // Asphalt Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool asphaltAndSkates { get; set; }

    // Sniper Scope
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool scopes { get; set; }

    // Spectre Staff
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool spectreAndVampire { get; set; }

    // Shiny Red Balloon
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool balloonJump { get; set; }

    // Beetle Scale Mail
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool beetleMight { get; set; }

    // Pygmy Staff
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool summonerCrossClass { get; set; }

    // Monk's Bushy Brow Bald Cap
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool classConversions { get; set; }

    // Hallowed Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool hallowedDodge { get; set; }

    // Shield of Cthulhu
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shieldBonkSafety { get; set; }

    // Butterfly Wings
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool wingAscent { get; set; }

    #endregion

    #region Progression

    [Header("progression")]

    // Rod of Discord
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shimmerGating { get; set; }

    // Blood Moon Starter
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool bloodMoonRequirement { get; set; }

    // Desert Fossil
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool fossilShattering { get; set; }

    #endregion

    #region Restored

    // these are NOT nerfs to vanilla - these are things Calamity used to do before they removed it.
    // To keep things simple, I'm using Calamity 2.0.4.006 as the reference

    [Header("restored")]

    // Ankh Shield
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shieldImmunities { get; set; }

    // Worm Scarf
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool expertDrops { get; set; }

    // Bomb
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool starterBag { get; set; }

    // Vortex Fragment, standing in because the Cosmolight is Calamity's own
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool earlyCosmolight { get; set; }

    // Fisherman's Guide
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool anglingKits { get; set; }

    // Rod of Discord
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shopStock { get; set; }

    // Wooden Crate
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool crateLoot { get; set; }

    // Luminite, standing in because the Astral Infection is Calamity's own
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool astralDrops { get; set; }

    // Life Crystal
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool autoswing { get; set; }

    // Cobalt Shield
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool defenceBonuses { get; set; }

    // Medusa Head
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool manaDiscounts { get; set; }

    // Angel Wings
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool wingBonuses { get; set; }

    // Obsidian Skull
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool fireImmunity { get; set; }

    // Optic Staff
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool summonSpeed { get; set; }

    // Reaver Shark
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool toolPower { get; set; }

    // Adamantite Breastplate
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool armourBonuses { get; set; }

    // Warding, no icon for a prefix so the Ankh Shield stands in
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool prefixScaling { get; set; }

    // Plague, standing in with the Vial of Venom
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool sicknessContactDamage { get; set; }

    // Gold Coin
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool nurseAndTaxes { get; set; }

    // Battle Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool spawnRates { get; set; }

    // Platinum Coin
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool sellPrices { get; set; }

    #endregion

    #region Item stats

    // these are wholesale reverts to vanilla, buffs and all, so they're all off by default unlike the rest

    [Header("itemStats")]

    // Excalibur
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemDamage { get; set; }

    // Eye of the Golem
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemCrit { get; set; }

    // Turtle Helmet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemDefence { get; set; }

    // Magic Cuffs
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemMana { get; set; }

    // Feral Claws
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemSpeed { get; set; }

    // Musket Ball
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemShootSpeed { get; set; }

    // Power Glove
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemKnockback { get; set; }

    // Titan Glove
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemScale { get; set; }

    // Lucky Coin
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemValue { get; set; }

    // Molten Pickaxe
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemToolPower { get; set; }

    // Toolbelt
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool itemBehaviour { get; set; }

    // Shroomite Breastplate
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool vanillaEquipStats { get; set; }

    #endregion

    #region Projectile stats

    [Header("projectileStats")]

    // Crystal Bullet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projPiercing { get; set; }

    // High Velocity Bullet
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projArmourPen { get; set; }

    // Swiftness Potion
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projSpeed { get; set; }

    // Magic Quiver
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projLifetime { get; set; }

    // Shadowbeam Staff
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projScale { get; set; }

    // Yelets
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projYoyoStats { get; set; }

    // Neptune's Shell
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projCollision { get; set; }

    // Night's Edge
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool projTrueMelee { get; set; }

    #endregion
}
