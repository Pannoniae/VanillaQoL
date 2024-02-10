using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.Items;

namespace VanillaQoL.Tiles;

public class CrimsonPylonTile  : BasePylonTile {
    public LocalizedText inCrimson;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.morePylons;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        inCrimson = this.GetLocalization(nameof(inCrimson));
    }

    public override Color LightColor => new Color(1f, 0.2f, 0f);
    public override int AssociatedItem => ModContent.ItemType<CrimsonPylon>();
    public override Color PylonMapColor => Color.OrangeRed;
    public override Color DustColor => Color.OrangeRed;

    public override NPCShop.Entry GetNPCShopEntry() {
        Condition biomeCondition =
            new Condition(inCrimson, () => Main.LocalPlayer.ZoneCrimson);
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
            return _sceneMetrics.EnoughTilesForCrimson;
        }
        catch {
            return false;
        }
    }
}