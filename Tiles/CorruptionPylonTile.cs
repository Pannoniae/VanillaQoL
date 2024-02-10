using CalamityMod.Items.Placeables.Pylons;
using CalamityMod.Tiles.Pylons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.Items;

namespace VanillaQoL.Tiles;

public class CorruptionPylonTile : BasePylonTile {

    public LocalizedText inCorruption;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.morePylons;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        inCorruption = this.GetLocalization(nameof(inCorruption));
    }

    public override Color LightColor => new Color(1f, 0.2f, 0.7f);
    public override int AssociatedItem => ModContent.ItemType<CorruptionPylon>();
    public override Color PylonMapColor => Color.MediumPurple;
    public override Color DustColor => Color.MediumPurple;

    public override NPCShop.Entry GetNPCShopEntry() {
        Condition biomeCondition =
            new Condition(inCorruption, () => Main.LocalPlayer.ZoneCorrupt);
        return new NPCShop.Entry(AssociatedItem, biomeCondition);
    }

    public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) =>
        validDestination(pylonInfo);

    private static bool validDestination(TeleportPylonInfo info) {
        var _sceneMetrics = new SceneMetrics();
        try
        {
            _sceneMetrics.ScanAndExportToMain(new SceneMetricsScanSettings()
            {
                VisualScanArea = new Rectangle?(),
                BiomeScanCenterPositionInWorld = new Vector2?(info.PositionInTiles.ToWorldCoordinates()),
                ScanOreFinderData = false
            });
            return _sceneMetrics.EnoughTilesForCorruption;
        }
        catch {
            return false;
        }
    }
}