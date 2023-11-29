using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;

namespace VanillaQoL;

public class Utils {
    /// <summary>
    /// Completely wipes a class including all static fields and inner classes.
    /// </summary>
    /// <param name="cls"></param>
    public static void completelyWipeClass(Type type) {
        // do the same for nested classes
        foreach (var nested in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
            completelyWipeClass(nested);
        }

        var fields = collectSelfStaticFieldInfo(type);
        foreach (var staticField in fields) {
            try {
                // constant, skip
                if (staticField.IsLiteral) {
                    continue;
                }
                staticField.SetValue(null, null);
            }
            // static readonly field? time for unsafe hackery because reflection doesn't work
            catch (FieldAccessException e) {

                wipeReadonlyField(staticField, null);
                // actually we can
                // don't throw if mod init broke, so check for null
                if (VanillaQoL.instance != null && VanillaQoL.instance.Logger != null) {
                    VanillaQoL.instance.Logger.Info($"A harmless exception happened! Ignore the above exception. " +
                                                    $"Couldn't clear {staticField.Name} the normal way, so IL generation was used, exception message: {e.Message}");
                }
            }
        }
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
            currentType = currentType.BaseType;
        }

        return fieldInfoList.ToArray();
    }

    public static FieldInfo[] collectSelfStaticFieldInfo(Type type) {
        // collect all static fields
        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return fieldInfos;
    }
}

public class FieldInfoComparer : IEqualityComparer<FieldInfo> {
    public bool Equals(FieldInfo x, FieldInfo y) {
        return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
    }

    public int GetHashCode(FieldInfo obj) {
        return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
    }
}