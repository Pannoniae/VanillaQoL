using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using System;

public class PriceElement : UIElement
{
    static readonly UIConfig config = GetInstance<UIConfig>();

    const int LINE_SPACING = -10;
    const int ICON_TEXT_PADDING = 5;
    private long price;
    private double adjustAmount = 1;
    private Coin[] coins = new Coin[4];
    private NPC npc;
    private Color bgColor, textColor;
    private int xOffset,
            totalHeight,
            maxLeft = 0,
            maxRight;

    private struct Coin
    {
        public long value;
        public Texture2D texture;
        public Color textColor;
        public Coin(int j)
        {
            value = 0;
            texture = TextureAssets.Item[71 + j].Value;
            switch (j)
            {
                case 0:
                    textColor = Colors.CoinCopper;
                    break;
                case 1:
                    textColor = Colors.CoinSilver;
                    break;
                case 2:
                    textColor = Colors.CoinGold;
                    break;
                case 3:
                    textColor = Colors.CoinPlatinum;
                    break;
                default:
                    textColor = Color.White;
                    break;
            }
        }
    }
    

    public PriceElement(NPC npc)
    {
        this.npc = npc;
    }

    public override void OnInitialize()
    {
        for (int i = 0; i < coins.Length; i++)
        {
            coins[i] = new Coin(3-i);
        }
        SetPrice(0);
        base.OnInitialize();
    }

    internal void SetPrice(long price)
    {
        if (this.price == price) return;
        this.price = price;
        var coinPrices = Utils.CoinsSplit(price);
        for (int i = 0; i < coinPrices.Length; i++)
        {
            coins[i].value = coinPrices[3-i];
        }
        UpdateColors();
    }

    public void UpdatePrice()
    {
        Item item = Main.HoverItem;
        if (!Main.keyState.IsKeyDown(Main.FavoriteKey) || Main.HoverItem.type == ItemID.None)
        {
            SetPrice(0);
            return;
        }
        // Get price per npc happiness
        ShoppingSettings tempSettings = Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, npc);
        adjustAmount = 1 / tempSettings.PriceAdjustment;
        Main.LocalPlayer.GetItemExpectedPrice(item, out long calcForSelling, out _);

        // Pre-Adjust for when player is already in a shop
        double itemPrice = calcForSelling / 5.0;
        if (!Main.LocalPlayer.currentShoppingSettings.Equals(ShoppingSettings.NotInShop))
        {
            itemPrice *= Main.LocalPlayer.currentShoppingSettings.PriceAdjustment;
        }

        // Adjust for NPC happiness
        itemPrice /= tempSettings.PriceAdjustment;

        SetPrice((long)(item.stack * Math.Round(itemPrice)));
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (price == 0) return;

        //Draw background
        if (config.npcSellPriceShowBackground && config.npcSellPriceBackgroundColor.A != 0)
        {
            var bgRect = GetDimensions().ToRectangle();
            bgRect.X -= maxLeft;
            bgRect.Inflate(config.npcSellPriceBackgroundPadding, config.npcSellPriceBackgroundPadding);
            Utils.DrawInvBG(spriteBatch, bgRect, bgColor);
        }


        //Draw coins
        totalHeight = 0;
        maxLeft = 0;
        maxRight = 0;

        foreach (Coin coin in coins)
        {
            if (coin.value == 0) continue;

            var coinText = coin.value.ToString();

            var coinTextSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, coinText, Vector2.One);
            int xOffset = (int)coinTextSize.X;
            Vector2 textPosition = new Vector2(Left.Pixels - xOffset, Top.Pixels + totalHeight);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, coinText, textPosition, coin.textColor, 0, Vector2.Zero, Vector2.One);

            if (config.showCoinIcon)
            {
                Vector2 iconPosition = new Vector2(Left.Pixels + ICON_TEXT_PADDING, Top.Pixels + totalHeight + (coinTextSize.Y + LINE_SPACING - coin.texture.Height) / 2);
                spriteBatch.Draw(coin.texture, iconPosition, Color.White);
                maxRight = Math.Max(coin.texture.Width + ICON_TEXT_PADDING, maxRight);
            }

            maxLeft = Math.Max(xOffset, maxLeft);
            totalHeight += (int)coinTextSize.Y + LINE_SPACING;
        }

        // Draw price multiplier
        if (config.showPriceMultiplier)
        {
            string valueText = ((int)(adjustAmount * 100)) + "%";
            var valueTextSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, valueText, Vector2.One);
            int xOffset = (int)valueTextSize.X;
            Vector2 textPosition = new Vector2(Left.Pixels - xOffset / 2, Top.Pixels + totalHeight);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, valueText, textPosition, textColor, 0, Vector2.Zero, Vector2.One);

            maxLeft = Math.Max(xOffset / 2, maxLeft);
            maxRight = Math.Max(xOffset / 2, maxRight);
            totalHeight += (int)valueTextSize.Y + LINE_SPACING;
        }

        Width.Set(maxLeft + maxRight, 0f);
        Height.Set(totalHeight, 0f);
    }

    //Adjust alphas and dynamic colors based on npc happiness.
    internal void UpdateColors()
    {
        if (config.npcSellPriceShowBackground)
        {
            Color bgc;
            if (config.dynamicBackgroundColor)
                bgc = AdjustColor(config.npcSellPriceBackgroundColor, adjustAmount);
            else
                bgc = config.npcSellPriceBackgroundColor;

            bgColor = AdjustAlpha(bgc);
        }
        if (config.showPriceMultiplier)
        {
            Color txc = AdjustColor(Color.LightGray, adjustAmount);
            textColor = AdjustAlpha(txc);
        }
    }

    //unhappy = more red; happy = more green
    internal Color AdjustColor(Color c, double adjustAmount)
    {
        if (adjustAmount > 1)
        {
            return new(
                (byte)(c.R / adjustAmount),
                (byte)Math.Min(c.G * (adjustAmount), 255),
                (byte)(c.B / (adjustAmount)),
                c.A);
        }
        else
        {
            return new(
                (byte)Math.Min(c.R / (adjustAmount), 255),
                (byte)(c.G * (adjustAmount)),
                (byte)(c.B * (adjustAmount)),
                c.A);
        }
    }
    //transparency hack
    internal Color AdjustAlpha(Color c)
    {
        return new((byte)(c.R * c.A / 255.0f), (byte)(c.G * c.A / 255.0f), (byte)(c.B * c.A / 255.0f), c.A);
    }
}
