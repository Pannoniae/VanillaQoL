using System.Collections.Generic;
using CalamityMod.Items.Tools;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class RemoveMobGriefingSimple : GlobalProjectile {
    public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
        return projectile.type is ProjectileID.SandBallFalling or ProjectileID.SnowBallHostile;
    }

    public override bool PreKill(Projectile projectile, int timeLeft) {
        if (projectile.type == ProjectileID.SandBallFalling && !projectile.friendly &&
            QoLConfig.Instance.noDroppedSand) {
            projectile.noDropItem = true;
        }

        if (projectile.type == ProjectileID.SnowBallHostile && QoLConfig.Instance.noDroppedSnow) {
            projectile.noDropItem = true;
        }

        return true;
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        int item = 0;
        switch (projectile.type) {
            case ProjectileID.SnowBallHostile:
                item = ItemID.SnowBlock;
                break;
            case ProjectileID.SandBallFalling:
                item = ItemID.SandBlock;
                break;
        }

        if (projectile.owner == Main.myPlayer) {
            int id = Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Hitbox, item);
            Main.item[id].noGrabDelay = 0;
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.SyncItem, number: id, number2: 1f);
            }
        }
    }
}

public class RemoveMobGriefing : GlobalProjectile {
    public static Dictionary<int, int> map = new() {
        { 42, 169 }, { 65, 370 }, { 68, 408 }, { 354, 1246 }
    };

    public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
        return map.ContainsKey(projectile.type);
    }

    public override bool PreKill(Projectile projectile, int timeLeft) {
        if (QoLConfig.Instance.noDroppedSandgun) {
            projectile.noDropItem = true;
            int id = Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Hitbox, map[projectile.type]);
            Main.item[id].noGrabDelay = 0;
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.SyncItem, number: id, number2: 1f);
            }
        }

        return true;
    }
}