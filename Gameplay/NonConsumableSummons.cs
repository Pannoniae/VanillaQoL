using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Shared;

namespace VanillaQoL.Gameplay;

public class NonConsumableSummons : GlobalItem {
	public override bool AppliesToEntity(Item item, bool lateInstantiation) {
		return lateInstantiation && Constants.isSummon(item);
	}
    public override void SetDefaults(Item item) {
        if (GlobalFeatures.nonConsumableSummons) {
            item.consumable = false;
        }
    }
}
