using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ZenithQoL.Gameplay.WingSlot;

[Autoload]
public class WingSlot : ModAccessorySlot {

    public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.AngelWings;
    public override string VanityTexture => "Terraria/Images/Item_" + ItemID.RedsWings;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.wingSlot;
    }

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) {
        return Constants.isWing(checkItem) || Constants.isBalloon(checkItem) || Constants.isBottle(checkItem);
    }

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) {
        return Constants.isWing(item) || Constants.isBalloon(item) || Constants.isBottle(item);
    }

    public override void OnMouseHover(AccessorySlotType context) {
        switch (context) {
            case AccessorySlotType.FunctionalSlot:
                Main.hoverItemName = WingSlotSystem.wings.Value;
                break;
            case AccessorySlotType.VanitySlot:
                Main.hoverItemName = WingSlotSystem.socialWings.Value;
                break;
            case AccessorySlotType.DyeSlot:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context), context, null);
        }
    }
}

public class WingSlotSystem : ModSystem, ILocalizedModType {

    public string LocalizationCategory => "Wings";

    public static LocalizedText wings = null!;
    public static LocalizedText socialWings = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.wingSlot;
    }

    public override void SetStaticDefaults() {
        ZenithQoL.instance.Logger.Warn("Hey!");
        wings = this.GetLocalization(nameof(wings));
        socialWings = this.GetLocalization(nameof(socialWings));
    }

    public override void Load() {
        IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += itemSlotDrawColourFixPatch;
    }

    public override void Unload() {
    }

    // [1553 13 - 1553 109]
    // IL_038e: callvirt     instance class [FNA]Microsoft.Xna.Framework.Graphics.Texture2D Terraria.ModLoader.AccessorySlotLoader::GetBackgroundTexture(int32, int32)
    // IL_0393: stloc.s      backgroundTexture
    public void itemSlotDrawColourFixPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        var backgroundTexture = 0;
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt<AccessorySlotLoader>("GetBackgroundTexture"),
                i => i.MatchStloc(out backgroundTexture))) {
            ZenithQoL.instance.Logger.Warn(
                "Couldn't match AccessorySlotLoader.GetBackgroundTexture in ItemSlot.Draw!");
        }

        // replace it with ours
        // color2 is 8
        // [1553 13 - 1553 109]
        //IL_0387: call         !!0/*class Terraria.ModLoader.AccessorySlotLoader*/ Terraria.ModLoader.LoaderManager::Get<class Terraria.ModLoader.AccessorySlotLoader>()
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdarg2();
        ilCursor.EmitCall<WingSlotSystem>("getColour");
        ilCursor.Emit(OpCodes.Stloc_S, (byte)8);
        ilCursor.EmitCall<WingSlotSystem>("getLoader");
        ilCursor.EmitLdarg3();
        ilCursor.EmitLdarg2();
        ilCursor.EmitCall<WingSlotSystem>("getTexture");
        ilCursor.EmitStloc(backgroundTexture);
    }

    public static AccessorySlotLoader getLoader() {
        return LoaderManager.Get<AccessorySlotLoader>();
    }

    public static Color getColour(int slot, int context) {
        return GetColorByLoadout(slot, context);
    }

    public static Texture2D getTexture(AccessorySlotLoader loader, int slot, int context) {
        ModAccessorySlot modAccessorySlot = loader.Get(slot);
        switch (context) {
            case -12:
                return ModContent.RequestIfExists(modAccessorySlot.DyeBackgroundTexture, out Asset<Texture2D> asset1)
                    ? asset1.Value
                    : TextureAssets.InventoryBack13.Value;
            case -11:
                return ModContent.RequestIfExists(modAccessorySlot.VanityBackgroundTexture, out Asset<Texture2D> asset2)
                    ? asset2.Value
                    : TextureAssets.InventoryBack13.Value;
            case -10:
                return ModContent.RequestIfExists(modAccessorySlot.FunctionalBackgroundTexture,
                    out Asset<Texture2D> asset3)
                    ? asset3.Value
                    : TextureAssets.InventoryBack13.Value;
            default:
                return TextureAssets.InventoryBack13.Value;
        }
    }

    // copied vanilla code
    public static Color GetColorByLoadout(int slot, int context) {
        var _lastTimeForVisualEffectsThatLoadoutWasChanged = (double)typeof(ItemSlot)
            .GetField("_lastTimeForVisualEffectsThatLoadoutWasChanged", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null)!;
        Color color1 = Color.White;
        Color color2;
        if (TryGetSlotColor(Main.LocalPlayer.CurrentLoadoutIndex, context, out color2))
            color1 = color2;
        Color color3 = new Color(color1.ToVector4() * Main.inventoryBack.ToVector4());
        float num = Terraria.Utils.Remap(
            (float)(Main.timeForVisualEffects - _lastTimeForVisualEffectsThatLoadoutWasChanged), 0.0f, 30f, 0.5f, 0.0f);
        Color white = Color.White;
        double amount = num * num * num;
        return Color.Lerp(color3, white, (float)amount);
    }

    public static bool TryGetSlotColor(int loadoutIndex, int context, out Color color) {
        var LoadoutSlotColors = (Color[,])typeof(ItemSlot)
            .GetField("LoadoutSlotColors", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null)!;
        color = new Color();
        if (loadoutIndex < 0 || loadoutIndex >= 3)
            return false;
        int index = -1;
        switch (context) {
            case 8:
            case 10:
            case -10:
                index = 0;
                break;
            case 9:
            case 11:
            case -11:
                index = 1;
                break;
            case 12:
            case -12:
                index = 2;
                break;
        }

        if (index == -1)
            return false;
        color = LoadoutSlotColors[loadoutIndex, index];
        return true;
    }
}