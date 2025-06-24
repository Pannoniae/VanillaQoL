using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class TownNPC : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.townPetsInvincible || QoLConfig.Instance.boundNPCProtection;
    }

    public override void SetDefaults(NPC npc) {
        if (QoLConfig.Instance.townPetsInvincible) {
            if (Constants.pets.Contains(npc.type) || Constants.slimes.Contains(npc.type)) {
                npc.dontTakeDamageFromHostiles = true;
            }
        }
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker) {
        if (QoLConfig.Instance.boundNPCProtection && npc.friendly && npc.aiStyle == NPCAIStyleID.FaceClosestPlayer) {
            return false;
        }

        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) {
        if (QoLConfig.Instance.boundNPCProtection && npc.friendly && projectile.hostile &&
            npc.aiStyle == NPCAIStyleID.FaceClosestPlayer) {
            return false;
        }

        return base.CanBeHitByProjectile(npc, projectile);
    }
}

public class TownNPCSystem : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableNPCHappiness;
    }

    public override void Load() {
    }

    private static void disableHappinessButton(ILContext il) {
        var ilCursor = new ILCursor(il);

        if (!ilCursor.TryGotoNext(MoveType.After, x => x.MatchLdstr("UI.NPCCheckHappiness"))) {
            return;
        }

        ilCursor.EmitDelegate((string text) => "");
    }

    public override void Unload() {
    }
}

public class TownNPCPlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disableNPCHappiness;
    }

    public override void PreUpdateBuffs() {
        //CHANGES SHOP PRICES WHEN HAPPINESS IS DISABLED (Thx Jabon!)
        Player.currentShoppingSettings.PriceAdjustment = QoLConfig.Instance.disableNPCHappinessConstant;
        Player.currentShoppingSettings.HappinessReport = "";
    }
}