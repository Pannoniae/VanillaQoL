using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ZenithQoL.Gameplay;

public class QuickBuffFavourite : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.quickBuffFavourite;
    }

    public override void Load() {
        IL_Player.QuickBuff += quickBuffFavouritePatch;
    }

    public override void Unload() {
        IL_Player.QuickBuff -= quickBuffFavouritePatch;
    }

    // [3100 14 - 3100 27]
    // IL_00bd: ldc.i4.0
    // IL_00be: stloc.s      index
    //
    // [3126 13 - 3126 22]
    // IL_01ca: ldloc.s      flag
    // IL_01cc: brfalse.s    IL_0240
    public void quickBuffFavouritePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0),
                i => i.MatchStloc(out _))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match ldc.i4.0 and stloc.s index in Player.QuickBuff!");
        }

        il.Body.Variables.Add(new VariableDefinition(il.Import(typeof(bool))));
        var idx = il.Body.Variables.Count - 1; // we use the last variable we just added!
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdloc3();
        ilCursor.EmitCall<QuickBuffFavourite>("hasFavouritedPotion");
        ilCursor.EmitStloc(idx);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(7),
                i => i.MatchBrfalse(out var label))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match CanUseItem flag in Player.QuickBuff!");
        }
        // after ldloc
        ilCursor.Index++;
        // we also want to check (flag(canUse) && (!hasFavouritedPotion() || item.favorited))
        // if we don't have one, we continue with vanilla logic, but if we have at least one favourited potion, we check whether
        // this one is favourited, and only consume it if so
        ilCursor.EmitLdloc(5);
        ilCursor.EmitLdloc(idx);
        ilCursor.EmitCall<QuickBuffFavourite>("isFavourited");
        ilCursor.EmitAnd();
    }

    public static bool hasFavouritedPotion(Player player, int max) {
        var itemCheck_CheckCanUse =
            typeof(Player).GetMethod("ItemCheck_CheckCanUse", BindingFlags.Instance | BindingFlags.NonPublic)!;
        for (int index = 0; index < max; ++index) {
            Item item = index >= 58 ? player.bank4.item[index - 58] : player.inventory[index];
            if (item.stack > 0 && item.type > ItemID.None && item.buffType > 0 &&
                !item.CountsAsClass(DamageClass.Summon) &&
                // ReSharper disable once CoVariantArrayConversion
                (bool)itemCheck_CheckCanUse.Invoke(player, new[] { item })!) {
                if (item.favorited) {
                    // we have a favourited potion, cancel
                    return true;
                }
            }
        }

        return false;
    }

    public static bool isFavourited(Item item, bool hasFavourited) {
        return !hasFavourited || item.favorited;
    }
}