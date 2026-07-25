using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using VanillaQoL.API;

namespace VanillaQoL.Config;

/// Calamity's QoL changes, for people who don't have Calamity. (used to be Calamity QOL for Vanilla)
[BackgroundColor(16, 0, 2, 1)]
[EnabledIf(typeof(CalamityAbsent))]
public class CalamityQoLConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618
    public static CalamityQoLConfig Instance;
#pragma warning restore CS8618

    public override ConfigScope Mode => ConfigScope.ServerSide;

    // greying out the page is only cosmetic, the values are still whatever they were before you installed Cal.
    // so we still need to check....
    public static bool active => !ModLoader.HasMod("CalamityMod");

    #region Gameplay

    [Header("gameplay")]

    // Pumpkin Pie
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool fullExpertRegen { get; set; }

    // Goodie Bag
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool starterBag { get; set; }

    #endregion

    #region Player boosts

    [Header("playerBoosts")]

    // Anklet of the Wind
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fasterMovement { get; set; }

    // Frog Leg
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool higherJumpHeight { get; set; }

    // Shiny Red Balloon
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fasterJumpSpeed { get; set; }

    // Brick Layer
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fasterTilePlacement { get; set; }

    // Lead Anvil
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    public bool fasterFall { get; set; }

    #endregion

    #region Recipes

    [Header("recipes")]

    // Chlorophyte Bar
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool vanillaRecipeTweaks { get; set; }

    // Worm Scarf
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool evilBiomeSwaps { get; set; }

    // Starfury
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool earlyWeaponRecipes { get; set; }

    // Hermes Boots
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool accessoryRecipes { get; set; }

    // Eskimo Coat
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool armourRecipes { get; set; }

    // Suspicious Looking Eye
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool summonRecipes { get; set; }

    // Encumbering Stone
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool miscRecipes { get; set; }

    // Bottomless Shimmer Bucket
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shimmerRecipes { get; set; }

    #endregion
}
