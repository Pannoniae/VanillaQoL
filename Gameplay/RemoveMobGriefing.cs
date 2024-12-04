using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

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

        if (projectile.owner == Main.myPlayer && !projectile.friendly) {
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
        }

        return true;
    }
}

public class RemoveTombstones : GlobalProjectile {
    public static Dictionary<int, int> map = new() {
        { 43, 321 }, { 201, 1173 }, { 202, 1174 }, { 203, 1175 }, { 204, 1176 }, { 205, 1177 }, { 527, 3229 },
        { 528, 3230 }, { 529, 3231 }, { 530, 3232 }, { 531, 3233 }
    };

    public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) {
        return map.ContainsKey(projectile.type);
    }

    public override bool PreAI(Projectile projectile) {
        if (QoLConfig.Instance.noDroppedTombstones) {
            projectile.noDropItem = true;
            if (projectile.owner == Main.myPlayer) {
                int Type = map[projectile.type];
                int number = Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Hitbox, Type);
                Main.item[number].noGrabDelay = 0;
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: number, number2: 1f);
                }
            }

            projectile.Kill();
            return false;
        }

        return true;
    }
}