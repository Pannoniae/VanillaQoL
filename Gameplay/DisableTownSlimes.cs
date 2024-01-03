using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class DisableTownSlimes : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableTownSlimes;
    }

    public override void Load() {
        if (QoLConfig.Instance.disableTownSlimes) {
            IL_Main.UpdateTime_SpawnTownNPCs += NPCSpawnConditionPatch;
            IL_NPC.TransformCopperSlime += GlobalHooks.noop;
            IL_NPC.TransformElderSlime += GlobalHooks.noop;
            IL_NPC.ViolentlySpawnNerdySlime += GlobalHooks.noop;
        }
    }

    public override void Unload() {
        IL_Main.UpdateTime_SpawnTownNPCs -= NPCSpawnConditionPatch;
        IL_NPC.TransformCopperSlime -= GlobalHooks.noop;
        IL_NPC.TransformElderSlime -= GlobalHooks.noop;
        IL_NPC.ViolentlySpawnNerdySlime -= GlobalHooks.noop;
    }

    // [54803 7 - 54803 45]
    // IL_0da8: ldloc.s      numTownNPCs
    // IL_0daa: call         void Terraria.ModLoader.NPCLoader::CanTownNPCSpawn(int32)
    // IL_0daf: ret
    public static void NPCSpawnConditionPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.Emit<GlobalHooks>(OpCodes.Call, "disableTownSlimeSpawn");
    }

    public override void OnWorldLoad() {
        foreach (var npc in Main.npc) {
            if (npc.active && Constants.slimes.Contains(npc.type)) {
                npc.active = false;
            }
        }
    }
}