using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ZenithQoL.UI;

public class NPCTagHandler : ITagHandler {
    public TextSnippet Parse(string text, Color baseColor = new Color(), string? options = null) {
        // first try the id
        int result;
        if (int.TryParse(text, out result) && result < NPCLoader.NPCCount && result > NPCID.NegativeIDCount) {
        }
        else {
            // if it didn't work, return 0
            result = 0;
        }

        if (NPCID.Search.ContainsName(text)) {
            result = NPCID.Search.GetId(text);
        }

        if (result <= 0) {
            return new TextSnippet(text);
        }

        var noTooltip = false;
        var body = false;
        if (options?.Contains('n') ?? false) {
            noTooltip = true;
        }
        else if (options?.Contains('f') ?? false) {
            body = true;
        }

        var npcSnippet = new NPCSnippet(result, noTooltip, body) {
            Text = GenerateTag(result),
            CheckForHover = true,
            DeleteWhole = true
        };
        return npcSnippet;
    }

    public static string GenerateTag(int netID) {
        return $"[npc:{netID}]";
    }

    public class NPCSnippet : TextSnippet {
        private int netID;
        private NPC npcClass;
        private bool noTooltip;
        private bool body;

        public NPCSnippet(int netID, bool noTooltip = false, bool body = false) {
            this.netID = netID;
            this.noTooltip = noTooltip;
            this.body = body;
            npcClass = ContentSamples.NpcsByNetId[netID];
        }

        public override void OnHover() {
            if (!noTooltip) {
                Main.instance.MouseText(Lang.GetNPCNameValue(netID));
            }
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
            Vector2 position = default, Color color = default, float scale = 1f) {
            float num1 = 24f * scale;
            Texture2D texture;
            var r = new Rectangle();
            var npcType = npcClass.type;
            // if not dedicated server
            if (Main.netMode != NetmodeID.Server && !Main.dedServ) {
                Main.instance.LoadNPC(npcType);
                int defaultHeadIndex = NPC.TypeToDefaultHeadIndex(npcType);
                // if body isn't selected + head is found
                // if head is not found, just render body, fuck it
                if (!body && defaultHeadIndex != -1) {
                    texture = TextureAssets.NpcHead[defaultHeadIndex].Value;
                    r = texture.Bounds;
                }
                else {
                    texture = TextureAssets.Npc[npcType].Value;
                    ref Rectangle local = ref r;
                    int width = texture.Width;
                    int height = texture.Height / Main.npcFrameCount[npcType];
                    local = new Rectangle(0, 0, width, height);
                }

                if (r.Width * scale > num1 || r.Height * scale > num1)
                    scale = r.Width <= r.Height ? num1 / r.Height : num1 / r.Width;


                if (!justCheckingString && color != Color.Black) {
                    NPC npc = ContentSamples.NpcsByNetId[netID];
                    Color white = Color.White;
                    // if in game, we can draw normally
                    if (Main.LocalPlayer != null && Main.LocalPlayer.ModPlayers.Length > 0) {
                        Main.spriteBatch.Draw(texture, position + new Vector2(num1 / 2f), r,
                            npc.GetAlpha(white), 0.0f, r.Center(), scale, SpriteEffects.None, 0.0f);
                        if (npc.color != new Color()) {
                            Main.spriteBatch.Draw(texture, position + new Vector2(num1 / 2f), r,
                                npc.GetColor(white), 0.0f, r.Center(), scale, SpriteEffects.None, 0.0f);
                        }
                    }
                    else {
                        Main.spriteBatch.Draw(texture, position + new Vector2(num1 / 2f), r,
                            white, 0.0f, r.Center(), scale, SpriteEffects.None, 0.0f);
                    }
                }
            }

            size = r.Size() * scale + new Vector2(2f, 0.0f);
            return true;
        }

        public override float GetStringLength(DynamicSpriteFont font) {
            return 20.8f * Scale;
        }
    }
}