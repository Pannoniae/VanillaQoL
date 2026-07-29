using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace VanillaQoL.API;

// Shoutout to Stevie / tomat for inspiration /priorart (The Great Pretender)
public static class Spoof {
    private delegate bool origHasMod(string name);

    private delegate Mod origGetMod(string name);

    private delegate bool origTryGetMod(string name, out Mod result);

    private delegate bool hookTryGetMod(origTryGetMod orig, string name, out Mod result);

    private static readonly Dictionary<string, Mod> spoofs = new();
    private static readonly HashSet<string> realMods = [];

    // we lie to mods but we should be fr to tML. Fun fact Boss Checklist Lite gets this wrong
    // so whenever you exit the config menu it keeps reloading mods because it always thinks the mod is loaded so time to unload
    [ThreadStatic]
    private static bool nocap;

    // we run all this crap *before* tml sets up the logger sooooo we have to do it ourselves!
    private static readonly ILog log = LogManager.GetLogger("VanillaQoL");

    public static bool installed { get; private set; }

    public static void install() {
        if (installed) {
            return;
        }

        try {
            var field = typeof(AssemblyManager).GetField("loadedModContexts",
                BindingFlags.NonPublic | BindingFlags.Static)!;
            var contexts = (IDictionary)field.GetValue(null)!;
            foreach (var key in contexts.Keys) {
                realMods.Add((string)key);
            }
        }
        catch (Exception e) {
            log.Warn($"Couldn't read the loaded mods, not spoofing anything! {e}");
            return;
        }

        MonoModHooks.Add(typeof(ModLoader).GetMethod(nameof(ModLoader.HasMod))!,
            new Func<origHasMod, string, bool>(hasModDetour));
        MonoModHooks.Add(typeof(ModLoader).GetMethod(nameof(ModLoader.GetMod))!,
            new Func<origGetMod, string, Mod>(getModDetour));
        MonoModHooks.Add(typeof(ModLoader).GetMethod(nameof(ModLoader.TryGetMod))!,
            new hookTryGetMod(tryGetModDetour));

        try {
            var uiModItem = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIModItem")!;
            MonoModHooks.Add(uiModItem.GetMethod("OnInitialize")!,
                new Action<Action<object>, object>(noCap));
        }
        catch (Exception e) {
            log.Warn($"Couldn't patch the modmenu to ignore spoofed mods: {e}");
        }

        installed = true;
    }

    private static void noCap(Action<object> orig, object self) {
        nocap = true;
        try {
            orig(self);
        }
        finally {
            nocap = false;
        }
    }

    /**
     * Register a spoofed mod. If the real mod is installed, this will do nothing and return false.
     */
    public static bool register(string name, string displayName, Mod facade) {
        if (!installed) {
            return false;
        }

        if (realMods.Contains(name)) {
            log.Info($"The real {name} is loaded, standing down the spoof.");
            return false;
        }

        spoofs[name] = facade;
        dueDiligence(name, displayName, facade);
        log.Info($"We are {name} now. (v{facade.Version})");
        return true;
    }

    /**
     * Make sure the facade mod has a TmodFile and other properties set so that it can be used in the mods menu. If the real mod is installed, this will do nothing.
     * 'We gotta do our DD'
     */
    private static void dueDiligence(string name, string displayName, Mod facade) {
        try {
            var organizer = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer")!;
            var allFound = (IEnumerable)organizer
                .GetProperty("AllFoundMods", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)!;

            object? best = null;
            Version? bestVersion = null;
            foreach (var local in allFound) {
                var type = local.GetType();
                if ((string)type.GetProperty("Name")!.GetValue(local)! != name) {
                    continue;
                }

                var version = (Version)type.GetProperty("Version")!.GetValue(local)!;
                if (bestVersion is null || version > bestVersion) {
                    best = local;
                    bestVersion = version;
                }
            }

            object file;
            if (best is not null) {
                file = best.GetType().GetField("modFile")!.GetValue(best)!;
                displayName = (string)best.GetType().GetProperty("DisplayName")!.GetValue(best)!;
            }
            else {
                file = fakeTmodFile(name, facade.Version);
            }

            var modT = typeof(Mod);
            modT.GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(facade, file);
            modT.GetProperty(nameof(Mod.DisplayName))!.SetValue(facade, displayName);
            modT.GetProperty(nameof(Mod.TModLoaderVersion))!.SetValue(facade, BuildInfo.tMLVersion);
        }
        catch (Exception e) {
            log.Error($"Couldn't do due diligence for {name}, the mod menu might be VERY fucked. {e}");
        }
    }

    /**
     * Zero entries, but enough to make tML happy.
     * I hope no one seriously uses `Code` or `File` as a form of fucked interop. (otherwise we might need to do way more advanced hacking here....)
     */
    private static object fakeTmodFile(string name, Version version) {
        var path = Path.Combine(Main.SavePath, "VanillaQoL", $"{name}.fake.tmod");
        var file = Activator.CreateInstance(typeof(TmodFile),
            BindingFlags.Instance | BindingFlags.NonPublic, null,
            [path, name, version], null)!;
        typeof(TmodFile).GetField("fileTable", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(file, Array.Empty<TmodFile.FileEntry>());
        typeof(TmodFile).GetProperty(nameof(TmodFile.TModLoaderVersion))!.SetValue(file, BuildInfo.tMLVersion);
        return file;
    }

    /**
     * Check if a mod is installed but being real for a second. If the real mod is installed, this will return true even if a spoof is registered.
     */
    public static bool realHasMod(string name) {
        return installed ? realMods.Contains(name) : ModLoader.HasMod(name);
    }

    /**
     * Check if a mod is spoofed. If the real mod is installed, this will return false even if a spoof is registered.
     */
    public static bool isSpoofed(string name) {
        return spoofs.ContainsKey(name);
    }

    // Terrible hack to read config early. ConfigManager doesn't exist yet, so we read the json config off disk.
    // TODO move this to a shared util so we can use this in more places??
    public static bool configBool(string key, bool def) {
        try {
            var path = Path.Combine(Main.SavePath, "ModConfigs", "VanillaQoL_QoLConfig.json");
            if (!File.Exists(path)) {
                return def;
            }

            var json = JObject.Parse(File.ReadAllText(path));
            return json[key]?.Value<bool>() ?? def;
        }
        catch (Exception e) {
            log.Warn($"No config, using default {key} = {def}. {e}");
            return def;
        }
    }

    public static void clear() {
        // MonoModHooks undoes the detours itself on unload luckily
        spoofs.Clear();
        realMods.Clear();
        installed = false;
    }

    private static bool hasModDetour(origHasMod orig, string name) {
        if (!nocap && spoofs.ContainsKey(name)) {
            return true;
        }

        return orig(name);
    }

    private static Mod getModDetour(origGetMod orig, string name) {
        if (!nocap && spoofs.TryGetValue(name, out var facade)) {
            return facade;
        }

        return orig(name);
    }

    private static bool tryGetModDetour(origTryGetMod orig, string name, out Mod result) {
        if (!nocap && spoofs.TryGetValue(name, out var facade)) {
            result = facade;
            return true;
        }

        return orig(name, out result);
    }
}