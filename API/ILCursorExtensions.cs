using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ZenithQoL.API;

public static class ILCursorExtensions {
    // Thank you Calamity! (this code is from their utils)
    /// <summary>
    /// Goes to the final ret instruction of a method.
    /// </summary>
    /// <returns>Whether at least one return was successfully found.</returns>
    public static bool GotoFinalRet(this ILCursor cursor, MoveType moveType) {
        bool ret = false;
        // ReSharper disable once AssignmentInConditionalExpression
        while (cursor.TryGotoNext(moveType, i => i.MatchRet())) {
            ret = true;
        }
        return ret;
    }

    public static bool forAll(this ILCursor cursor, MoveType moveType, out List<ILCursor> cursors, int minSuccesses = 1, params Func<Instruction, bool>[] predicates) {
        // ReSharper disable once AssignmentInConditionalExpression
        cursors = new List<ILCursor>();
        var ctr = 0;
        while (cursor.TryGotoNext(moveType, predicates)) {
            cursors.Add(new ILCursor(cursor));
            ctr++;
        }

        return ctr >= minSuccesses;
    }

    public static ILCursor EmitCall<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Call, memberName);

    public static ILCursor EmitCallvirt<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Callvirt, memberName);

    public static ILCursor EmitLdfld<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Ldfld, memberName);

    public static ILCursor EmitLdsfld<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Ldsfld, memberName);

    public static ILCursor EmitStfld<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Stfld, memberName);

    public static ILCursor EmitStsfld<T>(this ILCursor ilCursor, string memberName) =>
        ilCursor.Emit<T>(OpCodes.Stsfld, memberName);
}