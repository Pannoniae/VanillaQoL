using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class InvasionNPCDeath : GlobalNPC {
    private static int npcDeathCount;

    public override void OnKill(NPC npc) {
        if (QoLConfig.Instance.npcDeathsToCallOffInvasion != 0 && Main.invasionType > 0 && npc.townNPC) {
            npcDeathCount++;

            if (npcDeathCount >= QoLConfig.Instance.npcDeathsToCallOffInvasion) {
                Main.invasionType = 0;
                npcDeathCount = 0;
                Color c = new(180, 80, 255);
                Main.NewText(Language.GetTextValue("Mods.VanillaQoL.Tooltips.InvasionCalledOff"), c);
            }
        }
    }
}

public class InvasionPlayerDeath : ModPlayer {
    private static int playerDeathCount;

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
        // Call off invasions if the player dies too many times to the same one
        if (QoLConfig.Instance.playerDeathsToCallOffInvasion > 0 && Main.invasionType != InvasionID.None) {
            playerDeathCount++;

            if (playerDeathCount >= QoLConfig.Instance.playerDeathsToCallOffInvasion) {
                Main.invasionType = InvasionID.None;
                Color c = new(180, 80, 255);
                Main.NewText(Language.GetTextValue("Mods.VanillaQoL.Tooltips.InvasionCalledOff"), c);
            }
        }
    }
}