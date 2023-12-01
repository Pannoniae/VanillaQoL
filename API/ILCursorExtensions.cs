using MonoMod.Cil;

namespace VanillaQoL.API;

public static class ILCursorExtensions {
    // Thank you Calamity! (this code is from their utils)
    /// <summary>
    /// Goes to the final ret instruction of a method.
    /// </summary>
    /// <param name="cursor">The cursor to use.</param>
    /// <returns>Whether at least one return was successfully found.</returns>
    public static bool GotoFinalRet(this ILCursor cursor, MoveType moveType) {
        bool ret = false;
        // ReSharper disable once AssignmentInConditionalExpression
        while (cursor.TryGotoNext(moveType, i => i.MatchRet())) {
            ret = true;

        }
        return ret;
    }
}