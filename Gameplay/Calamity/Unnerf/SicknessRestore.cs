using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

[JITWhenModsEnabled("CalamityMod")]
public class SicknessRestore : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.sicknessContactDamage;
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) {
        var calamity = npc.GetGlobalNPC<CalamityGlobalNPC>();

        if (npc.poisoned) {
            modifiers.SourceDamage *= reduction(calamity);
        }

        if (npc.venom) {
            modifiers.SourceDamage *= reduction(calamity);
        }

        if (calamity.astralInfection) {
            modifiers.SourceDamage *= reduction(calamity);
        }

        if (calamity.plague) {
            modifiers.SourceDamage *= reduction(calamity);
        }
    }

    private static float reduction(CalamityGlobalNPC calamity) {
        var cut = calamity.irradiated ? 0.075f : 0.05f;

        if (calamity.VulnerableToSickness is { } vulnerable) {
            cut = vulnerable ? cut * 2f : cut / 2f;
        }

        return 1f - cut;
    }
}
