using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

[JITWhenModsEnabled("CalamityMod")]
public class CalamityRarities : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.calamityInWorldRarity && VanillaQoL.isCalamityLoaded();
    }

    public override void SetDefaults(Item item) {
        if (item.ModItem == null) {
            return;
        }

        if (item.type == ModContent.ItemType<Sylvestaff>()) {
            item.rare = ModContent.RarityType<RFabstaff>();
        }
        else if (item.type == ModContent.ItemType<StaffofBlushie>()) {
            item.rare = ModContent.RarityType<RStaffofBlushie>();
        }
        else if (item.type == ModContent.ItemType<TheDanceofLight>()) {
            item.rare = ModContent.RarityType<RTheDanceofLight>();
        }
        else if (item.type == ModContent.ItemType<NanoblackReaper>()) {
            item.rare = ModContent.RarityType<RNanoblackReaper>();
        }
        else if (item.type == ModContent.ItemType<ShatteredCommunity>()) {
            item.rare = ModContent.RarityType<RShatteredCommunity>();
        }
        else if (item.type == ModContent.ItemType<ProfanedSoulCrystal>()) {
            item.rare = ModContent.RarityType<RProfanedSoulCrystal>();
        }
        else if (item.type == ModContent.ItemType<TemporalUmbrella>()) {
            item.rare = ModContent.RarityType<RTemporalUmbrella>();
        }
        else if (item.type == ModContent.ItemType<Endogenesis>()) {
            item.rare = ModContent.RarityType<REndogenesis>();
        }
        else if (item.type == ModContent.ItemType<DraconicDestruction>()) {
            item.rare = ModContent.RarityType<RDraconicDestruction>();
        }
        else if (item.type == ModContent.ItemType<ScarletDevil>()) {
            item.rare = ModContent.RarityType<RScarletDevil>();
        }
        else if (item.type == ModContent.ItemType<RedSun>()) {
            item.rare = ModContent.RarityType<RRedSun>();
        }
        else if (item.type == ModContent.ItemType<CrystylCrusher>()) {
            item.rare = ModContent.RarityType<RCrystylCrusher>();
        }
        else if (item.type == ModContent.ItemType<SomaPrime>()) {
            item.rare = ModContent.RarityType<RSomaPrime>();
        }
        else if (item.type == ModContent.ItemType<Svantechnical>()) {
            item.rare = ModContent.RarityType<RSvantechnical>();
        }
        else if (item.type == ModContent.ItemType<Contagion>()) {
            item.rare = ModContent.RarityType<RContagion>();
        }
        else if (item.type == ModContent.ItemType<TriactisTruePaladinianMageHammerofMightMelee>()) {
            item.rare = ModContent.RarityType<RTriactisTruePaladinianMageHammerofMightMelee>();
        }
        else if (item.type == ModContent.ItemType<IllustriousKnives>()) {
            item.rare = ModContent.RarityType<RIllustriousKnives>();
        }
        else if (item.type == ModContent.ItemType<DemonshadeHelm>() ||
                 item.type == ModContent.ItemType<DemonshadeGreaves>() ||
                 item.type == ModContent.ItemType<DemonshadeBreastplate>()) {
            item.rare = ModContent.RarityType<RDemonshade>();
        }
        else if (item.type == ModContent.ItemType<AngelicAlliance>()) {
            item.rare = ModContent.RarityType<RAngelicAlliance>();
        }
        else if (item.type == ModContent.ItemType<Eternity>()) {
            item.rare = ModContent.RarityType<REternity>();
        }
        else if (item.type == ModContent.ItemType<FlamsteedRing>()) {
            item.rare = ModContent.RarityType<RFlamsteedRing>();
        }
        else if (item.type == ModContent.ItemType<Earth>()) {
            item.rare = ModContent.RarityType<REarth>();
        }
    }

    public static Color calamityCycle(params Color[] colors) {
        int index = (int)(Main.GlobalTimeWrappedHourly / 2 % colors.Length);
        float amount = Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f;
        return Color.Lerp(colors[index], colors[(index + 1) % colors.Length], amount);
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class Rarity : ModRarity {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.calamityInWorldRarity && VanillaQoL.isCalamityLoaded();
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class RFabstaff : Rarity {
    public override Color RarityColor => new Color(249, 197, 255);
}

[JITWhenModsEnabled("CalamityMod")]
public class RStaffofBlushie : Rarity {
    public override Color RarityColor => new Color(0, 0, 255);
}

[JITWhenModsEnabled("CalamityMod")]
public class RTheDanceofLight : Rarity {
    public override Color RarityColor => CalamityMod.Items.Weapons.Magic.TheDanceofLight.GetSyncedLightColor();
}

[JITWhenModsEnabled("CalamityMod")]
public class RNanoblackReaper : Rarity {
    public override Color RarityColor =>
        new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RShatteredCommunity : Rarity {
    private static readonly Color rarityColorOne = new Color(128, 62, 128);
    private static readonly Color rarityColorTwo = new Color(245, 105, 245);
    public override Color RarityColor => CalamityUtils.ColorSwap(rarityColorOne, rarityColorTwo, 3f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RProfanedSoulCrystal : Rarity {
    public override Color RarityColor =>
        CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25),
            6f); //alternates between emerald green and amber (BanditHueh)
}

[JITWhenModsEnabled("CalamityMod")]
public class RTemporalUmbrella : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(210, 0, 255), new Color(255, 248, 24), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class REndogenesis : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(131, 239, 255), new Color(36, 55, 230), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RDraconicDestruction : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(255, 69, 0), new Color(139, 0, 0), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RScarletDevil : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(191, 45, 71), new Color(185, 187, 253), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RRedSun : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(204, 86, 80), new Color(237, 69, 141), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RCrystylCrusher : Rarity {
    public override Color RarityColor => new Color(129, 29, 149);
}

[JITWhenModsEnabled("CalamityMod")]
public class RSomaPrime : Rarity {
    public override Color RarityColor =>
        CalamityUtils.ColorSwap(new Color(255, 255, 255), new Color(0xD1, 0xCC, 0x6F), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RSvantechnical : Rarity {
    public override Color RarityColor => new Color(220, 20, 60);
}

[JITWhenModsEnabled("CalamityMod")]
public class RContagion : Rarity {
    public override Color RarityColor => new Color(207, 17, 117);
}

[JITWhenModsEnabled("CalamityMod")]
public class RTriactisTruePaladinianMageHammerofMightMelee : Rarity {
    public override Color RarityColor => new Color(227, 226, 180);
}

[JITWhenModsEnabled("CalamityMod")]
public class RIllustriousKnives : Rarity {
    public override Color RarityColor =>
        CalamityUtils.ColorSwap(new Color(154, 255, 151), new Color(228, 151, 255), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RDemonshade : Rarity {
    public override Color RarityColor => CalamityUtils.ColorSwap(new Color(255, 132, 22), new Color(221, 85, 7), 4f);
}

[JITWhenModsEnabled("CalamityMod")]
public class RAngelicAlliance : Rarity {
    public override Color RarityColor => CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f,
        new Color(255, 196, 55), new Color(255, 231, 107), new Color(255, 254, 243));
}

[JITWhenModsEnabled("CalamityMod")]
public class REternity : Rarity {
    public override Color RarityColor => CalamityRarities.calamityCycle(
        new Color(188, 192, 193), // white
        new Color(157, 100, 183), // purple
        new Color(249, 166, 77), // honey-ish orange
        new Color(255, 105, 234), // pink
        new Color(67, 204, 219), // sky blue
        new Color(249, 245, 99), // bright yellow
        new Color(236, 168, 247)); // purplish pink
}

[JITWhenModsEnabled("CalamityMod")]
public class RFlamsteedRing : Rarity {
    public override Color RarityColor {
        get {
            return (Main.GlobalTimeWrappedHourly % 1f) switch {
                < 0.6f => new Color(89, 229, 255),
                < 0.8f => Color.Lerp(new Color(89, 229, 255), Color.White,
                    (Main.GlobalTimeWrappedHourly % 1f - 0.6f) / 0.2f),
                _ => Color.Lerp(Color.White, new Color(89, 229, 255), (Main.GlobalTimeWrappedHourly % 1f - 0.8f) / 0.2f)
            };
        }
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class REarth : Rarity {
    public override Color RarityColor => CalamityRarities.calamityCycle(
        new Color(255, 99, 146),
        new Color(255, 228, 94),
        new Color(127, 200, 248));
}