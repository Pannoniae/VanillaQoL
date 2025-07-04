using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Cil;
using StarlightRiver.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL;

public class Utils {
    /// <summary>
    /// Completely wipes a class including all static fields and inner classes.
    /// </summary>
    /// <param name="type">The type to wipe.</param>
    public static void completelyWipeClass(Type type) {
        //VanillaQoL.instance!.Logger!.Info("type: " + type);
        // do the same for nested classes
        foreach (var nested in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                   BindingFlags.NonPublic)) {
            //VanillaQoL.instance!.Logger!.Info("nested: " + nested);
            completelyWipeClass(nested);
        }

        var fields = collectSelfStaticFieldInfo(type);
        foreach (var staticField in fields) {
            // constant, skip
            if (staticField.IsLiteral) {
                continue;
            }

            if (type.ContainsGenericParameters) {
                continue;
            }

            //staticField.SetValue(null, null);
            wipeReadonlyFieldIL(staticField);
        }
    }

    public static void completelyWipeNestedClass(Type type) {
        // do the same for nested classes
        foreach (var nested in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                   BindingFlags.NonPublic)) {
            var fields = collectSelfStaticFieldInfo(type);
            foreach (var staticField in fields) {
                // constant, skip
                if (staticField.IsLiteral) {
                    continue;
                }

                //staticField.SetValue(null, null);
                wipeReadonlyFieldIL(staticField);
            }
        }
    }

    // This is the code, just written in IL. This will come back when tML is .NET 8 so the fucking compiler won't refuse to compile this.
    unsafe static private void wipeReadonlyFieldIL(FieldInfo field) {
        var f = field.GetValue(null);
        var addr = &f;
        *addr = null;
    }

    unsafe static private void setReadonlyFieldIL<T>(FieldInfo field, T value) {
       T f = (T)field.GetValue(null);
       var addr = &f;
       *addr = value;
    }

    /// <summary>
    /// Sets the value of a readonly field on an object.
    /// </summary>
    /// <param name="field">The field to set.</param>
    /// <param name="obj">The object to set the field on. `null` if it's a static field.</param>
    /// <param name="value">The value to set the field to.</param>
    public static void setReadonlyField(FieldInfo field, object? obj, object? value) {
        setReadonlyField<object>(field, obj, value);
    }

    /// <inheritdoc cref="setReadonlyField"/>
    public static void setReadonlyField<T>(FieldInfo field, object? obj, T? value) {
        // implement this in IL, because .net6 compiler doesn't like reference to managed object
        // let's do generics by hand!
        var theType = obj?.GetType() ?? typeof(object);
        var method = new DynamicMethod(
            name: "setReadonlyField",
            returnType: null,
            parameterTypes: new[] { theType, field.FieldType },
            restrictedSkipVisibility: true
        );
        var gen = method.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, field);
        gen.Emit(OpCodes.Ret);

        var fieldSetter = method.CreateDelegate(typeof(Action<,>).MakeGenericType(theType, field.FieldType));

        fieldSetter.DynamicInvoke(obj, value);
    }

    /// <inheritdoc cref="setReadonlyField"/>
    /// <summary>
    /// Wipes a readonly field on an object (sets it to null).
    /// </summary>
    public static void wipeReadonlyField(FieldInfo field, object? obj) {
        setReadonlyField(field, obj, null);
    }

    public static FieldInfo[] collectStaticFieldInfo(Type type) {
        if (type == null) throw new ArgumentNullException(nameof(type));
        // collect all static fields
        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        // If this class doesn't have a base, don't waste any time
        if (type.BaseType == typeof(object)) {
            return fieldInfos;
        }

        // Otherwise, collect all types up to the furthest base class
        Type currentType = type;

        var fieldComparer = new FieldInfoComparer();
        var fieldInfoList = new HashSet<FieldInfo>(fieldInfos, fieldComparer);
        while (currentType != typeof(object)) {
            fieldInfos = currentType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            fieldInfoList.UnionWith(fieldInfos);
            currentType = currentType.BaseType!;
        }

        return fieldInfoList.ToArray();
    }

    public static FieldInfo[] collectSelfStaticFieldInfo(Type type) {
        // collect all static fields
        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return fieldInfos;
    }

    /// <returns>
    /// A <see cref="LocalizedText"/> instance which will have the item's translated name.
    /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
    /// </returns>
    public static LocalizedText GetItemName<T>() where T : ModItem =>
        GetTextFromModItem(ModContent.ItemType<T>(), "DisplayName");

    /// <param name="itemID">The item's ID.</param>
    /// <param name="suffix">The desired suffix.</param>
    /// <returns>
    /// A <see cref="LocalizedText"/> instance for the given item and suffix
    /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
    /// </returns>
    public static LocalizedText GetTextFromModItem(int itemID, string suffix) {
        var modItem = ItemLoader.GetItem(itemID);
        return modItem.GetLocalization(suffix);
    }

    /// <param name="itemID">The item's ID.</param>
    /// <returns>
    /// A <see cref="LocalizedText"/> instance for an item's name.
    /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
    /// </returns>
    public static LocalizedText GetItemName(int itemID) {
        if (itemID < ItemID.Count) {
            return Language.GetText("ItemName." + ItemID.Search.GetName(itemID));
        }

        return GetTextFromModItem(itemID, "DisplayName");
    }

    public static void dump(ILContext il) {
        MonoModHooks.DumpIL(VanillaQoL.instance, il);
        MonoModHooks.DumpILHooks();
        MonoModHooks.DumpOnHooks();
    }
}

public class FieldInfoComparer : IEqualityComparer<FieldInfo> {
    public bool Equals(FieldInfo? x, FieldInfo? y) {
        return x?.DeclaringType == y?.DeclaringType && x?.Name == y?.Name;
    }

    public int GetHashCode(FieldInfo obj) {
        return obj.DeclaringType?.GetHashCode() ?? 0 ^ obj.Name.GetHashCode();
    }
}