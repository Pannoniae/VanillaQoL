using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Buffs;

public class NurseHeal : ModBuff {
    // we reuse because we are lazy
    public override string Texture => "Terraria/Images/Buff_2";

    public override void SetStaticDefaults() {
        Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world}
    }

    public override void Update(Player player, ref int buffIndex) {
        player.GetModPlayer<NurseHealPlayer>().nurseHeal = true;
    }
}

public class NurseHealPlayer : ModPlayer
{
    // Flag checking when life regen debuff should be activated
    public bool nurseHeal;

    // how long to apply buff for (ticks). Used to calculate regen rate
    public int time;

    // how much HP to heal overall. Used to calculate regen rate
    public int totalToHeal;

    public override void ResetEffects() {
        nurseHeal = false;
    }

    public override void UpdateLifeRegen() {
        if (nurseHeal) {
            // we want to heal a total amount of totalToHeal. so, the regen rate per second is
            // totalToHeal / time
            var regen = totalToHeal / (time / 60);
            var regen2 = regen * 2;
            // lifeRegen is measured in 1/2 life per second.
            Player.lifeRegen += regen2;
        }
    }
}