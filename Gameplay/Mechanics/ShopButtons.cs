using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace VanillaQoL.Gameplay;

public class ShopButtons : GameInterfaceLayer {


    public ShopButtons()
        : base("Vanilla+ QoL: Shop Buttons", InterfaceScaleType.UI) {
    }

    protected override bool DrawSelf() {
        if (Main.npcShop <= 0) {
            return true;
        }

        var shop = ShopExpander.instance;

        // time for some maths!
        // here is the vanilla code for drawing the shop. now we only need to solve for 0 and 40 ;)
        // int x = (int) (73.0 + (double) (index * 56) * (double) Main.inventoryScale);
        // int y2 = (int) ((double) this.invBottom + (double) (index * 56) * (double) Main.inventoryScale);
        // size of the thing
        var factor = 0.755f; // Main.inventoryScale at that point
        var size = TextureAssets.InventoryBack6.Size() * factor;

        int width = TextureAssets.CraftUpButton.Width();
        int height = TextureAssets.CraftUpButton.Height();

        // good luck to all you fuckers copypasting my code without understanding it;)
        int minX = 73 - TextureAssets.ScrollLeftButton.Value.Width;
        int maxX = (int)(73 + 10 * 56 * Main.inventoryScale);
        int minY = Main.instance.invBottom;
        // shift down on the last row so it's on the bottom of the last row
        int maxY = (int)(Main.instance.invBottom + 3 * 56 * Main.inventoryScale + size.Y - height);
        bool changed = false;
        if (shop.page > 0) {
            Rectangle scrollLeft = new Rectangle(maxX, maxY - height - 4, width, height);
            // highlight is white, else slightly opaque
            Color color = Color.White * 0.8f;
            if (scrollLeft.Contains(Main.MouseScreen.ToPoint())) {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    shop.page--;
                    changed = true;
                }
                color = Color.White;
            }

            Main.spriteBatch.Draw(TextureAssets.CraftUpButton.Value, scrollLeft, color);
        }

        if (shop.page < shop.pageCount - 1) {
            Rectangle scrollRight = new Rectangle(maxX, maxY, width, height);
            Color color = Color.White * 0.8f;
            if (scrollRight.Contains(Main.MouseScreen.ToPoint())) {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    shop.page++;
                    changed = true;
                }

                color = Color.White;
            }

            Main.spriteBatch.Draw(TextureAssets.CraftDownButton.Value, scrollRight, color);
        }

        if (changed) {
            shop.refresh();
            SoundEngine.PlaySound(in SoundID.MenuTick);
        }

        return true;
    }
}