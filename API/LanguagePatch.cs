using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.Localization;

namespace ZenithQoL.API;

public class LanguagePatch {
    // mirror
    private static Dictionary<string, LocalizedText> _localizedTexts = new();
    private static Dictionary<string, List<string>> _categoryGroupedKeys = new();

    private static HashSet<(string, string)> _moddedCategoryKeys = new();

    internal static Dictionary<string, ExtendedLocalizedText> _overrides = new();
    public static bool loaded;

    public static void load() {
        // setup our reload hook
        LanguageManager.Instance.OnLanguageChanged += reloadHandler;
        var lm = typeof(LanguageManager);
        var inst = LanguageManager.Instance;
        _localizedTexts = (Dictionary<string, LocalizedText>)lm.GetField(nameof(_localizedTexts),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(inst)!;
        _categoryGroupedKeys = (Dictionary<string, List<string>>)lm.GetField(nameof(_categoryGroupedKeys),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(inst)!;
        loaded = true;
    }

    public static void unload() {
        LanguageManager.Instance.OnLanguageChanged -= reloadHandler;
        _overrides = null!;
        loaded = false;
    }

    private static void reloadHandler(LanguageManager language) {
        // apply the values again for overridden keys
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

        unloadModdedKeys();
    }

    public static void unloadModdedKeys() {
        // reset modded categories
        foreach (var (category, entry) in _moddedCategoryKeys) {
            var key = category + "." + entry;
            _localizedTexts.Remove(key);
            _categoryGroupedKeys[category].Remove(entry);
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
                new object?[] { value });

            ZenithQoL.instance.Logger.Warn(
                $"Overriding localization key {key} from mod {tryToGuessMod(loc)}! Don't send bug reports to the developers of that mod about broken text.");
        }
    }

    public static void addToCategory(string category, string entry, string value) {
        var key = category + "." + entry;
        _moddedCategoryKeys.Add((category, entry));
        if (_localizedTexts.ContainsKey(key)) {
            //_localizedTexts[key].SetValue(value);
            var txt = _localizedTexts[key];
            txt.GetType().GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(txt,
                new object?[] { value });
        }

        var type = typeof(LocalizedText);
        ConstructorInfo ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
            new[] { typeof(string), typeof(string) })!;
        LocalizedText loc = (LocalizedText)ctor.Invoke(new object?[] { entry, value });

        if (!_localizedTexts.ContainsKey(key)) {
            _localizedTexts.Add(key, loc);
        }
        else {
            ZenithQoL.instance.Logger.Warn($"Tried to add the same category twice! {key}");
        }
        if (!_categoryGroupedKeys.ContainsKey(category)) {
            _categoryGroupedKeys.Add(category, new List<string>());
        }

        _categoryGroupedKeys[category].Add(entry);
    }
}

public class ExtendedLocalizedText {
    public string value;
    public GameCulture.CultureName language;
    public bool hidden;

    public ExtendedLocalizedText(string value, GameCulture.CultureName language) {
        this.value = value;
        this.language = language;
        hidden = false;
    }

    public ExtendedLocalizedText(string value, GameCulture.CultureName language, bool hidden) {
        this.value = value;
        this.language = language;
        this.hidden = hidden;
    }
}