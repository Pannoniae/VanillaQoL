using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DisableShimmering : ModPlayer {
    public override void PostUpdateEquips() {
        if (QoLConfig.Instance.disableShimmering) {
            Player.shimmerImmune = true;
        }
    }
}