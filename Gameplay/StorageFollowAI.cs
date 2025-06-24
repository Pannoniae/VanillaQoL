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
            var player = Main.player[projectile.owner];
            var distance = Vector2.Distance(projectile.Center, player.Center);
            if (distance > 3000f) {
                projectile.Center = player.Top;
            }
            else if (projectile.Center != player.Center) {
                var targetPosition = player.Center + projectile.DirectionFrom(player.Center) * 3f * 16f;
                
                // Check for other storage projectiles and push them away
                foreach (var otherProjectile in Main.projectile) {
                    if (otherProjectile.active && 
                        otherProjectile.whoAmI != projectile.whoAmI &&
                        otherProjectile.owner == projectile.owner &&
                        otherProjectile.type is ProjectileID.FlyingPiggyBank or ProjectileID.VoidLens) {
                        
                        var otherDistance = Vector2.Distance(projectile.Center, otherProjectile.Center);
                        if (otherDistance is < 64f and > 0f) {
                            var pushDirection = Vector2.Normalize(projectile.Center - otherProjectile.Center);
                            var pushForce = (64f - otherDistance) * 0.5f;
                            targetPosition += pushDirection * pushForce;
                        }
                    }
                }
                
                var val2 = (targetPosition - projectile.Center) / ((distance < 48f) ? 30f : 60f);
                projectile.position += val2;
            }

            if (projectile.timeLeft is < 2 and > 0) {
                projectile.timeLeft = 2;
            }
        }

        return base.PreAI(projectile);
    }
}