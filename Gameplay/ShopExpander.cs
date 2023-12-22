using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using VanillaQoL.Config;
using Terraria.ID;

namespace VanillaQoL.Gameplay;

public class ShopExpander : ModSystem {
    public static ShopExpander instance = null!;

    public int page;
    public int pageCount;

    public List<Item> items = null!;

    public Item[] currentItems = new Item[Chest.maxItems];

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.shopExpander;
    }

    public override void Load() {
        instance = this;
        IL_Chest.SetupShop_string_NPC += shopExpanderPatch;
    }

    public override void Unload() {
        instance = null!;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        var index = layers.FindIndex((Predicate<GameInterfaceLayer>)(layer => layer.Name.Equals("Vanilla: Inventory")));
        if (index == -1) {
            return;
        }

        layers.Insert(index + 1, new ShopButtons());
    }

    public void shopExpanderPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.EmitLdarg2();
        ilCursor.Emit<ShopExpander>(OpCodes.Call, "hijackSetupShop");
        ilCursor.EmitRet();
    }

    public static void hijackSetupShop(Chest self, string shopName, NPC? npc) {
        // fill with empty first
        Array.Fill(self.item, null);
        var items = new List<Item?>();
        if (NPCShopDatabase.TryGetNPCShop(shopName, out var shop)) {
            shop.FillShop(items, npc);
        }

        // fill it up to 40 for idiot mods who strictly expect a 40-item array
        var rem = 40 - items.Count;
        for (int i = 0; i < rem; i++) {
            items.Add(new Item());
        }

        // time for lots of copying
        var itemsToModify = items.ToArray();
        if (npc != null) {
            NPCLoader.ModifyActiveShop(npc, shopName, itemsToModify);
        }
        // copy the array back lol
        items = new List<Item?>(itemsToModify);

        // this is unsafe as fuck :(
        foreach (ref var item in CollectionsMarshal.AsSpan(items)) {
            item ??= new Item();
            item.isAShopItem = true;
        }

        // not nullable anymore, we've got rid of the nulls
        instance.items = items!;
        instance.loadPage();

        // we hijack this chest/shop to our custom page
        self.item = instance.currentItems;
    }

    private void loadPage() {
        page = 0;
        pageCount = (int)Math.Ceiling(items.Count / 40.0);
        // special case for empty pages, the last item must be filled
        if (items.Count % 40 == 0 && items[Chest.maxItems - 1].type != ItemID.None) {
            pageCount += 1;
        }
        refresh();
    }

    public void refresh() {
        var index = page * Chest.maxItems;
        var end = index + Chest.maxItems;
        var ctr = 0;
        for (int i = index; i < end; i++) {
            if (i < items.Count) {
                currentItems[ctr] = items[i];
            }
            else {
                currentItems[ctr] = new Item();
            }
            ctr++;
        }
    }
}