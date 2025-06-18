using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class NoDevSet : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.removeDevSet;
    }

    public override void SetDefaults(Item item) {
        if (ItemID.Sets.BossBag[item.type]) {
            ItemID.Sets.PreHardmodeLikeBossBag[item.type] = true;
        }
    }
}