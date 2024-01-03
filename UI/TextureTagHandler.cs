using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace VanillaQoL.UI;

public class TextureTagHandler : ITagHandler {
    public TextSnippet Parse(string text, Color baseColor = new Color(), string? options = null) {
        string tooltip = "";
        int h = 0;
        int v = 0;
        if (options != null) {
            var splitOptions = options.Split(',');
            foreach (var option in splitOptions) {
                if (option.StartsWith("t")) {
                    tooltip = option[1..];
                }
                else if (option.StartsWith("h")) {
                    var s = int.TryParse(option[1..], out h);
                }
                else if (option.StartsWith("v")) {
                    var s = int.TryParse(option[1..], out v);
                }
            }
        }
        if ((options?.Length ?? 0) > 0) {
            tooltip = options!;
        }
        var result = ModContent.RequestIfExists(text, out Asset<Texture2D> a, AssetRequestMode.ImmediateLoad);

        if (!result) {
            return new TextSnippet(text);
        }

        var textureSnippet = new TextureSnippet(text, tooltip, h, v) {
            Text = GenerateTag(text),
            CheckForHover = true,
            DeleteWhole = true
        };
        return textureSnippet;
    }

    public static string GenerateTag(string name) {
        return $"[t:{name}]";
    }

    public class TextureSnippet : TextSnippet {
        private string tooltip;
        private int v;
        private int h;

        private int width;
        private int height;

        private Asset<Texture2D> asset;

        public TextureSnippet(string name, string tooltip = "", int h = 0, int v = 0) {
            this.tooltip = tooltip;
            this.h = h;
            this.v = v;
            ModContent.RequestIfExists(name, out Asset<Texture2D> a, AssetRequestMode.ImmediateLoad);
            asset = a;
            var texture = asset.Value;
            width = texture.Width;
            height = texture.Height;
        }

        public override void OnHover() {
            if (tooltip != "") {
                Main.instance.MouseText(tooltip);
            }
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
            Vector2 position = default, Color color = default, float scale = 1f) {
            float num1 = 24f * scale;
            var r = new Rectangle();
            // if not dedicated server
            if (Main.netMode != NetmodeID.Server && !Main.dedServ) {

                r = new Rectangle(0, 0, width, height);


                if (r.Width * scale > num1 || r.Height * scale > num1) {
                    scale = r.Width <= r.Height ? num1 / r.Height : num1 / r.Width;
                }

                var half = new Vector2(r.Width / 2f * scale);

                if (!justCheckingString && color != Color.Black) {
                    Color white = Color.White;

                    Main.spriteBatch.Draw(asset.Value, position + half + new Vector2(v, h), r,
                        white, 0.0f, r.Center(), scale, SpriteEffects.None, 0.0f);
                }
            }

            size = r.Size() * scale;
            return true;
        }

        public override float GetStringLength(DynamicSpriteFont font) {
            return width * Scale;
        }
    }
}