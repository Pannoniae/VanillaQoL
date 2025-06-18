using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class DungeonNPCs : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.killableDungeonNPCs;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        switch (npc.type) {
            case NPCID.SpikeBall:
                npcLoot.Add(ItemDropRule.Common(ItemID.Spike, 1, 5, 10));
                break;
            case NPCID.BlazingWheel:
                npcLoot.Add(ItemDropRule.Common(ItemID.LivingFireBlock, 1, 5, 15));
                break;
        }
    }

    public override void AI(NPC npc) {
        if (npc.type is NPCID.SpikeBall or NPCID.BlazingWheel) {
            npc.dontTakeDamage = false;
        }
    }
}