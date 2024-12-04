using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ZenithQoL.Items;

namespace ZenithQoL.Gameplay;

public class AddMobGriefing : GlobalProjectile {
    public override void OnKill(Projectile projectile, int timeLeft) {
        if (QoLConfig.Instance.clownExplosions) {
            if (projectile.type == ProjectileID.HappyBomb) {
                projectile.tileCollide = false;
                Explosives.explosionCode(projectile, 3);
            }
        }
    }
}