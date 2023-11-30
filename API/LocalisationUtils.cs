using System;
using System.Runtime.CompilerServices;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.API;

public static class LocalisationUtils {
    /// <summary>
    /// Gets a suitable localization key belonging to this piece of content. <br /><br />Localization keys follow the pattern of "Mods.{ModName}.{Category}.{ContentName}.{DataName}", in this case the <paramref name="suffix" /> corresponds to the DataName.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string GetLocalizationKey(ILocalizedModType type, string suffix) {
        Mod mod = type.Mod;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 3);
        interpolatedStringHandler.AppendFormatted(type.LocalizationCategory);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(suffix);
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        return mod.GetLocalizationKey(stringAndClear);
    }

    /// <summary>
    /// Gets a suitable localization key belonging to this piece of content. <br /><br />Localization keys follow the pattern of "Mods.{ModName}.{Category}.{ContentName}.{DataName}", in this case the <paramref name="suffix" /> corresponds to the DataName.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="subCategory"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string GetLocalizationKey(ILocalizedModType type, string subCategory, string suffix) {
        Mod mod = type.Mod;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
        interpolatedStringHandler.AppendFormatted(type.LocalizationCategory);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(subCategory);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(suffix);
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        return mod.GetLocalizationKey(stringAndClear);
    }

    /// <summary>
    /// Returns a <see cref="T:Terraria.Localization.LocalizedText" /> for this piece of content with the provided <paramref name="suffix" />.
    /// <br />If no existing localization exists for the key, it will be defined so it can be exported to a matching mod localization file.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="suffix"></param>
    /// <param name="makeDefaultValue">A factory method for creating the default value, used to update localization files with missing entries</param>
    /// <returns></returns>
    public static LocalizedText GetLocalization(
        ILocalizedModType type,
        string suffix,
        Func<string>? makeDefaultValue = null) {
        return Language.GetOrRegister(GetLocalizationKey(type, suffix), makeDefaultValue);
    }

    /// <summary>
    /// Returns a <see cref="T:Terraria.Localization.LocalizedText" /> for this piece of content with the provided <paramref name="suffix" />.
    /// <br />If no existing localization exists for the key, it will be defined so it can be exported to a matching mod localization file.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="subCategory"></param>
    /// <param name="suffix"></param>
    /// <param name="makeDefaultValue">A factory method for creating the default value, used to update localization files with missing entries</param>
    /// <returns></returns>
    public static LocalizedText GetLocalization(
        ILocalizedModType type,
        string subCategory,
        string suffix,
        Func<string>? makeDefaultValue = null) {
        return Language.GetOrRegister(GetLocalizationKey(type, subCategory, suffix), makeDefaultValue);
    }

    /// <summary>
    /// Retrieves the text value for a localization key belonging to this piece of content with the given <paramref name="suffix" />. The text returned will be for the currently selected language.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string GetLocalizedValue(ILocalizedModType type, string suffix) =>
        Language.GetTextValue(GetLocalizationKey(type, suffix));
}