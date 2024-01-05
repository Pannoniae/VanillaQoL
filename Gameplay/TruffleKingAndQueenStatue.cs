using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class TruffleKingAndQueenStatue : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Truffle;

    public override bool? CanGoToStatue(NPC npc, bool toKingStatue) => QoLConfig.Instance.truffleKingAndQueenStatue ? true : null;
}
