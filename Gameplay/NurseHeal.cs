using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using VanillaQoL.Config;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;


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

public class NurseHealPlayer : ModPlayer {
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

public class NurseHealing {
    public static void load() {
        if (QoLConfig.Instance.overTimeNurseHealing) {
            IL_Main.GUIChatDrawInner += nurseHealingPatch;
        }
    }

    public static void unload() {
        IL_Main.GUIChatDrawInner -= nurseHealingPatch;
    }

    public static void nurseAddHealing(int health) {
        //VanillaQoL.instance.Logger.Info("Healed with Nurse!");
        // apply buff until specified health is reached
        var player = Main.LocalPlayer;
        // in ticks
        var time = QoLConfig.Instance.nurseHealingTime * 60;
        player.GetModPlayer<NurseHealPlayer>().totalToHeal = health;
        player.GetModPlayer<NurseHealPlayer>().time = time;
        // sync with network
        player.AddBuff(ModContent.BuffType<NurseHeal>(), time, false);
    }

    // [32261 19 - 32261 64]
    // IL_21d5: ldsfld       class Terraria.Player[] Terraria.Main::player
    // IL_21da: ldsfld       int32 Terraria.Main::myPlayer
    // IL_21df: ldelem.ref
    // IL_21e0: dup
    // IL_21e1: ldfld        int32 Terraria.Player::statLife
    // IL_21e6: ldloc.s      health
    // IL_21e8: add
    // IL_21e9: stfld        int32 Terraria.Player::statLife
    //
    // we remove this entire fucking block and just replace it lol
    public static void nurseHealingPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        int health = 0;
        if (ilCursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<Main>("player"),
                i => i.MatchLdsfld<Main>("myPlayer"),
                i => i.MatchLdelemRef(),
                i => i.MatchDup(),
                i => i.MatchLdfld<Player>("statLife"),
                i => i.MatchLdloc(out health), // health is index 14
                i => i.MatchAdd(),
                i => i.MatchStfld<Player>("statLife"))) {
            // great, so new we have all those cute instructions, we will get rid of all of them
            ilCursor.Emit(OpCodes.Ldloc_S, (byte)health);
            ilCursor.Emit<NurseHealing>(OpCodes.Call, "nurseAddHealing");
            ilCursor.RemoveRange(8);

            ILEdits.updateOffsets(ilCursor);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Failed to locate nurse healing at GUIChatDrawInner (Player.statLife)");
        }
    }
}