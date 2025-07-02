using System.Reflection.Emit;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace VanillaQoL.Gameplay;

public class OOA : GlobalNPC {
	public override bool IsLoadingEnabled(Mod mod) {
		return QoLConfig.Instance.OOAdrops2;
	}

	public override void Load() {
        IL_DD2Event.CheckProgress += moreWavesDropMedalsPatch;
        IL_DD2Event.WinInvasionInternal += moreWavesDropMedalsWinPatch;
    }

    private static void moreWavesDropMedalsWinPatch(ILContext il) {
	    var ilCursor = new ILCursor(il);
	    // this is simple
	    // match ldc.i4.3 and call
	    if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(3), i => i.MatchCall<DD2Event>("DropMedals"))) {
		    VanillaQoL.instance.Logger.Warn("Couldn't match ldc.i4.3 in DD2Event.WinInvasionInternal!");
		    return;
	    }
	    
	    // modify the ldc.i4.3 to ldc.i4.4
	    ilCursor.Next.OpCode = OpCodes.Ldc_I4_4;
	    
	    // match ldc.i4 15 and call
	    if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(15), i => i.MatchCall<DD2Event>("DropMedals"))) {
		    VanillaQoL.instance.Logger.Warn("Couldn't match ldc.i4.15 in DD2Event.WinInvasionInternal!");
		    return;
	    }

	    ilCursor.Next.Operand = 20; // change it from 15 to 20

    }

    private static void moreWavesDropMedalsPatch(ILContext il) {
	    
	    var ilCursor = new ILCursor(il);
        // We want to patch these lines:
        /*if (OngoingDifficulty == 1) {
				if (num3 == 5)
					DropMedals(1);

				if (num3 == 4)
					DropMedals(1);
			}

			if (OngoingDifficulty == 2) {
				if (num3 == 7)
					DropMedals(6);

				if (num3 == 6)
					DropMedals(3);

				if (num3 == 5)
					DropMedals(1);
			}

			if (OngoingDifficulty == 3) {
				if (num3 == 7)
					DropMedals(25);

				if (num3 == 6)
					DropMedals(11);

				if (num3 == 5)
					DropMedals(3);

				if (num3 == 4)
					DropMedals(1);
			}
			*/
        // T1 drops 0-0-1-1-3=5 medals in total
        // T2 drops 0-0-0-1-3-6-15=25 medals in total
        // T3 drops 0-0-1-3-11-25-60=100 medals in total
        
        // We want to change it to:
        // T1 drops 1-1-2-2-4=10 medals in total (add 1 medal to each wave)
        // T2 drops 1-2-2-4-6-10-20=45 medals in total
        // T3 drops 2-3-5-8-17-25-60=120 medals in total
        
        // jump to before OngoingDifficulty ldsfld
        if (!ilCursor.TryGotoNext(MoveType.Before,
			i => i.MatchLdsfld<DD2Event>("OngoingDifficulty"))) {
	        VanillaQoL.instance.Logger.Warn("Couldn't match DD2Event.OngoingDifficulty in DD2Event.CheckProgress!");
			return;
		}
        
        // emit call
        ilCursor.EmitLdsfld<DD2Event>("OngoingDifficulty");
        //[7] int32 num3,
        ilCursor.EmitLdloc(7);
        ilCursor.EmitCall<OOA>("adjustMedals");
        
        // mark spot (to insert the jump)
        var mark = ilCursor.MarkLabel();
        
        // jump ahead to ldloc.2
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(2))) {
			VanillaQoL.instance.Logger.Warn("Couldn't match ldloc.2 (currentKillCount) in DD2Event.CheckProgress!");
			return;
		}
        
        var mark2 = ilCursor.MarkLabel();
        // emit jump to the mark
        ilCursor.GotoLabel(mark);
        ilCursor.EmitBr(mark2);
        
        
        // dump method
        // MonoModHooks.DumpIL(VanillaQoL.instance, il);
    }

    public override void Unload() {
        IL_DD2Event.CheckProgress -= moreWavesDropMedalsPatch;
        IL_DD2Event.WinInvasionInternal -= moreWavesDropMedalsWinPatch;
    }

    public static void adjustMedals(int ongoingDifficulty, int currentWave) {
	    // adjust the medals dropped by the DD2Event based on the ongoing difficulty and current kill count

	    // T1 drops 1-1-2-2-4=10 medals in total
	    // T2 drops 1-2-2-4-6-10-20=45 medals in total
	    // T3 drops 2-3-5-8-17-25-60=120 medals in total

	    // this doesn't just add the additional medals! we wiped the entire logic and replaced it with this.

	    // we decrement because the variable already got incremented after the round ended! this restores the original value for which wave we are getting the drops for.
	    currentWave--;

	    switch (ongoingDifficulty) {
		    // T1
		    case 1: {
			    switch (currentWave) {
				    case 1:
				    case 2:
					    DD2Event.DropMedals(1);
					    break;
				    case 3:
				    case 4:
					    DD2Event.DropMedals(2);
					    break;
			    }
			    break;
		    }
		    
		    // T2
		    case 2: {
			    switch (currentWave) {
				    case 1:
					    DD2Event.DropMedals(1);
					    break;
				    case 2:
				    case 3:
					    DD2Event.DropMedals(2);
					    break;
				    case 4:
					    DD2Event.DropMedals(4);
					    break;
				    case 5:
					    DD2Event.DropMedals(6);
					    break;
				    case 6:
					    DD2Event.DropMedals(10);
					    break;
			    }
			    break;
		    }
		    
		    // T3
		    case 3: {
			    switch (currentWave) {
				    case 1:
					    DD2Event.DropMedals(2);
					    break;
				    case 2:
					    DD2Event.DropMedals(3);
					    break;
				    case 3:
					    DD2Event.DropMedals(5);
					    break;
				    case 4:
					    DD2Event.DropMedals(8);
					    break;
				    case 5:
					    DD2Event.DropMedals(17);
					    break;
				    case 6:
					    DD2Event.DropMedals(25);
					    break;
			    }
			    break;
		    }
	    }
    }


    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        
        if (!QoLConfig.Instance.OOAdrops) {
            return;
        }
        switch (npc.type) {
            case NPCID.DD2DarkMageT1:
                npcLoot.Add(ItemDropRule.Common(ItemID.DefenderMedal, 1, 1, 2));
                break;
            case NPCID.DD2DarkMageT3:
            case NPCID.DD2OgreT2:
                npcLoot.Add(ItemDropRule.Common(ItemID.DefenderMedal, 1, 10, 10));
                break;
            case NPCID.DD2OgreT3:
                npcLoot.Add(ItemDropRule.Common(ItemID.DefenderMedal, 1, 15, 15));
                break;
        }
    }
}