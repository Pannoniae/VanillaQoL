using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class Recipes : ModSystem {
    public override void SetStaticDefaults() {
        // shimmering stuff
        if (QoLRecipeConfig.Instance.shimmerBlackLens) {
            shimmerLens();
        }

        if (QoLRecipeConfig.Instance.shimmerGuns) {
            shimmerGuns();
        }
    }

    public override void AddRecipes() {
        if (QoLRecipeConfig.Instance.templeTraps) {
            addTempleTraps();
        }

        if (QoLRecipeConfig.Instance.dungeonFurniture) {
            addDungeonFurniture();
        }

        if (QoLRecipeConfig.Instance.obsidianFurniture) {
            addObsidianFurniture();
        }

        if (QoLRecipeConfig.Instance.clothierVoodooDoll) {
            addClothierVoodooDoll();
        }

        if (QoLRecipeConfig.Instance.bannerRecipes) {
            addBannerRecipes();
        }

        if (QoLRecipeConfig.Instance.teamBlocks) {
            addTeamBlocks();
        }
    }

    private static void addTeamBlocks() {
        addTeamBlock(ItemID.TeamBlockWhite, ItemID.SilverDye);
        addTeamPlatform(ItemID.TeamBlockWhite, ItemID.TeamBlockWhitePlatform);

        addTeamBlock(ItemID.TeamBlockRed, ItemID.RedDye);
        addTeamPlatform(ItemID.TeamBlockRed, ItemID.TeamBlockRedPlatform);

        addTeamBlock(ItemID.TeamBlockBlue, ItemID.BlueDye);
        addTeamPlatform(ItemID.TeamBlockBlue, ItemID.TeamBlockBluePlatform);

        addTeamBlock(ItemID.TeamBlockGreen, ItemID.GreenDye);
        addTeamPlatform(ItemID.TeamBlockGreen, ItemID.TeamBlockGreenPlatform);

        addTeamBlock(ItemID.TeamBlockYellow, ItemID.YellowDye);
        addTeamPlatform(ItemID.TeamBlockYellow, ItemID.TeamBlockYellowPlatform);

        addTeamBlock(ItemID.TeamBlockPink, ItemID.PinkDye);
        addTeamPlatform(ItemID.TeamBlockPink, ItemID.TeamBlockPinkPlatform);
    }

    private static void addTeamBlock(int type, int dye) {
        Recipe.Create(type, 25)
            .AddIngredient(ItemID.StoneBlock, 25)
            .AddIngredient(dye)
            .AddTile(TileID.DyeVat)
            .Register();
    }

    private static void addTeamPlatform(int type, int dye) {
        Recipe.Create(dye, 2)
            .AddIngredient(type)
            .Register();

        Recipe.Create(type)
            .AddIngredient(dye, 2)
            .Register();
    }

    private static void addClothierVoodooDoll() {
        Recipe.Create(ItemID.ClothierVoodooDoll)
            .AddIngredient(ItemID.GuideVoodooDoll)
            .AddIngredient(ItemID.RedHat)
            .AddTile(TileID.DemonAltar)
            .Register();
    }

    public override void AddRecipeGroups() {
        string anyDungeonBrick = Language.GetTextValue("Mods.VanillaQoL.RecipeGroups.DungeonBricks");
        RecipeGroup.RegisterGroup("DungeonBricks",
            new RecipeGroup(() => anyDungeonBrick, ItemID.BlueBrick, ItemID.GreenBrick, ItemID.PinkBrick));
    }

    private static void shimmerLens() {
        ItemID.Sets.ShimmerTransformToItem[ItemID.BlackLens] = ItemID.Lens;
    }
    
    private static void shimmerGuns() {
        ItemID.Sets.ShimmerTransformToItem[ItemID.HelFire] = ItemID.Cascade;
        ItemID.Sets.ShimmerTransformToItem[ItemID.ZapinatorOrange] = ItemID.ZapinatorGray;
    }


    private static void addDungeonFurniture() {
        dungeonBlueBrick();
        dungeonGreenBrick();
        dungeonPinkBrick();

        Recipe.Create(ItemID.DungeonDoor)
            .AddRecipeGroup(RecipeGroupID.Wood, 6)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.Catacomb)
            .AddRecipeGroup("DungeonBricks", 10)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.HangingSkeleton)
            .AddIngredient(ItemID.Bone, 20)
            .AddCondition(Condition.InGraveyard)
            .Register();

        Recipe.Create(ItemID.WallSkeleton)
            .AddIngredient(ItemID.Bone, 20)
            .AddCondition(Condition.InGraveyard)
            .Register();

        Recipe.Create(ItemID.GothicBookcase)
            .AddRecipeGroup("DungeonBricks", 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GothicChair)
            .AddRecipeGroup("DungeonBricks", 4)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GothicTable)
            .AddRecipeGroup("DungeonBricks", 8)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GothicWorkBench)
            .AddRecipeGroup("DungeonBricks", 10)
            .Register();

        Recipe.Create(ItemID.ChainLantern)
            .AddIngredient(ItemID.Chain, 4)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.BrassLantern)
            .AddIngredient(ItemID.CopperBar, 2)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.BrassLantern)
            .AddIngredient(ItemID.TinBar, 2)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.CagedLantern)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.CarriageLantern)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.AlchemyLantern)
            .AddIngredient(ItemID.Glass, 6)
            .AddIngredient(ItemID.JungleTorch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.DiablostLamp)
            .AddIngredient(ItemID.Silk, 6)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();

        Recipe.Create(ItemID.OilRagSconse)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.WorkBenches)
            .Register();
    }

    private static void dungeonBlueBrick() {
        Recipe.Create(ItemID.BlueBrickPlatform, 2)
            .AddIngredient(ItemID.BlueBrick)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonBathtub)
            .AddIngredient(ItemID.BlueBrick, 14)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonBed)
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonBookcase)
            .AddIngredient(ItemID.BlueBrick, 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonCandelabra)
            .AddIngredient(ItemID.BlueBrick, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonCandle)
            .AddIngredient(ItemID.BlueBrick, 4)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonChair)
            .AddIngredient(ItemID.BlueBrick, 4)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonChandelier)
            .AddIngredient(ItemID.BlueBrick, 4)
            .AddIngredient(ItemID.Torch, 4)
            .AddIngredient(ItemID.Chain)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.DungeonClockBlue)
            .AddIngredient(ItemID.BlueBrick, 10)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonDoor)
            .AddIngredient(ItemID.BlueBrick, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonDresser)
            .AddIngredient(ItemID.BlueBrick, 16)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonLamp)
            .AddIngredient(ItemID.Torch)
            .AddIngredient(ItemID.BlueBrick, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonPiano)
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddIngredient(ItemID.Bone, 4)
            .AddIngredient(ItemID.Book)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonSofa)
            .AddIngredient(ItemID.BlueBrick, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonVase)
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonTable)
            .AddIngredient(ItemID.BlueBrick, 8)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.BlueDungeonWorkBench)
            .AddIngredient(ItemID.BlueBrick, 10)
            .Register();
    }

    private static void dungeonGreenBrick() {
        Recipe.Create(ItemID.GreenBrickPlatform, 2)
            .AddIngredient(ItemID.GreenBrick)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonBathtub)
            .AddIngredient(ItemID.GreenBrick, 14)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonBed)
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonBookcase)
            .AddIngredient(ItemID.GreenBrick, 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonCandelabra)
            .AddIngredient(ItemID.GreenBrick, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonCandle)
            .AddIngredient(ItemID.GreenBrick, 4)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonChair)
            .AddIngredient(ItemID.GreenBrick, 4)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonChandelier)
            .AddIngredient(ItemID.GreenBrick, 4)
            .AddIngredient(ItemID.Torch, 4)
            .AddIngredient(ItemID.Chain)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.DungeonClockGreen)
            .AddIngredient(ItemID.GreenBrick, 10)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonDoor)
            .AddIngredient(ItemID.GreenBrick, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonDresser)
            .AddIngredient(ItemID.GreenBrick, 16)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonLamp)
            .AddIngredient(ItemID.Torch)
            .AddIngredient(ItemID.GreenBrick, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonPiano)
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddIngredient(ItemID.Bone, 4)
            .AddIngredient(ItemID.Book)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonSofa)
            .AddIngredient(ItemID.GreenBrick, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonVase)
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonTable)
            .AddIngredient(ItemID.GreenBrick, 8)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.GreenDungeonWorkBench)
            .AddIngredient(ItemID.GreenBrick, 10)
            .Register();
    }

    private static void dungeonPinkBrick() {
        Recipe.Create(ItemID.PinkBrickPlatform, 2)
            .AddIngredient(ItemID.PinkBrick)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonBathtub)
            .AddIngredient(ItemID.PinkBrick, 14)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonBed)
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonBookcase)
            .AddIngredient(ItemID.PinkBrick, 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonCandelabra)
            .AddIngredient(ItemID.PinkBrick, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonCandle)
            .AddIngredient(ItemID.PinkBrick, 4)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonChair)
            .AddIngredient(ItemID.PinkBrick, 4)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonChandelier)
            .AddIngredient(ItemID.PinkBrick, 4)
            .AddIngredient(ItemID.Torch, 4)
            .AddIngredient(ItemID.Chain)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.DungeonClockPink)
            .AddIngredient(ItemID.PinkBrick, 10)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonDoor)
            .AddIngredient(ItemID.PinkBrick, 6)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonDresser)
            .AddIngredient(ItemID.PinkBrick, 16)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonLamp)
            .AddIngredient(ItemID.Torch)
            .AddIngredient(ItemID.PinkBrick, 3)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonPiano)
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddIngredient(ItemID.Bone, 4)
            .AddIngredient(ItemID.Book)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonSofa)
            .AddIngredient(ItemID.PinkBrick, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonVase)
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonTable)
            .AddIngredient(ItemID.PinkBrick, 8)
            .AddTile(TileID.BoneWelder)
            .Register();

        Recipe.Create(ItemID.PinkDungeonWorkBench)
            .AddIngredient(ItemID.PinkBrick, 10)
            .Register();
    }

    private static void addTempleTraps() {
        Recipe.Create(ItemID.SuperDartTrap)
            .AddIngredient(ItemID.LihzahrdBrick, 5)
            .AddIngredient(ItemID.DartTrap)
            .AddTile(TileID.LihzahrdFurnace)
            .Register();

        Recipe.Create(ItemID.SpearTrap)
            .AddIngredient(ItemID.LihzahrdBrick, 5)
            .AddIngredient(ItemID.Javelin)
            .AddTile(TileID.LihzahrdFurnace)
            .Register();

        Recipe.Create(ItemID.SpikyBallTrap)
            .AddIngredient(ItemID.LihzahrdBrick, 5)
            .AddIngredient(ItemID.SpikyBall, 5)
            .AddTile(TileID.LihzahrdFurnace)
            .Register();

        Recipe.Create(ItemID.FlameTrap)
            .AddIngredient(ItemID.LihzahrdBrick, 5)
            .AddIngredient(ItemID.LivingFireBlock, 5)
            .AddTile(TileID.LihzahrdFurnace)
            .Register();
    }

    private static void addObsidianFurniture() {
        Recipe.Create(ItemID.ObsidianChair)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianTable)
            .AddIngredient(ItemID.Obsidian, 8)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianWorkBench)
            .AddIngredient(ItemID.Obsidian, 10)
            .Register();

        Recipe.Create(ItemID.ObsidianCandle)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianLamp)
            .AddIngredient(ItemID.Obsidian, 3)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianBed)
            .AddIngredient(ItemID.Obsidian, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianBookcase)
            .AddIngredient(ItemID.Obsidian, 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianDoor)
            .AddIngredient(ItemID.Obsidian, 6)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianCandelabra)
            .AddIngredient(ItemID.Obsidian, 5)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianChandelier)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddIngredient(ItemID.Torch, 4)
            .AddIngredient(ItemID.Chain)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianChest)
            .AddIngredient(ItemID.Obsidian, 8)
            .AddIngredient(ItemID.IronBar, 2)
            .AddTile(TileID.Furnaces)
            .Register();
        
        Recipe.Create(ItemID.ObsidianClock)
            .AddIngredient(ItemID.Obsidian, 10)
            .AddIngredient(ItemID.IronBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianDresser)
            .AddIngredient(ItemID.Obsidian, 16)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianLantern)
            .AddIngredient(ItemID.Obsidian, 6)
            .AddIngredient(ItemID.Torch)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianPiano)
            .AddIngredient(ItemID.Obsidian, 15)
            .AddIngredient(ItemID.Bone, 4)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianPlatform, 2)
            .AddIngredient(ItemID.Obsidian)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianSink)
            .AddIngredient(ItemID.Obsidian, 6)
            .AddIngredient(ItemID.WaterBucket)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ObsidianSofa)
            .AddIngredient(ItemID.Obsidian, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.Furnaces)
            .Register();

        Recipe.Create(ItemID.ToiletObsidian)
            .AddIngredient(ItemID.Obsidian, 6)
            .AddTile(TileID.Furnaces)
            .Register();
    }

    private static void addBannerRecipes() {
        Recipe.Create(ItemID.WorldBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.SunBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.GravityBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.AnkhBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.SnakeBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.OmegaBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.MarchingBonesBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.NecromanticSign)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.RustedCompanyStandard)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.RaggedBrotherhoodSigil)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.MoltenLegionFlag)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.DiabolicSigil)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.HellboundBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.HellHammerBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.HelltowerBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.LostHopesofManBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.ObsidianWatcherBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.LavaEruptsBanner)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Loom)
            .Register();
    }
}