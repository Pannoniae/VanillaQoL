using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace VanillaQoL.API;

// a system to disable config options.
// sadly tML doesn't have good UX for "optional" config i.e. mod-dependant stuff or "only if you have this other option enabled". so we have to do it ourselves.
// todo add mutually exlusive options or something too?
public static class DisabledOptions {
    private static readonly List<Func<PropertyFieldWrapper, object, string?>> rules = [];

    public static void addRule(Func<PropertyFieldWrapper, object, string?> rule) {
        rules.Add(rule);
    }

    public static void clear() {
        rules.Clear();
    }

    private delegate Tuple<UIElement, UIElement>? orig_WrapIt(UIElement parent, ref int top,
        PropertyFieldWrapper memberInfo, object item, int order, object? list, Type? arrayType, int index);

    public static void load() {
        // internal class again, so reflection it is then!
        var wrapIt = typeof(ConfigElement).Assembly
            .GetType("Terraria.ModLoader.Config.UI.UIModConfig")!
            .GetMethod("WrapIt", BindingFlags.Static | BindingFlags.Public)!;
        MonoModHooks.Add(wrapIt, wrapItHook);
    }

    private static Tuple<UIElement, UIElement>? wrapItHook(orig_WrapIt orig, UIElement parent, ref int top,
        PropertyFieldWrapper memberInfo, object item, int order, object? list, Type? arrayType, int index) {
        var tuple = orig(parent, ref top, memberInfo, item, order, list, arrayType, index);
        if (tuple is null || item is null || tuple.Item2 is not ConfigElement element) {
            return tuple;
        }

        var reason = why(memberInfo, item);
        if (reason is null) {
            return tuple;
        }

        element.IgnoresMouseInteraction = true;
        // we have to add this into the parent, NOT to the element cuz the click would walk up
        // into the element (LeftClick -> Parent) and flip the toggle we just disabled
        tuple.Item1.Append(new DisabledOverlay(element, reason));
        return tuple;
    }

    private static string? why(PropertyFieldWrapper member, object config) {
        foreach (var rule in rules) {
            var reason = rule(member, config);
            if (reason is not null) {
                return reason;
            }
        }

        return null;
    }
}

public partial class DisabledOverlay : UIElement {
    private static readonly PropertyInfo textDisplayFunction = typeof(ConfigElement)
        .GetProperty("TextDisplayFunction", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;

    private static readonly PropertyInfo tooltip = typeof(ConfigElement).Assembly
        .GetType("Terraria.ModLoader.Config.UI.UIModConfig")!
        .GetProperty("Tooltip", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly ConfigElement element;
    private readonly string reason;

    public DisabledOverlay(ConfigElement element, string reason) {
        this.element = element;
        this.reason = reason;
        Width.Set(0f, 1f);
        Height.Set(0f, 1f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        // the element's rect, not ours. close, but the container has its own padding
        var dimensions = element.GetDimensions();
        var pixel = TextureAssets.MagicPixel.Value;

        spriteBatch.Draw(pixel,
            new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height),
            Color.Black * 0.30f);

        strikeLabel(spriteBatch, dimensions, pixel);

        if (IsMouseHovering) {
            tooltip.SetValue(null, reason);
        }
    }

    // labels start with an [i:whatever] icon and striking through that looks like arse, so measure the
    // tags separately and start after them
    private static readonly Regex leadingTags = MyRegex();

    private void strikeLabel(SpriteBatch spriteBatch, CalculatedStyle dimensions, Texture2D pixel) {
        if (textDisplayFunction.GetValue(element) is not Func<string> label) {
            return;
        }

        var font = FontAssets.ItemStack.Value;
        const float scale = 0.8f;
        var text = label();

        // the icon has to be measured as a snippet (ChatManager knows how wide an [i:whatever] is), but
        // the words must NOT be - GetStringSize does per-snippet arithmetic that comes up short of what
        // actually gets drawn, and it reports the icon's height rather than the text's
        var icons = leadingTags.Match(text).Value;
        var words = allTags().Replace(text[icons.Length..], "");
        var offset = ChatManager.GetStringSize(font, icons, new Vector2(scale)).X;
        var size = font.MeasureString(words) * scale;

        // ConfigElement puts the label at +8,+8 and the ItemStack at 0.8
        var x = (int)(dimensions.X + 8f + offset);
        // god how fiddly text is...
        var y = (int)(dimensions.Y + 8f + size.Y * 0.38f);
        var width = (int)size.X;

        // the font is outlined, so an unoutlined line looks jank af, give it the same treatment
        spriteBatch.Draw(pixel, new Rectangle(x - 2, y - 2, width + 4, 6), Color.Black * 0.8f);
        spriteBatch.Draw(pixel, new Rectangle(x, y, width, 2), Color.White * 0.85f);
    }

    [GeneratedRegex(@"^(?:\[[a-zA-Z](?:\/[^:\]]*)?:[^\]]*\]\s*)+", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    // MeasureString would count tag markup as literal characters, so strip the lot first
    [GeneratedRegex(@"\[[a-zA-Z](?:\/[^:\]]*)?:([^\]]*)\]", RegexOptions.Compiled)]
    private static partial Regex allTags();
}
