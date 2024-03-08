using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

public class PricesUIState : UIState
{
    protected bool visible = false;

    public static Dictionary<string, PriceElement> npcUIPairs = new();

    public override void Update(GameTime gameTime)
    {
        UpdateTextUI();
        base.Update(gameTime);
    }

    public void Show()
    {
        if (visible) return;
        this.visible = true;
        UpdateTextUI();
    }

    public void Hide()
    {
        if (!visible) return;
        this.visible = false;
        ClearPrices();
    }

    public void ClearPrices()
    {
        RemoveAllChildren();
        npcUIPairs.Clear();
    }

    public void UpdateTextUI()
    {
        if (!visible) return;
        foreach (NPC npc in Main.npc.Where(n => n.isLikeATownNPC && n.friendly && NPCShopDatabase.AllShops.Any(s => s.NpcType == n.type)))
        {
            if (!npcUIPairs.ContainsKey(npc.FullName))
            {
                PriceElement npcPriceText = new(npc);
                npcUIPairs.Add(npc.FullName, npcPriceText);
                Append(npcUIPairs[npc.FullName]);
            }
            npcUIPairs[npc.FullName].UpdatePrice();
            npcUIPairs[npc.FullName].Left.Set(npc.position.ToScreenPosition().X + UIConfig.Instance.npcSellPriceXOffset, 0f);
            npcUIPairs[npc.FullName].Top.Set(npc.position.ToScreenPosition().Y + UIConfig.Instance.npcSellPriceYOffset, 0f);
        }
    }

    protected override void DrawChildren(SpriteBatch spriteBatch)
    {
        base.DrawChildren(spriteBatch);
    }

}
