using Terraria;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class TownNPC : GlobalNPC {
    public override void SetDefaults(NPC npc) {
        if (QoLConfig.Instance.townPetsInvincible) {
            if (Constants.pets.Contains(npc.type) || Constants.slimes.Contains(npc.type)) {
                npc.dontTakeDamageFromHostiles = true;
            }
        }
    }
}