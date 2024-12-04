using CalamityMod;
using CalamityMod.NPCs.SupremeCalamitas;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.NPCs.BossThePrimordials;

namespace ZenithQoL.Gameplay;


// the fucking compiler-generated classes are at it again
[NoJIT]
public class DisableCredits : ModSystem {
    // don't patch anything if vanilla
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moveCredits != Credits.Vanilla;
    }

    public override void Load() {
        IL_NPC.OnGameEventClearedForTheFirstTime += disableMoonLordCreditsPatch;
        // If Thorium, move it to Primordials, if Calamity, move it to Supreme Calamitas
        if (ZenithQoL.instance.hasCalamity) {
            calModify();
        }
        else if (ZenithQoL.instance.hasThorium) {
            thoriumModify();
        }
    }
    private void calModify() {
        var type = typeof(SupremeCalamitas);
        var method = type.GetMethod("OnKill");
        MonoModHooks.Modify(method, playCreditsPatch2);
    }
    private void thoriumModify() {
        var type = typeof(DreamEater);
        var method = type.GetMethod("OnKill");
        var type2 = typeof(PrimordialBase);
        var method2 = type2.GetMethod("OnKill");
        MonoModHooks.Modify(method, playCreditsPatch);
        MonoModHooks.Modify(method2, playCreditsPatch);
    }

    // [571 7 - 571 73]
    // IL_0000: ldsflda      bool ThoriumMod.ThoriumWorld::downedThePrimordials
    public void playCreditsPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdsflda<ThoriumWorld>("downedThePrimordials"))) {
            ZenithQoL.instance.Logger.Warn("Couldn't find downedThePrimordials in OnKill!");
            return;
        }

        ilCursor.EmitDup();
        ilCursor.Emit<DisableCredits>(OpCodes.Call, "playCredits");
    }

    // [2441 7 - 2441 46]
    //IL_00b8: ldc.i4.1
    //IL_00b9: call         void CalamityMod.DownedBossSystem::set_downedCalamitas(bool)
    public void playCreditsPatch2(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(1),
                i => i.MatchCall<DownedBossSystem>("set_downedCalamitas"))) {
            ZenithQoL.instance.Logger.Warn("Couldn't find downedThePrimordials in OnKill!");
            return;
        }

        ilCursor.Emit<DownedBossSystem>(OpCodes.Call, "get_downedCalamitas");
        ilCursor.Emit<DisableCredits>(OpCodes.Call, "playCredits2");
    }

    public static void playCredits(ref bool downedVar) {
        var firstTime = downedVar == false;
        if (QoLConfig.Instance.moveCredits != Credits.Disabled && firstTime) {
            CreditsRollEvent.TryStartingCreditsRoll();
        }
    }

    public static void playCredits2(bool downedVar) {
        var firstTime = downedVar == false;
        if (QoLConfig.Instance.moveCredits != Credits.Disabled && firstTime) {
            CreditsRollEvent.TryStartingCreditsRoll();
        }
    }

    public override void Unload() {
        IL_NPC.OnGameEventClearedForTheFirstTime -= disableMoonLordCreditsPatch;
    }

    // [42814 11 - 42814 52]
    // IL_002b: call         void Terraria.GameContent.Events.CreditsRollEvent::TryStartingCreditsRoll()
    public void disableMoonLordCreditsPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.Before, i => i.MatchCall<CreditsRollEvent>("TryStartingCreditsRoll"));
        ilCursor.Remove();
    }
}