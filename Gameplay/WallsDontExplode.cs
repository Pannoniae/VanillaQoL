using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class WallsDontExplode : GlobalWall {
    public override bool CanExplode(int i, int j, int type) {
        if (QoLConfig.Instance.wallsDontExplode && Constants.explosionProofWalls.Contains(type)) {
            return false;
        }

        return true;
    }
}