using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class VanillaQoLGlobalItem : GlobalItem, ILocalizedModType {

    public string LocalizationCategory => "Tooltips";

    public static LocalizedText reachText;
    public static LocalizedText launchText;
    public static LocalizedText reelText;
    public static LocalizedText pullText;

    public override void SetStaticDefaults() {
        reachText = this.GetLocalization(nameof(reachText));
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        var proj = item.shoot;
        // if not hook, return
        if (!Main.projHook[item.shoot]) {
            return;
        }

        var projectile = ContentSamples.ProjectilesByType[proj];
        var player = Main.LocalPlayer;

        // modded logic
        ModProjectile mproj;
        string tooltip;
        float distance = 0;
        float reach = 0;
        float launch = 0;
        float reel = 0;
        float pull = 0;
        if ((mproj = projectile.ModProjectile) != null) {
            distance = mproj.GrappleRange();
            launch = item.shootSpeed;
            mproj.GrappleRetreatSpeed(player, ref reel);
            mproj.GrapplePullSpeed(player, ref pull);
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
                default:
                    VanillaQoL.instance.Logger.Warn(
                        $"unhandled hook {item.shoot}!");
                    break;
            }
        }

        reach = distance / 16;

        tooltip = hookStats(reach, launch, reel, pull);
        var tooltipLine = new TooltipLine(VanillaQoL.instance, "HookInfo", tooltip);
        addHookTooltip(tooltips, tooltipLine);
    }

    private static void addHookTooltip(List<TooltipLine> tooltips, TooltipLine tooltip) {
        var equipableTooltip = tooltips.Find(t => t.Mod == "Terraria" && t.Name == "Equipable")!;
        tooltips.AddAfter(equipableTooltip, tooltip);
    }


    private string hookStats(float reach, float launch, float reel, float pull) {
        // todo localise this
        var sb = new StringBuilder();
        sb.Append(reachText.Format(reach.ToString("F2")));
        return sb.ToString();
        /*
        StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler =
            new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
        interpolatedStringHandler.AppendLiteral("Reach: ");
        interpolatedStringHandler.AppendFormatted<float>(reach, "N3");
        interpolatedStringHandler.AppendLiteral(" tiles\n");
        ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
        stringBuilder3.Append(ref local1);
        StringBuilder stringBuilder4 = stringBuilder1;
        StringBuilder stringBuilder5 = stringBuilder4;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder4);
        interpolatedStringHandler.AppendLiteral("Launch Velocity: ");
        interpolatedStringHandler.AppendFormatted<float>(launch, "N2");
        interpolatedStringHandler.AppendLiteral("\n");
        ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
        stringBuilder5.Append(ref local2);
        StringBuilder stringBuilder6 = stringBuilder1;
        StringBuilder stringBuilder7 = stringBuilder6;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder6);
        interpolatedStringHandler.AppendLiteral("Reelback Velocity: ");
        interpolatedStringHandler.AppendFormatted<float>(reel, "N2");
        interpolatedStringHandler.AppendLiteral("\n");
        ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
        stringBuilder7.Append(ref local3);
        StringBuilder stringBuilder8 = stringBuilder1;
        StringBuilder stringBuilder9 = stringBuilder8;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(15, 1, stringBuilder8);
        interpolatedStringHandler.AppendLiteral("Pull Velocity: ");
        interpolatedStringHandler.AppendFormatted<float>(pull, "N2");
        ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler;
        stringBuilder9.Append(ref local4);
        return stringBuilder1.ToString();*/
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
        list.Insert(idx+1, item);
    }
}