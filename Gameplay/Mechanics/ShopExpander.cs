using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace VanillaQoL.Gameplay;

public class ShopExpander : ModSystem {
    public static ShopExpander instance = null!;

    public int page;
    public int pageCount;

    public List<Item> items = null!;

    public Item[] currentItems = new Item[Chest.maxItems];

    public int pageidx(int index) {
        return page * 40 + index;
    }

    public bool notIn(int index) {
        return index < 0 || index >= items.Count;
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.shopExpander;
    }

    public override void Load() {
        instance = this;
        IL_Chest.SetupShop_string_NPC += shopExpanderPatch;
        if (QoLConfig.Instance.extraSellPages) {
            IL_Chest.AddItemToShop += sellPagesPatch;
        }
    }

    public override void Unload() {
        instance = null!;
        IL_Chest.SetupShop_string_NPC -= shopExpanderPatch;
        IL_Chest.AddItemToShop -= sellPagesPatch;
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
        if (rem > 0) {
            for (int i = 0; i < rem; i++) {
                items.Add(new Item());
            }
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
        // fill it up multiples of 40
        var rem = 40 * pageCount - items.Count;
        if (rem > 0) {
            for (int i = 0; i < rem; i++) {
                items.Add(new Item());
            }
        }

        var index = page * Chest.maxItems;
        var end = index + Chest.maxItems;
        var ctr = 0;
        for (int i = index; i < end; i++) {
            currentItems[ctr] = items[i];

            ctr++;
        }
    }

    public void sellPagesPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdarg1();
        ilCursor.Emit<ShopExpander>(OpCodes.Call, "hijackAddItem");
        ilCursor.EmitRet();
    }

    public static int hijackAddItem(Chest self, Item newItem) {
        int amount = Main.shopSellbackHelper.Remove(newItem);
        if (amount >= newItem.stack) {
            return 0;
        }

        for (int shop = 0; shop < 39; ++shop) {
            if (instance.notIn(instance.pageidx(shop)) || instance.items[instance.pageidx(shop)] == null! ||
                instance.items[instance.pageidx(shop)].type == ItemID.None) {
                if (instance.notIn(instance.pageidx(shop))) {
                    instance.items.Add(newItem.Clone());
                }
                else {
                    instance.items[instance.pageidx(shop)] = newItem.Clone();
                }

                instance.items[instance.pageidx(shop)].favorited = false;
                instance.items[instance.pageidx(shop)].buyOnce = true;
                instance.items[instance.pageidx(shop)].stack -= amount;
                //int value = instance.items[shop].value;
                instance.refresh();
                return shop;
            }
        }

        // if we are selling on first page and we don't have more space, expand
        // this checks the next page's first item, if it's not in the list, expand
        if (instance.notIn(instance.pageidx(0) + 40)) {
            // if still not found (i = 40), expand shop
            instance.pageCount += 1;
        }

        // TODO don't allow putting items into the 40th slot after the first page as well
        // instead of items.Add, make items properly expand to 40 after each refresh
        instance.page = instance.pageCount - 1;
        instance.refresh();
        for (int shop = 0; shop < 39; ++shop) {
            if (instance.notIn(instance.pageidx(shop)) || instance.items[instance.pageidx(shop)] == null! ||
                instance.items[instance.pageidx(shop)].type == ItemID.None) {
                if (instance.notIn(instance.pageidx(shop))) {
                    instance.items.Add(newItem.Clone());
                }
                else {
                    instance.items[instance.pageidx(shop)] = newItem.Clone();
                }

                instance.items[instance.pageidx(shop)].favorited = false;
                instance.items[instance.pageidx(shop)].buyOnce = true;
                instance.items[instance.pageidx(shop)].stack -= amount;
                //int value = instance.items[shop].value;
                instance.refresh();
                return shop;
            }
        }

        return 0;
    }
}