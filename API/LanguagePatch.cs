using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.Localization;

namespace VanillaQoL.API;

public class LanguagePatch {

    internal static Dictionary<string, ExtendedLocalizedText> _overrides = new();
    public static bool loaded;

    public static void load() {
        // setup our reload hook
        LanguageManager.Instance.OnLanguageChanged += reloadHandler;
        loaded = true;
    }

    public static void unload() {
        LanguageManager.Instance.OnLanguageChanged -= reloadHandler;
        _overrides = null;
        loaded = false;
    }

    private static void reloadHandler(LanguageManager language) {
        foreach (var langOverride in _overrides) {
            // global override
            if (langOverride.Value.language == 0) {
                modifyKey(langOverride.Key, langOverride.Value.value);
            }
            // language-specific override
            else {
                modifyKey(langOverride.Key, langOverride.Value.value, langOverride.Value.language);
            }
        }
    }

    private static string tryToGuessMod(LocalizedText loc) {
        var parts = loc.Key.Split(".").ToList();
        var modsIndex = parts.IndexOf("Mods");
        // if not found or last, return unknown
        if (modsIndex == -1 || modsIndex == parts.Count - 1) {
            return "<unknown mod>";
        }

        // return next part
        return parts[modsIndex + 1];
    }

    public static void hideKey(string key) {
        modifyKey(key, " ");
    }

    public static void modifyKey(string key, string value, GameCulture.CultureName language = 0) {
        if (!loaded) {
            load();
        }
        var loc = Language.GetOrRegister(key);
        if (!_overrides.ContainsKey(key)) {
            var isHidden = value == " ";
            _overrides.Add(key, new ExtendedLocalizedText(value, language, isHidden));
        }

        if (language == 0 || GameCulture.FromCultureName(language) == Language.ActiveCulture) {
            loc.GetType().GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(loc,
                new[] { value });

            VanillaQoL.instance.Logger.Warn(
                $"Overriding localization key {key} from mod {tryToGuessMod(loc)}! Don't send bug reports to the developers of that mod about broken text.");
        }
    }
}


public class ExtendedLocalizedText {
    public string value;
    public GameCulture.CultureName language;
    public bool hidden = false;

    public ExtendedLocalizedText(string value, GameCulture.CultureName language) {
        this.value = value;
        this.language = language;
    }

    public ExtendedLocalizedText(string value, GameCulture.CultureName language, bool hidden) {
        this.value = value;
        this.language = language;
        this.hidden = true;
    }
}