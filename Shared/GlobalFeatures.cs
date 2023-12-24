using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader;

namespace VanillaQoL.Shared;


/// <summary>
/// For using features requested by other mods (ideally from their config OnChanged), do it in SetupContent/SetStaticDefaults or later!
/// Earlier it won't have loaded yet.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class GlobalFeatures {
    // stores features by name
    private static Dictionary<string, EnableList> features = new();

    public static void enableFeature(Mod mod, string name) {
        // if feature doesn't exist, add
        if (!features.ContainsKey(name)) {
            features[name] = new EnableList();
        }

        // if already contains, do nothing
        if (!features[name].enabledMods.Contains(mod)) {
            features[name].enabledMods.Add(mod);
        }
    }

    public static void disableFeature(Mod mod, string name) {
        // if feature doesn't exist, add
        if (!features.ContainsKey(name)) {
            features[name] = new EnableList();
        }

        // if it's not contained, do nothing
        if (features[name].enabledMods.Contains(mod)) {
            features[name].enabledMods.Remove(mod);
        }
    }

    public static bool enabled(string name) {
        if (!features.ContainsKey(name)) {
            return false;
        }

        return features[name].enabledMods.Count > 0;
    }

    public static bool NPCShops => enabled(nameof(NPCShops));
    public static bool nonConsumableSummons => enabled(nameof(nonConsumableSummons));

    // we don't want those mod objects laying around after unload
    internal static void clear() {
        features = null!;
    }
}

public class EnableList {
    public List<Mod> enabledMods = new();
}