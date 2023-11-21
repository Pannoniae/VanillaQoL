using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class VanillaQoLGlobalItem : GlobalItem, ILocalizedModType {
    public string LocalizationCategory => "Tooltips";

    public static LocalizedText reachText;
    public static LocalizedText launchText;
    public static LocalizedText reelText;
    public static LocalizedText pullText;
    public static LocalizedText numHooksText1;
    public static LocalizedText numHooksText2Plus;

    public override void SetStaticDefaults() {
        reachText = this.GetLocalization(nameof(reachText));
        numHooksText1 = this.GetLocalization(nameof(numHooksText1));
        numHooksText2Plus = this.GetLocalization(nameof(numHooksText2Plus));
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {

        if (!QoLConfig.Instance.showHookTooltips) {
            return;
        }

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
                Thorium.ModifyTooltips(mproj, ref distance, ref reach, ref launch, ref reel, ref pull, ref numHooks);
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
                case ProjectileID.LunarHookSolar or ProjectileID.LunarHookVortex or ProjectileID.LunarHookNebula or ProjectileID.LunarHookStardust:
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
        addHookTooltip(tooltips, tooltipLine);
    }

    private static void addHookTooltip(List<TooltipLine> tooltips, TooltipLine tooltip) {
        var equipableTooltip = tooltips.Find(t => t.Mod == "Terraria" && t.Name == "Equipable")!;
        tooltips.AddAfter(equipableTooltip, tooltip);
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

    public static class Thorium {
        public static void ModifyTooltips(ModProjectile proj, ref float distance, ref float reach, ref float launch, ref float reel, ref float pull, ref int numHooks) {
            var thorium = ModLoader.GetMod("ThoriumMod");
            if (proj.Type == thorium.Find<ModProjectile>("SpringHookPro").Type) {
                numHooks = 1;

            }
        }
    }
}

public static class ListExtensions {
    /// <summary>
    /// Add an item after a specified element to a list.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="element">The element to add after.</param>
    /// <param name="item">The item to add.</param>
    /// <typeparam name="T"></typeparam>
    public static void AddAfter<T>(this List<T> list, T element, T item) {
        var idx = list.IndexOf(element);
        list.Insert(idx + 1, item);
    }
}