using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Shared;

namespace VanillaQoL.Gameplay;

public class NonConsumableSummons : GlobalItem {
    public override void SetDefaults(Item item) {
        if (GlobalFeatures.nonConsumableSummons) {
            if (Constants.isSummon(item)) {
                item.consumable = false;
            }
        }
    }
}