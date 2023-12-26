using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class MountFallDamage : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.mountFallDamage;
    }

    public override void PostSetupContent() {
        var mounts = Mount.mounts;
        foreach (var mount in mounts) {
            mount.fallDamage = 0f;
        }
    }
}