using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace VanillaQoL.Config;

public class QoLRecipeConfig : ModConfig {
    
    // Field automagically set by tML
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static QoLRecipeConfig Instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public override ConfigScope Mode => ConfigScope.ServerSide;
    
    #region Recipes

    [Header("recipes")]
    
    // Temple Trap
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool templeTraps { get; set; }

    // Blue Dungeon Brick
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool dungeonFurniture { get; set; }
    
    // Obsidian Chandelier1
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool obsidianFurniture { get; set; }
    
    // Clothier Voodoo Doll
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool clothierVoodooDoll { get; set; }
    
    // Sun Banner
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool bannerRecipes { get; set; }
    
    // Pink Team Block
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool teamBlocks { get; set; }
    #endregion

    #region Shimmer

    [Header("shimmer")]
    
    // Black Lens
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shimmerBlackLens { get; set; }

    // Orange Zapinator
    [BackgroundColor(192, 54, 128, 192)]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool shimmerGuns { get; set; }

    #endregion
}