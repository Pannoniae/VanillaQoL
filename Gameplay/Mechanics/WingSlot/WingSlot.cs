using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.WingSlot;

[Autoload]
public class WingSlot : ModAccessorySlot, ILocalizedModType {

    public string LocalizationCategory => "Wings";

    public static LocalizedText wings = null!;
    public static LocalizedText socialWings = null!;
    public static LocalizedText dye = null!;

    public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.AngelWings;
    public override string VanityTexture => "Terraria/Images/Item_" + ItemID.RedsWings;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.wingSlot;
    }

    public override void SetStaticDefaults() {
        wings = this.GetLocalization(nameof(wings));
        socialWings = this.GetLocalization(nameof(socialWings));
        dye = this.GetLocalization(nameof(dye));
    }

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) {
        // tML asks us about the dye slot too, and dyes are famously not wings:tm:
        return context == AccessorySlotType.DyeSlot || fitsSlot(checkItem);
    }

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) {
        return fitsSlot(item);
    }

    public override void OnMouseHover(AccessorySlotType context) {
        Main.hoverItemName = context switch {
            AccessorySlotType.FunctionalSlot => wings.Value,
            AccessorySlotType.VanitySlot => socialWings.Value,
            AccessorySlotType.DyeSlot => dye.Value,
            _ => Main.hoverItemName
        };
    }

    // bottles and balloons are movement accessories too, they can share
    private static bool fitsSlot(Item item) {
        return Constants.isWing(item) || Constants.isBalloon(item) || Constants.isBottle(item);
    }
}
