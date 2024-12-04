using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZenithQoL.Tiles;

namespace ZenithQoL.Items;

public class CorruptionPylon : ModItem, ILocalizedModType {
    public new string LocalizationCategory => "Items.Placeables";

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.morePylons;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<CorruptionPylonTile>());

        Item.value = Item.buyPrice(0, 10, 0, 0);
        Item.rare = ItemRarityID.Blue;
    }
}