using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod.Items;
using CalamityMod.Projectiles;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * Calamity builds a lot of vanilla nerfs in Mod.Load and read back in SetDefaults. Instead of IL patching a lot of shit, how about we just change the tables instead?
 *
 * Has to run in OnModLoad and not PostSetupContent: ContentSamples is built in between, and samples that disagree
 * with real items are worse than the nerf we're removing.
 *
 * The rule types are internal and nested so this is horrible. Oh well...
 */
[JITWhenModsEnabled("CalamityMod")]
public static class CalamityTweakTables {
    public static int stripItemRules(params string[] rules) {
        return strip(typeof(CalamityGlobalItem), rules);
    }

    public static int stripProjectileRules(params string[] rules) {
        return strip(typeof(CalamityGlobalProjectile), rules);
    }

    private static int strip(Type owner, string[] rules) {
        var field = owner.GetField("currentTweaks", BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is not IDictionary table) {
            VanillaQoL.instance.Logger.Warn($"Couldn't find {owner.Name}.currentTweaks, can't strip {string.Join(", ", rules)}!");
            return 0;
        }

        var doomed = new HashSet<Type>();
        foreach (var rule in rules) {
            var type = owner.GetNestedType(rule, BindingFlags.NonPublic);
            if (type == null) {
                VanillaQoL.instance.Logger.Warn($"Couldn't find {owner.Name}.{rule}, can't strip!");
                continue;
            }

            doomed.Add(type);
        }

        if (doomed.Count == 0) {
            return 0;
        }

        var changed = 0;
        // the arrays are shared between entries so we swap in a new one instead of editing in place
        foreach (var key in table.Keys.Cast<object>().ToList()) {
            if (table[key] is not Array applied) {
                continue;
            }

            var kept = applied.Cast<object>().Where(rule => rule == null || !doomed.Contains(rule.GetType())).ToList();
            if (kept.Count == applied.Length) {
                continue;
            }

            var fittest = Array.CreateInstance(applied.GetType().GetElementType()!, kept.Count);
            for (var i = 0; i < kept.Count; i++) {
                fittest.SetValue(kept[i], i);
            }

            table[key] = fittest;
            changed++;
        }

        return changed;
    }
}
