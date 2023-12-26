using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

[SuppressMessage("ReSharper", "PossibleLossOfFraction")]
public class Explosives : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moreExplosives;
    }

    public override void Load() {
        IL_Projectile.BombsHurtPlayers += bombHurtPlayerPatch;
    }

    public override void Unload() {
        IL_Projectile.BombsHurtPlayers -= bombHurtPlayerPatch;
    }

    // IL_0006: ldc.i4.s     16 // 0x10
    // IL_0008: bne.un.s     IL_0086
    private void bombHurtPlayerPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        // if our condition is true, we jump after it
        var ilCursor2 = new ILCursor(ilCursor);
        // ldsfld System.Boolean[] Terraria.ID.ProjectileID/Sets::RocketsSkipDamageForPlayers
        if (!ilCursor2.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(out var _))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match the jump in Projectile.BombsHurtPlayers!");
            return;
        }

        // we inject straight at the beginning
        var target = ilCursor2.Next!;
        ilCursor.EmitLdarg0();
        ilCursor.Emit<Explosives>(OpCodes.Call, "isBomb");
        ilCursor.EmitBrtrue(target);
    }

    public static bool isBomb(Projectile proj) {
        return proj.ModProjectile != null &&
               Constants.explosives.Contains(proj.ModProjectile.Type);
    }

    public static void bounce(Projectile proj, Vector2 lastVelocity) {
        if (Math.Abs(proj.velocity.X - lastVelocity.X) > Constants.FLT_PROJ_TOLERANCE &&
            Math.Abs(lastVelocity.X) > 1f) {
            proj.velocity.X = lastVelocity.X * -0.9f;
            // if dynamite, apply dampening
            if (Constants.dynamites.Contains(proj.type)) {
                proj.velocity.X *= 0.9f;
            }
        }

        if (Math.Abs(proj.velocity.Y - lastVelocity.Y) > Constants.FLT_PROJ_TOLERANCE &&
            Math.Abs(lastVelocity.Y) > 1f) {
            proj.velocity.Y = lastVelocity.Y * -0.9f;
            if (Constants.dynamites.Contains(proj.type)) {
                proj.velocity.X *= 0.9f;
            }
        }
    }

    public static void explosiveCollide(Projectile proj, Vector2 lastVelocity) {
        // gravity
        if (Math.Abs(proj.velocity.X - lastVelocity.X) > Constants.FLT_PROJ_TOLERANCE) {
            proj.velocity.X = lastVelocity.X * -0.4f;
            if (Constants.dynamites.Contains(proj.type)) {
                proj.velocity.X *= 0.8f;
            }
        }

        if (Math.Abs(proj.velocity.Y - lastVelocity.Y) > Constants.FLT_PROJ_TOLERANCE && lastVelocity.Y > 0.7) {
            proj.velocity.Y = lastVelocity.Y * -0.4f;
            if (Constants.dynamites.Contains(proj.type)) {
                proj.velocity.Y *= 0.8f;
            }
        }
    }

    public static void NPCDamage(Projectile proj, NPC npc) {
        if (proj.timeLeft > 3)
            proj.timeLeft = 3;

        if (npc.position.X + npc.width / 2 < proj.position.X + proj.width / 2)
            proj.direction = -1;
        else
            proj.direction = 1;
    }

    public static void PlayerDamage(Projectile proj, Player player) {
        if (proj.timeLeft > 3)
            proj.timeLeft = 3;

        if (player.position.X + player.width / 2 < proj.position.X + proj.width / 2)
            proj.direction = -1;
        else
            proj.direction = 1;
    }

    public static void dynamiteAI(Projectile proj) {
        proj.velocity.X *= 0.8f;
        proj.velocity.Y *= 0.8f;
    }

    public static void explosiveAI(Projectile proj, int explosionWidth, int explosionHeight, int damage,
        float knockback) {
        // The projectile is in the midst of exploding during the last 3 updates.
        if (proj.owner == Main.myPlayer && proj.timeLeft <= 3) {
            proj.tileCollide = false;
            // Set to transparent. This projectile technically lives as transparent for about 3 frames
            proj.alpha = 255;
            proj.ai[1] = 0f;

            // change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
            proj.Resize(explosionWidth, explosionHeight);

            proj.damage = damage;
            proj.knockBack = knockback;
        }
        else {
            // dust
            if (Main.rand.NextBool(2)) {
                int num28 = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y), proj.width, proj.height,
                    DustID.Smoke, 0f, 0f, 100);
                Main.dust[num28].scale = 0.1f + Main.rand.Next(5) * 0.1f;
                Main.dust[num28].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[num28].noGravity = true;
                Main.dust[num28].position =
                    proj.Center + new Vector2(0f, -proj.height / 2).RotatedBy(proj.rotation) * 1.1f;
                var dust = DustID.Torch;
                if (Constants.dirtExplosives.Contains(proj.type)) {
                    dust = DustID.Dirt;
                }

                spawnDust(proj, dust);
            }

            // sticky
            if (Constants.stickyExplosives.Contains(proj.type)) {
                try {
                    int num3 = (int)(proj.position.X / 16f) - 1;
                    int num4 = (int)((proj.position.X + proj.width) / 16f) + 2;
                    int num5 = (int)(proj.position.Y / 16f) - 1;
                    int num6 = (int)((proj.position.Y + proj.height) / 16f) + 2;
                    if (num3 < 0)
                        num3 = 0;

                    if (num4 > Main.maxTilesX)
                        num4 = Main.maxTilesX;

                    if (num5 < 0)
                        num5 = 0;

                    if (num6 > Main.maxTilesY)
                        num6 = Main.maxTilesY;

                    Vector2 vector = default(Vector2);
                    for (int j = num3; j < num4; j++) {
                        for (int k = num5; k < num6; k++) {
                            if (Main.tile[j, k] == null || !Main.tile[j, k].HasUnactuatedTile ||
                                !Main.tileSolid[Main.tile[j, k].TileType] || Main.tileSolidTop[Main.tile[j, k].TileType])
                                continue;

                            vector.X = j * 16;
                            vector.Y = k * 16;
                            if (!(proj.position.X + proj.width - 4f > vector.X) || !(proj.position.X + 4f < vector.X + 16f) ||
                                !(proj.position.Y + proj.height - 4f > vector.Y) || !(proj.position.Y + 4f < vector.Y + 16f))
                                continue;

                            if (proj.type == 911 && proj.owner == Main.myPlayer && proj.localAI[0] == 0f) {
                                float num7 = 12f;
                                Vector2 value = vector + new Vector2(8f, 8f);
                                if (Vector2.Distance(proj.Center, value) < num7)
                                    proj.Center += proj.velocity.SafeNormalize(Vector2.Zero) * -4f;

                                proj.localAI[0] = 1f;
                                proj.netUpdate = true;
                            }

                            proj.velocity.X = 0f;
                            proj.velocity.Y = -0.2f;
                            //proj.flag = true;
                        }
                    }
                }
                catch (Exception e) {
                    VanillaQoL.instance.Logger.Error(e);
                }
            }

            // gravity
            if (proj.ai[0] > 5f) {
                proj.ai[0] = 10f;
                if (proj.velocity.Y == 0f && proj.velocity.X != 0f) {
                    proj.velocity.X *= 0.97f;
                    if (Constants.dynamites.Contains(proj.type)) {
                        proj.velocity.X *= 0.99f;
                    }

                    if (proj.velocity.X > -0.01 && proj.velocity.X < 0.01) {
                        proj.velocity.X = 0f;
                        proj.netUpdate = true;
                    }
                }

                proj.velocity.Y += 0.2f;
            }

            // rotation
            proj.ai[0] += 1f;
            proj.rotation += proj.velocity.X * 0.1f;
        }
    }

    public static void spawnDust(Projectile proj, int dust) {
        Dust dust8 = Dust.NewDustDirect(proj.position, proj.width, proj.height, dust, 0f, 0f, 100);
        dust8.scale = 1f + Main.rand.Next(5) * 0.1f;
        dust8.noGravity = true;
        dust8.position = proj.Center + new Vector2(0f, -proj.height / 2 - 6).RotatedBy(proj.rotation) * 1.1f;
    }

    public static void explosionCode(Projectile proj, int rad) {
        if (proj.owner == Main.myPlayer) {
            Vector2 position = proj.position;
            if (proj.type == 716 || proj.type == 718 || proj.type == 773)
                position = proj.Center;

            int minX = (int)(position.X / 16f - rad);
            int maxX = (int)(position.X / 16f + rad);
            int minY = (int)(position.Y / 16f - rad);
            int maxY = (int)(position.Y / 16f + rad);
            if (minX < 0)
                minX = 0;

            if (maxX > Main.maxTilesX)
                maxX = Main.maxTilesX;

            if (minY < 0)
                minY = 0;

            if (maxY > Main.maxTilesY)
                maxY = Main.maxTilesY;

            bool shouldWallExplode = proj.ShouldWallExplode(position, rad, minX, maxX, minY, maxY);
            proj.ExplodeTiles(position, rad, minX, maxX, minY, maxY, shouldWallExplode);
        }
    }

    public static void dirtExplosionCode(Projectile proj, int origWidth, int origHeight, float rad) {
        if (proj.owner == Main.myPlayer) {
            proj.Resize(origWidth, origHeight);
            SoundEngine.PlaySound(in SoundID.Item14, proj.position);
            Color transparent2 = Color.Transparent;
            for (int i = 0; i < 30; i++) {
                Dust smoke = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.Smoke, 0f, 0f, 100,
                    transparent2,
                    1.5f);
                smoke.velocity *= 1.4f;
            }

            for (int i = 0; i < 80; i++) {
                Dust dirt = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.Dirt, 0f, 0f, 100,
                    transparent2,
                    2.2f);
                dirt.noGravity = true;
                dirt.velocity.Y -= 1.2f;
                Dust dirt2 = dirt;
                dirt2.velocity *= 4f;
                dirt = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.Dirt, 0f, 0f, 100,
                    transparent2,
                    1.3f);
                dirt.velocity.Y -= 1.2f;
                dirt2 = dirt;
                dirt2.velocity *= 2f;
            }

            for (int num851 = 1; num851 <= 2; num851++) {
                for (int num852 = -1; num852 <= 1; num852 += 2) {
                    for (int num853 = -1; num853 <= 1; num853 += 2) {
                        Gore gore6 = Gore.NewGoreDirect(proj.GetSource_FromThis(), proj.position, Vector2.Zero,
                            Main.rand.Next(61, 64));
                        gore6.velocity *= num851 == 1 ? 0.4f : 0.8f;
                        gore6.velocity += new Vector2(num852, num853);
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Point pt = proj.Center.ToTileCoordinates();
                proj.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, rad,
                    DelegateMethods.SpreadDirt);
            }
        }
    }


    public static void explosionCode(Projectile proj) {
        var position = proj.position;
        SoundEngine.PlaySound(SoundID.Item14, position);
        position.X += proj.width / 2;
        position.Y += proj.height / 2;
        proj.width = 22;
        proj.height = 22;
        position.X -= proj.width / 2;
        position.Y -= proj.height / 2;

        for (int num951 = 0; num951 < 20; num951++) {
            int num952 = Dust.NewDust(new Vector2(position.X, position.Y), proj.width, proj.height, DustID.Smoke, 0f,
                0f, 100,
                default, 1.5f);
            Dust dust2 = Main.dust[num952];
            dust2.velocity *= 1.4f;
        }

        for (int num953 = 0; num953 < 10; num953++) {
            int num954 = Dust.NewDust(position, proj.width, proj.height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
            Main.dust[num954].noGravity = true;
            Dust dust2 = Main.dust[num954];
            dust2.velocity *= 5f;
            num954 = Dust.NewDust(position, proj.width, proj.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
            dust2 = Main.dust[num954];
            dust2.velocity *= 3f;
        }

        int num955 = Gore.NewGore(proj.GetSource_FromThis(), position, default, Main.rand.Next(61, 64));
        Gore gore2 = Main.gore[num955];
        gore2.velocity *= 0.4f;
        Main.gore[num955].velocity.X += 1f;
        Main.gore[num955].velocity.Y += 1f;
        num955 = Gore.NewGore(proj.GetSource_FromThis(), position, default, Main.rand.Next(61, 64));
        gore2 = Main.gore[num955];
        gore2.velocity *= 0.4f;
        Main.gore[num955].velocity.X -= 1f;
        Main.gore[num955].velocity.Y += 1f;
        num955 = Gore.NewGore(proj.GetSource_FromThis(), position, default, Main.rand.Next(61, 64));
        gore2 = Main.gore[num955];
        gore2.velocity *= 0.4f;
        Main.gore[num955].velocity.X += 1f;
        Main.gore[num955].velocity.Y -= 1f;
        num955 = Gore.NewGore(proj.GetSource_FromThis(), position, default, Main.rand.Next(61, 64));
        gore2 = Main.gore[num955];
        gore2.velocity *= 0.4f;
        Main.gore[num955].velocity.X -= 1f;
        Main.gore[num955].velocity.Y -= 1f;
        if (proj.type == 102) {
            position.X += proj.width / 2;
            position.Y += proj.height / 2;
            proj.width = 128;
            proj.height = 128;
            position.X -= proj.width / 2;
            position.Y -= proj.height / 2;
            proj.damage = 40;
            proj.Damage();
            proj.width = 22;
            proj.height = 22;
        }

        if (proj.type == 75) {
            proj.Resize(128, 128);
            proj.damage = 60;
            proj.knockBack = 8f;
            proj.Damage();
            proj.Resize(22, 22);
        }
    }

    public static void dynamiteExplosionCode(Projectile proj) {
        SoundEngine.PlaySound(SoundID.Item14, proj.position);
        if (Constants.dynamites.Contains(proj.type)) {
            proj.position.X += proj.width / 2;
            proj.position.Y += proj.height / 2;
            proj.width = 200;
            proj.height = 200;
            proj.position.X -= proj.width / 2;
            proj.position.Y -= proj.height / 2;
        }

        for (int num956 = 0; num956 < 50; num956++) {
            int num957 = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y), proj.width, proj.height, DustID.Smoke, 0f,
                0f, 100,
                default, 2f);
            Dust dust2 = Main.dust[num957];
            dust2.velocity *= 1.4f;
        }

        for (int num958 = 0; num958 < 80; num958++) {
            int num959 = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y), proj.width, proj.height, DustID.Torch, 0f, 0f,
                100,
                default, 3f);
            Main.dust[num959].noGravity = true;
            Dust dust2 = Main.dust[num959];
            dust2.velocity *= 5f;
            num959 = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y), proj.width, proj.height, DustID.Torch, 0f, 0f,
                100, default,
                2f);
            dust2 = Main.dust[num959];
            dust2.velocity *= 3f;
        }

        for (int num960 = 0; num960 < 2; num960++) {
            int num961 =
                Gore.NewGore(proj.GetSource_FromThis(),
                    new Vector2(proj.position.X + proj.width / 2 - 24f, proj.position.Y + proj.height / 2 - 24f),
                    default, Main.rand.Next(61, 64));
            Main.gore[num961].scale = 1.5f;
            Main.gore[num961].velocity.X += 1.5f;
            Main.gore[num961].velocity.Y += 1.5f;
            num961 = Gore.NewGore(proj.GetSource_FromThis(),
                new Vector2(proj.position.X + proj.width / 2 - 24f, proj.position.Y + proj.height / 2 - 24f), default,
                Main.rand.Next(61, 64));
            Main.gore[num961].scale = 1.5f;
            Main.gore[num961].velocity.X -= 1.5f;
            Main.gore[num961].velocity.Y += 1.5f;
            num961 = Gore.NewGore(proj.GetSource_FromThis(),
                new Vector2(proj.position.X + proj.width / 2 - 24f, proj.position.Y + proj.height / 2 - 24f), default,
                Main.rand.Next(61, 64));
            Main.gore[num961].scale = 1.5f;
            Main.gore[num961].velocity.X += 1.5f;
            Main.gore[num961].velocity.Y -= 1.5f;
            num961 = Gore.NewGore(proj.GetSource_FromThis(),
                new Vector2(proj.position.X + proj.width / 2 - 24f, proj.position.Y + proj.height / 2 - 24f), default,
                Main.rand.Next(61, 64));
            Main.gore[num961].scale = 1.5f;
            Main.gore[num961].velocity.X -= 1.5f;
            Main.gore[num961].velocity.Y -= 1.5f;
        }

        proj.position.X += proj.width / 2;
        proj.position.Y += proj.height / 2;
        proj.width = 10;
        proj.height = 10;
        proj.position.X -= proj.width / 2;
        proj.position.Y -= proj.height / 2;
    }
}