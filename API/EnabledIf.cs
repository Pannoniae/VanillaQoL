using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace VanillaQoL.API;

public interface Condition {
    bool enabled();

    /// already localised, we just show it
    string reason();
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
public class EnabledIfAttribute(Type condition) : Attribute {
    public readonly Type condition = condition;
}

// template to copypaste lol
public static class EnabledIf {
    private static readonly Dictionary<Type, Condition> cache = new();

    public static string? rule(PropertyFieldWrapper member, object config) {
        var attribute = member.MemberInfo?.GetCustomAttribute<EnabledIfAttribute>()
                        ?? config.GetType().GetCustomAttribute<EnabledIfAttribute>();
        if (attribute is null) {
            return null;
        }

        var condition = get(attribute.condition);
        return condition.enabled() ? null : condition.reason();
    }

    private static Condition get(Type type) {
        if (cache.TryGetValue(type, out var cached)) {
            return cached;
        }

        if (!typeof(Condition).IsAssignableFrom(type)) {
            throw new ArgumentException($"[EnabledIf({type.Name})] - that isn't an ICondition.");
        }

        return cache[type] = (Condition)Activator.CreateInstance(type)!;
    }
}
