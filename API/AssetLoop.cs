using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using log4net;
using Terraria.ModLoader;

namespace VanillaQoL.API;

public static class AssetLoop {
    private delegate void origTransfer();

    // we run before tML sets up the logger
    private static readonly ILog log = LogManager.GetLogger("VanillaQoL");

    private static FieldInfo? isLoading;

    public static void install() {
        var target = typeof(ModContent).GetMethod("TransferCompletedAssets",
            BindingFlags.NonPublic | BindingFlags.Static);
        isLoading = typeof(ModLoader).GetField("isLoading", BindingFlags.NonPublic | BindingFlags.Static);
        if (target == null || isLoading == null) {
            log.Warn("Couldn't find the asset pump, mods will load at tML speed.");
            return;
        }

        MonoModHooks.Add(target, new Action<origTransfer>(loop));
    }

    private static void loop(origTransfer orig) {
        if (!(bool)isLoading!.GetValue(null)!) {
            orig();
            return;
        }

        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 15 && SpinWait.SpinUntil(transfer, 1)) {

        }
    }

    private static bool transfer() {
        var x = false;
        foreach (var mod in ModLoader.Mods) {
            if (mod.Assets is { IsDisposed: false } assets) {
                x |= assets.TransferCompletedAssets();
            }
        }

        return x;
    }
}
