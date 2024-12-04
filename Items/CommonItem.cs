using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ZenithQoL.Items;

public class CommonItem {
    public static void nightsEdgeEffects(Player player, Rectangle hitbox) {
        if (Main.rand.NextBool(5)) {
            Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width,
                hitbox.Height, DustID.Demonite, player.direction * 2, Alpha: 150, Scale: 1.4f);
        }

        int index = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width,
            hitbox.Height, DustID.Shadowflame, player.velocity.X * 0.2f + player.direction * 3, player.velocity.Y * 0.2f, 100,
            Scale: 1.2f);
        Main.dust[index].noGravity = true;
        Main.dust[index].velocity.X /= 2f;
        Main.dust[index].velocity.Y /= 2f;
    }

    public static void excaliburEffects(Player player, Rectangle hitbox) {
        if (Main.rand.NextBool(3)) {
            int index = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Enchanted_Gold,
                player.velocity.X * 0.2f + player.direction * 3, player.velocity.Y * 0.2f, 100, Scale: 1.1f);
            Main.dust[index].noGravity = true;
            Main.dust[index].velocity.X /= 2f;
            Main.dust[index].velocity.Y /= 2f;
            Main.dust[index].velocity.X += player.direction * 2;
        }

        if (Main.rand.NextBool(4)) {
            int index = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.TintableDustLighted,
                Alpha: 254, Scale: 0.3f);
            Main.dust[index].velocity *= 0.0f;
        }
    }
}