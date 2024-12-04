using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class DisableShimmering : ModPlayer {
    public override void PostUpdateEquips() {
        if (QoLConfig.Instance.disableShimmering) {
            Player.shimmerImmune = true;
        }
    }
}