using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

public class VoidVault : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.stackVoidBag;
    }

    public override void Load() {
        IL_Player.GetItem += voidBagReorderPatch;
    }

    private static void voidBagReorderPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        
        // jump after this
        //// [32303 4 - 32303 42]
        // IL_004c: ldloc.1      // item
        // IL_004d: ldfld        int32 Terraria.Item::'type'
        // IL_0052: brfalse.s    IL_005c
        // IL_0054: ldloc.1      // item
        // IL_0055: ldfld        int32 Terraria.Item::stack
        // IL_005a: brtrue.s     IL_0062

        // [32304 5 - 32304 23]
        // IL_005c: newobj       instance void Terraria.Item::.ctor()
        // IL_0061: ret
        
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(1),
                i => i.MatchLdfld<Item>("type"),
                i => i.MatchBrfalse(out _), 
                i => i.MatchLdloc(1),
                i => i.MatchLdfld<Item>("stack"),
                i => i.MatchBrtrue(out _),
                i => i.MatchNewobj<Item>(),
                i => i.MatchRet())) {
            VanillaQoL.instance.Logger.Error("Failed to find first inventory pickup for Void Vault patch in Player.GetItem!");
            return;
        }
        
        // move after labels
        ilCursor.MoveAfterLabels();
        
        // we need to emit this sequence
        // [32336 3 - 32336 151]
        //IL_011b: ldarg.3      // settings
        //IL_011c: ldfld        bool Terraria.GetItemSettings::CanGoIntoVoidVault
        //IL_0121: brfalse.s    IL_0151
        //IL_0123: ldarg.0      // this
        //IL_0124: call         instance bool Terraria.Player::get_IsVoidVaultEnabled()
        //IL_0129: brfalse.s    IL_0151
        //IL_012b: ldarg.0      // this
        //IL_012c: ldarg.2      // newItem
        //IL_012d: call         instance bool Terraria.Player::CanVoidVaultAccept(class Terraria.Item)
        //IL_0132: brfalse.s    IL_0151
        //IL_0134: ldarg.0      // this
        //IL_0135: ldarg.1      // plr
        //IL_0136: ldarg.0      // this
        //IL_0137: ldfld        class Terraria.Chest Terraria.Player::bank4
        //IL_013c: ldfld        class Terraria.Item[] Terraria.Chest::item
        //IL_0141: ldarg.2      // newItem
        //IL_0142: ldarg.3      // settings
        //IL_0143: ldloc.1      // item
        //IL_0144: call         instance bool Terraria.Player::GetItem_VoidVault(int32, class Terraria.Item[], class Terraria.Item, valuetype Terraria.GetItemSettings, class Terraria.Item)
        //IL_0149: brfalse.s    IL_0151
        
        //// [32337 4 - 32337 22]
        //IL_014b: newobj       instance void Terraria.Item::.ctor()
        //IL_0150: ret
        var afterLabel = ilCursor.DefineLabel();
        
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdfld<GetItemSettings>("CanGoIntoVoidVault");
        ilCursor.Emit(OpCodes.Brfalse_S, afterLabel);
        ilCursor.EmitLdarg0();
        ilCursor.EmitCall<Player>("get_IsVoidVaultEnabled");
        ilCursor.Emit(OpCodes.Brfalse_S, afterLabel);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg2();
        ilCursor.EmitCall<Player>("CanVoidVaultAccept");
        ilCursor.Emit(OpCodes.Brfalse_S, afterLabel);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdfld<Player>("bank4");
        ilCursor.EmitLdfld<Chest>("item");
        ilCursor.EmitLdarg2();
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdloc1();
        ilCursor.EmitCall<Player>("GetItem_VoidVault");
        ilCursor.Emit(OpCodes.Brfalse_S, afterLabel);
        ilCursor.Emit(OpCodes.Newobj, typeof(Item).GetConstructor([])!);
        ilCursor.Emit(OpCodes.Ret);
        ilCursor.MarkLabel(afterLabel);
        
        
        // important to update offsets here, this stuff is messy...
        ILEdits.updateOffsets(ilCursor);
        MonoModHooks.DumpIL(VanillaQoL.instance, il);

    }
}