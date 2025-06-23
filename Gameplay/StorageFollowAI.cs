using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class StorageFollowAI : GlobalProjectile {
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.type is ProjectileID.FlyingPiggyBank or ProjectileID.VoidLens;
    }

    public override bool PreAI(Projectile projectile) {
        if (QoLConfig.Instance.storageFollowsPlayer) {
            Player player = Main.player[projectile.owner];
            float distance = Vector2.Distance(projectile.Center, player.Center);
            if (distance > 3000f) {
                projectile.Center = player.Top;
            }
            else if (projectile.Center != player.Center) {
                var val2 =
                    (player.Center + projectile.DirectionFrom(player.Center) * 3f * 16f - projectile.Center) /
                    ((distance < 48f) ? 30f : 60f);
                projectile.position += val2;
            }

            if (projectile.timeLeft < 2 && projectile.timeLeft > 0) {
                projectile.timeLeft = 2;
            }
        }

        return base.PreAI(projectile);
    }
}