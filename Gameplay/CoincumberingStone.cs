using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class CoincumberingStone : ModSystem {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.coincumberingStone;
    }

    public override void PostSetupContent() {
        for (int i = 0; i < ItemLoader.ItemCount; ++i) {
            if (ItemID.Sets.CommonCoin[i])
                ItemID.Sets.IgnoresEncumberingStone[i] = true;
        }
    }

    public override void Unload() {
        for (int i = 0; i < ItemLoader.ItemCount; ++i) {
            if (ItemID.Sets.CommonCoin[i])
                ItemID.Sets.IgnoresEncumberingStone[i] = false;
        }
    }
}