using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ID;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class JungleRose : GlobalTile {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.naturesGift;
    }

    public override void Load() {
        IL_TileDrawing.DrawTiles_EmitParticles += drawParticlesPatch;
    }

    public override void Unload() {
        IL_TileDrawing.DrawTiles_EmitParticles -= drawParticlesPatch;
    }

    private void drawParticlesPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        // xy is swapped
        ilCursor.EmitLdarg2();
        ilCursor.EmitLdarg1();
        ilCursor.EmitLdarg(4);
        ilCursor.EmitLdarg(5);
        ilCursor.EmitLdarg(6);
        ilCursor.EmitLdarg(7);
        ilCursor.Emit<JungleRose>(OpCodes.Call, "DrawEffectsButActuallyCalled");
    }

    public static void DrawEffectsButActuallyCalled(TileDrawing drawing, int i, int j, int type, short tileFrameX,
        short tileFrameY, Color tileLight) {
        if (type == 61) {
            // idk the exact tileframe
            // natures gift
            VanillaQoL.instance.Logger.Warn("Test early!");
            if (tileFrameX is >= 160 and <= 164 &&
                Main.rand.NextBool(60)) {
                VanillaQoL.instance.Logger.Warn("Test!");
                int num35 = Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<NaturesGiftDust>(), 0f, 0f, 250,
                    default, 0.4f);
                Main.dust[num35].fadeIn = 0.7f;
            }

            // jungle rose
            if (tileFrameX is >= 106 and <= 128 && Main.rand.NextBool(60)) {
                int num35 = Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<JungleRoseDust>(), 0f, 0f, 250,
                    default, 0.4f);
                Main.dust[num35].fadeIn = 0.7f;
            }
        }
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b) {
        var tile = Main.tile[i, j];
        if (type == 61) {
            // idk the exact tileframe
            if (tile.TileFrameX is > 160 and < 166) {
                // jungle spore but blue

                // strong
                float num8 = 1f + (270 - Main.mouseTextColor) / 400f;
                // weak
                float num9 = 0.8f - (270 - Main.mouseTextColor) / 400f;
                r = 0.22f * num9;
                g = 0.51f * num9;
                b = 0.82f * num8;
            }

            if (tile.TileFrameX is > 106 and <= 128) {
                float num8 = 1f + (270 - Main.mouseTextColor) / 400f;
                float num9 = 0.8f - (270 - Main.mouseTextColor) / 400f;
                r = 0.82f * num8;
                g = 0.51f * num9;
                b = 0.12f * num9;
            }
        }
    }
}

public class NaturesGiftDust : ModDust {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.naturesGift;
    }

    public override bool Update(Dust dust) {
        dust.velocity.X += Main.rand.Next(-10, 11) * (3f / 1000f);
        dust.velocity.Y += Main.rand.Next(-10, 11) * (3f / 1000f);
        if (dust.velocity.X > 0.35)
            dust.velocity.X = 0.35f;
        if (dust.velocity.X < -0.35)
            dust.velocity.X = -0.35f;
        if (dust.velocity.Y > 0.35)
            dust.velocity.Y = 0.35f;
        if (dust.velocity.Y < -0.35)
            dust.velocity.Y = -0.35f;
        dust.scale += 0.0085f;
        float g = dust.scale * 0.7f;
        if (g > 1.0)
            g = 1f;
        Lighting.AddLight((int) (dust.position.X / 16.0), (int) (dust.position.Y / 16.0), g * 0.4f, g * 0.7f, g);

        // heh nogravity hack
        dust.velocity.Y -= 0.1f;
        return true;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        float num1 = (byte.MaxValue - dust.alpha) / (float) byte.MaxValue;
        num1 = (float) ((num1 + 3.0) / 4.0);
        int r1 = (int) ((double) lightColor.R * num1);
        int g1 = (int) ((double) lightColor.G * num1);
        int b1 = (int) ((double) lightColor.B * num1);
        int alpha3 = lightColor.A - dust.alpha;
        if (alpha3 < 0)
            alpha3 = 0;
        if (alpha3 > byte.MaxValue)
            alpha3 = byte.MaxValue;
        return new Color(r1, g1, b1, alpha3);
    }
}
public class JungleRoseDust : ModDust {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.naturesGift;
    }

    public override bool Update(Dust dust) {
        dust.velocity.X += Main.rand.Next(-10, 11) * (3f / 1000f);
        dust.velocity.Y += Main.rand.Next(-10, 11) * (3f / 1000f);
        if (dust.velocity.X > 0.35)
            dust.velocity.X = 0.35f;
        if (dust.velocity.X < -0.35)
            dust.velocity.X = -0.35f;
        if (dust.velocity.Y > 0.35)
            dust.velocity.Y = 0.35f;
        if (dust.velocity.Y < -0.35)
            dust.velocity.Y = -0.35f;
        dust.scale += 0.0085f;
        float g = dust.scale * 0.7f;
        if (g > 1.0)
            g = 1f;
        Lighting.AddLight((int) (dust.position.X / 16.0), (int) (dust.position.Y / 16.0), g, g * 0.7f, g * 0.4f);
        // heh nogravity hack
        dust.velocity.Y -= 0.1f;
        return true;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        float num1 = (byte.MaxValue - dust.alpha) / (float) byte.MaxValue;
        num1 = (float) ((num1 + 3.0) / 4.0);
        int r1 = (int) ((double) lightColor.R * num1);
        int g1 = (int) ((double) lightColor.G * num1);
        int b1 = (int) ((double) lightColor.B * num1);
        int alpha3 = lightColor.A - dust.alpha;
        if (alpha3 < 0)
            alpha3 = 0;
        if (alpha3 > byte.MaxValue)
            alpha3 = byte.MaxValue;
        return new Color(r1, g1, b1, alpha3);
    }
}