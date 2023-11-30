using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class QoLGlobalItem : GlobalItem, ILocalizedModType {
    public string LocalizationCategory => "Tooltips";

    public static LocalizedText reachText = null!;
    public static LocalizedText launchText = null!;
    public static LocalizedText reelText = null!;
    public static LocalizedText pullText = null!;
    public static LocalizedText numHooksText1 = null!;
    public static LocalizedText numHooksText2Plus = null!;

    public static LocalizedText timeText = null!;
    public static LocalizedText hoverText = null!;
    public static LocalizedText hSpeedText = null!;

    public static LocalizedText shimmerable = null!;
    public static LocalizedText shimmerItem = null!;
    public static LocalizedText shimmerPostSkeletron = null!;
    public static LocalizedText shimmerPostGolem = null!;
    public static LocalizedText shimmerPostML = null!;
    public static LocalizedText shimmerDecraft = null!;
    public static LocalizedText shimmerNPC = null!;
    public static LocalizedText shimmerCoinLuck = null!;

    public static LocalizedText ammoFire = null!;
    public static LocalizedText ammoFireArrows = null!;
    public static LocalizedText ammoFireBullets = null!;
    public static LocalizedText ammoFireCoins = null!;
    public static LocalizedText ammoFireRockets = null!;
    public static LocalizedText ammoFireDarts = null!;

    private const string hooks = "Hooks";
    private const string wings = "Wings";
    private const string shimmer = "Shimmer";
    private const string ammo = "Ammo";

    public override void SetStaticDefaults() {
        reachText = LocalisationUtils.GetLocalization(this, hooks, nameof(reachText));
        numHooksText1 = LocalisationUtils.GetLocalization(this, hooks, nameof(numHooksText1));
        numHooksText2Plus = LocalisationUtils.GetLocalization(this, hooks, nameof(numHooksText2Plus));
        timeText = LocalisationUtils.GetLocalization(this, wings, nameof(timeText));
        hoverText = LocalisationUtils.GetLocalization(this, wings, nameof(hoverText));
        hSpeedText = LocalisationUtils.GetLocalization(this, wings, nameof(hSpeedText));
        shimmerable = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerable));
        shimmerItem = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerItem));
        shimmerPostSkeletron = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerPostSkeletron));
        shimmerPostGolem = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerPostGolem));
        shimmerPostML = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerPostML));
        shimmerDecraft = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerDecraft));
        shimmerNPC = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerNPC));
        shimmerCoinLuck = LocalisationUtils.GetLocalization(this, shimmer, nameof(shimmerCoinLuck));
        ammoFire = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFire));
        ammoFireArrows = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFireArrows));
        ammoFireBullets = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFireBullets));
        ammoFireCoins = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFireCoins));
        ammoFireRockets = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFireRockets));
        ammoFireDarts = LocalisationUtils.GetLocalization(this, ammo, nameof(ammoFireDarts));
    }


    /// <summary>
    /// Allows you to modify all the tooltips that display for the given item. See here for information about TooltipLine. To hide tooltips, please use <see cref="M:Terraria.ModLoader.TooltipLine.Hide" /> and defensive coding.
    /// </summary>
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (QoLConfig.Instance.showHookTooltips) {
            hookTooltips(item, tooltips);
        }

        if (QoLConfig.Instance.showWingTooltips) {
            wingTooltips(item, tooltips);
        }

        if (QoLConfig.Instance.ammunitionTooltips) {
            ammunitionTooltips(item, tooltips);
        }

        if (QoLConfig.Instance.vanillaThoriumTooltips && VanillaQoL.instance.hasThorium) {
            vanillaifyThoriumTooltips(item, tooltips);
        }

        if (QoLConfig.Instance.shimmerableTooltip) {
            shimmmerableTooltips(item, tooltips);
        }
    }

    private void vanillaifyThoriumTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.RemoveAll(t => pred(t, "BardTag"));
        tooltips.RemoveAll(t => pred(t, "ThrowerTag"));
        tooltips.RemoveAll(t => pred(t, "HealerTag"));
        tooltips.RemoveAll(t => pred(t, "TransformationTag"));
        tooltips.RemoveAll(t => pred(t, "RealityTag"));
        tooltips.RemoveAll(t => pred(t, "AccessoryWarning"));
        tooltips.RemoveAll(t => pred(t, "InstrumentTag"));
    }

    private static bool pred(TooltipLine t, string name) {
        return t.Mod == "ThoriumMod" && t.Name == name;
    }

    private void wingTooltips(Item item, List<TooltipLine> tooltips) {
        // calamity does the same thing, don't apply tooltips in that case
        if (VanillaQoL.instance.hasCalamity) {
            return;
        }

        // not a wing
        if (item.wingSlot < 1) {
            return;
        }

        var wingType = item.wingSlot;

        var stats = ArmorIDs.Wing.Sets.Stats[item.wingSlot];
        var player = Main.LocalPlayer;

        var baseSpeed = stats.AccRunSpeedOverride * Constants.speedToMph;
        // it's an open question whether we want to apply the player speed / other factors or not. probably not, it would make it inconsistent.
        var horizontalSpeed = baseSpeed;

        float constantAscend = 0.0f;
        float ascentWhenFalling = 0.0f;
        float maxAscentMultiplier = 0.0f;
        float maxCanAscendMultiplier = 0.0f;
        float ascentWhenRising = 0.0f;

        copiedVanillaLogic(player, wingType, ref constantAscend, ref ascentWhenFalling, ref maxAscentMultiplier,
            ref maxCanAscendMultiplier, ref ascentWhenRising);
        // in vanilla, speed ranges from 3, most clustering around 6, up to 9 on endgame wings.
        // in mph, this is 3 = 15, 6 = 30.5, 7 = 35.7, 9 = 46.
        // strongest cal wing is 1.15 = 58


        var time = (float)stats.FlyTime / 60;
        var hover = stats.HasDownHoverStats;

        string tooltip = wingStats(time, hover, horizontalSpeed);
        var tooltipLine = new TooltipLine(VanillaQoL.instance, "WingInfo", tooltip);
        addTooltip(tooltips, tooltipLine);
    }

    private void copiedVanillaLogic(Player player, int wingType, ref float constantAscend, ref float ascentWhenFalling,
        ref float maxAscentMultiplier, ref float maxCanAscendMultiplier, ref float ascentWhenRising) {
        constantAscend = 0.1f;
        ascentWhenFalling = 0.5f;
        maxAscentMultiplier = 1.5f;
        maxCanAscendMultiplier = 0.5f;
        ascentWhenRising = 0.1f;
        if (wingType == 26) {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        if (wingType == 8 || wingType == 11 || wingType == 24 || wingType == 27 || wingType == 22)
            maxAscentMultiplier = 1.66f;
        if (wingType == 21 || wingType == 12 || wingType == 20 || wingType == 23)
            maxAscentMultiplier = 1.805f;
        if (wingType == 37) {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        if (wingType == 44) {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.75f;
            constantAscend = 0.125f;
        }

        if (wingType == 45) {
            ascentWhenFalling = 0.95f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 4.5f;
        }

        if (wingType == 29 || wingType == 32) {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        if (wingType == 30 || wingType == 31) {
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.45f;
            constantAscend = 0.15f;
        }

        ItemLoader.VerticalWingSpeeds(player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier,
            ref maxAscentMultiplier, ref constantAscend);
    }

    private string wingStats(float time, bool hover, float horizontalSpeed) {
        var sb = new StringBuilder();
        sb.AppendLine(timeText.Format(time.ToString("0.##")));
        sb.Append(hSpeedText.Format(horizontalSpeed.ToString("0")));
        //if (hover) {
        //    sb.AppendLine();
        //    sb.Append(hoverText);
        //}

        return sb.ToString();
    }

    private void hookTooltips(Item item, List<TooltipLine> tooltips) {
        // calamity does the same thing, don't apply tooltips in that case
        if (VanillaQoL.instance.hasCalamity) {
            return;
        }

        var proj = item.shoot;
        // if not hook, return
        if (!Main.projHook[item.shoot]) {
            return;
        }

        var projectile = ContentSamples.ProjectilesByType[proj];
        var player = Main.LocalPlayer;


        ModProjectile mproj;
        string tooltip;
        float distance = 0;
        float reach = 0;
        float launch = 0;
        float reel = 0;
        float pull = 0;
        int numHooks = 0;
        // modded logic
        if ((mproj = projectile.ModProjectile) != null) {
            distance = mproj.GrappleRange();
            launch = item.shootSpeed;
            mproj.GrappleRetreatSpeed(player, ref reel);
            mproj.GrapplePullSpeed(player, ref pull);
            mproj.NumGrappleHooks(player, ref numHooks);

            if (VanillaQoL.instance.hasThorium) {
                Thorium.ModifyTooltips(mproj, ref distance, ref reach, ref launch, ref reel, ref pull,
                    ref numHooks);
            }
        }
        // vanilla logic
        else {
            // distance
            switch (item.shoot) {
                case ProjectileID.Hook:
                    distance = 300;
                    break;
                case ProjectileID.IvyWhip:
                    distance = 400;
                    break;
                case ProjectileID.DualHookBlue or ProjectileID.DualHookRed:
                    distance = 440;
                    break;
                case ProjectileID.Web:
                    distance = 375;
                    break;
                case ProjectileID.SkeletronHand:
                    distance = 350;
                    break;
                case ProjectileID.BatHook:
                    distance = 500;
                    break;
                case ProjectileID.WoodHook:
                    distance = 550;
                    break;
                case ProjectileID.CandyCaneHook:
                    distance = 400;
                    break;
                case ProjectileID.ChristmasHook:
                    distance = 550;
                    break;
                case ProjectileID.FishHook:
                    distance = 400;
                    break;
                case ProjectileID.SlimeHook:
                    distance = 300;
                    break;
                case ProjectileID.LunarHookSolar or ProjectileID.LunarHookVortex or ProjectileID.LunarHookNebula
                    or ProjectileID.LunarHookStardust:
                    distance = 550;
                    break;
                case ProjectileID.StaticHook:
                    distance = 600;
                    break;
                case ProjectileID.QueenSlimeHook:
                    distance = 500;
                    break;
                case ProjectileID.SquirrelHook:
                    distance = 300;
                    break;
                case ProjectileID.TendonHook or ProjectileID.ThornHook or ProjectileID.IlluminantHook
                    or ProjectileID.WormHook:
                    distance = 480;
                    break;
                // gemhooks
                case >= ProjectileID.GemHookAmethyst and <= ProjectileID.GemHookDiamond:
                    // setting distance by itemid
                    distance = 300 + (item.shoot - ProjectileID.GemHookAmethyst) * 30;
                    break;
                case ProjectileID.AmberHook:
                    distance = 420;
                    break;
                case ProjectileID.AntiGravityHook:
                    distance = 500;
                    break;
                default:
                    VanillaQoL.instance.Logger.Warn(
                        $"unhandled hook {item.shoot}!");
                    break;
            }

            numHooks = 3;
            switch (item.shoot) {
                case ProjectileID.DualHookBlue or ProjectileID.DualHookRed:
                    numHooks = 2;
                    break;
                case ProjectileID.Web:
                    numHooks = 8;
                    break;
                case ProjectileID.SkeletronHand:
                    numHooks = 2;
                    break;
                case ProjectileID.FishHook:
                    numHooks = 2;
                    break;
                case ProjectileID.StaticHook:
                    numHooks = 2;
                    break;
                case ProjectileID.LunarHookSolar or ProjectileID.LunarHookVortex or ProjectileID.LunarHookNebula
                    or ProjectileID.LunarHookStardust:
                    numHooks = 4;
                    break;
            }

            // if SingleGrappleHook, then numHooks is always 1, regardless of the numHooks value
            if (ProjectileID.Sets.SingleGrappleHook[item.shoot]) {
                numHooks = 1;
            }

            launch = numHooks;
        }

        reach = distance / 16;

        tooltip = hookStats(reach, launch, reel, pull, numHooks);
        var tooltipLine = new TooltipLine(VanillaQoL.instance, "HookInfo", tooltip);
        addTooltip(tooltips, tooltipLine);
    }

    private static void addTooltip(List<TooltipLine> tooltips, TooltipLine tooltip) {
        var equipableTooltip = tooltips.Find(t => t.Mod == "Terraria" && t.Name == "Equipable")!;
        tooltips.AddAfter(equipableTooltip, tooltip);
    }

    private static void addShimmerTooltip(List<TooltipLine> tooltips, TooltipLine tooltip) {
        var materialTooltip = tooltips.FindLast(t => t.Mod == "Terraria" && t.Name != "Expert" && t.Name != "Master")!;
        tooltips.AddAfter(materialTooltip, tooltip);
    }

    private static void addAmmoTooltip(List<TooltipLine> tooltips, TooltipLine tooltip) {
        var knockbackTooltip = tooltips.FindLast(t => t.Mod == "Terraria" && t.Name == "Knockback")!;
        tooltips.AddAfter(knockbackTooltip, tooltip);
    }

    private string hookStats(float reach, float launch, float reel, float pull, int numHooks) {
        LocalizedText numHooksFormatted = numHooks switch {
            1 => numHooksText1,
            _ => numHooksText2Plus
        };

        var sb = new StringBuilder();
        sb.AppendLine(reachText.Format(reach.ToString("0.###")));
        sb.Append(numHooksFormatted.Format(numHooks));
        return sb.ToString();
    }

    /// <summary>
    /// Allows you to do things before a tooltip line of this item is drawn. The line contains draw info.
    /// </summary>
    /// <param name="item">The item</param>
    /// <param name="line">The line that would be drawn</param>
    /// <param name="yOffset">The Y offset added for next tooltip lines</param>
    /// <returns>Whether or not to draw this tooltip line</returns>
    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
        // todo make coloured tooltip with api
        // vary between two colours with sin interpolation? idk
        return true;
    }

    private void shimmmerableTooltips(Item item, List<TooltipLine> tooltips) {
        if (!item.CanShimmer()) {
            return;
        }

        string tooltip = "";

        // apply vanilla overrides
        int itemTransform = ItemID.Sets.ShimmerCountsAsItem[item.type];
        int sourceItem = itemTransform == -1 ? item.type : itemTransform;
        int targetItem = ItemID.Sets.ShimmerTransformToItem[sourceItem];

        var NPCTransform = NPCID.Sets.ShimmerTransformToNPC[item.makeNPC];
        var targetNPC = NPCTransform == -1 ? item.makeNPC : NPCTransform;
        int coinLuckValue = ItemID.Sets.CoinLuckValue[sourceItem];
        int decraftingRecipeIndex = ShimmerTransforms.GetDecraftingRecipeIndex(sourceItem);

        // I planned to use these to display shimmering conditions, but that's not done yet. So here it is commented out so the compiler shuts up about unused variables.
        bool postSkeletron = false;
        bool postGolem = false;
        if (decraftingRecipeIndex > -1) {
            if (ShimmerTransforms.RecipeSets.PostSkeletron[decraftingRecipeIndex]) {
                postSkeletron = true;
            }

            if (ShimmerTransforms.RecipeSets.PostGolem[decraftingRecipeIndex]) {
                postGolem = true;
            }
        }

        bool moonLordRequirement = requiresMoonLordToShimmer(sourceItem);
        // wtf special luminite brick logic
        if (sourceItem == ItemID.LunarBrick) {
            targetItem = Main.GetMoonPhase() switch {
                MoonPhase.Full => 5408,
                MoonPhase.ThreeQuartersAtLeft => 5401,
                MoonPhase.HalfAtLeft => 5403,
                MoonPhase.QuarterAtLeft => 5402,
                MoonPhase.QuarterAtRight => 5407,
                MoonPhase.HalfAtRight => 5405,
                MoonPhase.ThreeQuartersAtRight => 5404,
                _ => 5406
            };
        }
        else if (item.createTile == TileID.MusicBoxes) {
            targetItem = 576;
        }

        // Shimmer transform
        if (targetItem > 0) {
            var itemName = ContentSamples.ItemsByType[targetItem].Name;
            var itemString = targetItem;
            if (moonLordRequirement) {
                tooltip = shimmerable + shimmerItem.Format(itemString, itemName) + shimmerPostML;
            }
            else {
                tooltip = shimmerable + shimmerItem.Format(itemString, itemName);
            }
        }

        // Decrafting
        // we don't actually need a tooltip for this, that would be bloat
        if (decraftingRecipeIndex > -1) {
            var shimmerCondition = "";
            if (postSkeletron) {
                shimmerCondition = shimmerPostSkeletron.Value;
            }

            if (postGolem) {
                shimmerCondition = shimmerPostGolem.Value;
            }
            tooltip = shimmerCondition;
        }

        // gamer girl bathwater
        if (sourceItem == ItemID.GelBalloon) {
            var NPCName = NPCIDtoNPC(NPCID.TownSlimeRainbow).TypeName;
            tooltip = shimmerable + shimmerNPC.Format(NPCName);
        }

        // NPC
        if (targetNPC > 0) {
            var NPCName = NPCIDtoNPC(targetNPC).TypeName;
            tooltip = shimmerable + shimmerNPC.Format(NPCName);
        }

        // coin luck
        if (coinLuckValue > 0) {
            //var value = Main.ValueToCoins(coinLuckValue);
            tooltip = shimmerable + shimmerCoinLuck.Format(coinLuckValue);
        }

        // Add conditions (post-skeletron/golem/ml)


        var tooltipLine = new TooltipLine(VanillaQoL.instance, "ShimmerInfo", tooltip);
        // we want a purple glint but until we have it, white it is
        //tooltipLine.OverrideColor = Color.LightPink;
        addShimmerTooltip(tooltips, tooltipLine);
    }

    private NPC NPCIDtoNPC(int id) {
        return ContentSamples.NpcsByNetId[id];
    }

    private bool requiresMoonLordToShimmer(int item) {
        return item == ItemID.Clentaminator || item == ItemID.RodofDiscord || item == ItemID.BottomlessBucket || item == ItemID.BottomlessShimmerBucket;
    }

    private void ammunitionTooltips(Item item, List<TooltipLine> tooltips) {
        var ammoItem = item.useAmmo;

        if (ammoItem == AmmoID.None) {
            return;
        }

        string tooltip;
        if (ammoItem == AmmoID.Arrow) {
            tooltip = ammoFireArrows.Value;
        }
        else if (ammoItem == AmmoID.Bullet) {
            tooltip = ammoFireBullets.Value;
        }
        else if (ammoItem == AmmoID.Coin) {
            tooltip = ammoFireCoins.Value;
        }
        else if (ammoItem == AmmoID.Rocket) {
            tooltip = ammoFireRockets.Value;
        }
        else if (ammoItem == AmmoID.Dart) {
            tooltip = ammoFireDarts.Value;
        }
        else {
            var itemName = ContentSamples.ItemsByType[ammoItem].Name;
            tooltip = ammoFire.Format(ammoItem, itemName);
        }

        var tooltipLine = new TooltipLine(VanillaQoL.instance, "AmmoInfo", tooltip);
        addAmmoTooltip(tooltips, tooltipLine);
    }

    public static class Thorium {
        public static void ModifyTooltips(ModProjectile proj, ref float distance, ref float reach, ref float launch,
            ref float reel, ref float pull, ref int numHooks) {
            var thorium = ModLoader.GetMod("ThoriumMod");
            if (proj.Type == thorium.Find<ModProjectile>("SpringHookPro").Type) {
                numHooks = 1;
            }
        }
    }
}