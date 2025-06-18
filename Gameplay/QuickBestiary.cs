using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class QuickBestiary : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.quickBestiary;
    }

    public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npc.type], quickUnlock: true);
    }
}