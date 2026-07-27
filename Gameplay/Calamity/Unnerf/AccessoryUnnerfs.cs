using CalamityMod.Projectiles;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class YoyoGloveUnnerf : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.yoyoGlove;
    }

    public override void OnModLoad() {
        var preAI = typeof(CalamityGlobalProjectile).GetMethod("PreAI");
        if (preAI == null) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find CalamityGlobalProjectile.PreAI!");
            return;
        }

        MonoModHooks.Modify(preAI, fullDamage);
    }

    // ldfld  Player::yoyoGlove
    // brfalse -> skip the halving
    private void fullDamage(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("yoyoGlove"))) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find the Yo-yo Glove check in CalamityGlobalProjectile.PreAI!");
            return;
        }

        ilCursor.EmitPop();
        ilCursor.EmitLdcI4(0);
    }
}
