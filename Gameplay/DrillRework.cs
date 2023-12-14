using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DrillRework : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.drillRework;
    }

    public override void Load() {
        IL_Player.ItemCheck_OwnerOnlyCode += drillPatch;
        //IL_Player.CanAutoReuseItem += autoReusePatch;
    }

    public override void Unload() {
        IL_Player.ItemCheck_OwnerOnlyCode -= drillPatch;
        //IL_Player.CanAutoReuseItem -= autoReusePatch;
    }

    // make drills autoReuse
    public void autoReusePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // IL_005a: ldfld        bool Terraria.Item::channel
        // IL_005f: brfalse.s    IL_0069
        if (ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchLdfld<Player>("channel"))) {
            ilCursor.Next.OpCode = OpCodes.Br_S;
            // pop it
            ilCursor.EmitPop();
        }
        else {
            VanillaQoL.instance.Logger.Warn("Couldn't match channel in Player.CanAutoReuseItem! (Player.channel)");
        }
    }

    public void drillPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // [31253 7 - 31253 25]
        // IL_01a7: ldarg.0      // this
        // IL_01a8: ldfld        bool Terraria.Player::channel
        // IL_01ad: brtrue.s     IL_01bd
        ILLabel l = null!;
        if (ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg0(),
                i => i.MatchLdfld<Player>("channel"),
                i => i.MatchBrtrue(out l))) {
            // we don't need these instructions
            var label = ilCursor.DefineLabel();
            ilCursor.GotoLabel(l);
            ilCursor.EmitLdarg0();
            ilCursor.EmitLdarg2();
            ilCursor.Emit<DrillRework>(OpCodes.Call, "drillToolTime");
            ilCursor.EmitBr(label);
            // jump ahead
            ilCursor.GotoNext(MoveType.After, i => i.MatchStfld<Player>("toolTime"));
            ilCursor.MarkLabel(label);

        }
        else {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't match channeling in Player.ItemCheck_OwnerOnlyCode! (Player.channel)");
        }
    }

    public static void drillToolTime(Player p, Item item) {
        --p.toolTime;
        if (p.toolTime < 0)
            p.toolTime = (int)(CombinedHooks.TotalUseTime(item.useTime, p, item) * p.pickSpeed);
    }


    // example:
    //case 263:
    // this.useStyle = 5;
    // this.useAnimation = 25;
    // this.useTime = 13;
    // this.shootSpeed = 32f;
    // this.knockBack = 0.5f;
    // this.width = 20;
    // this.height = 12;
    // this.damage = 10;
    // this.pick = 110;
    // this.UseSound = new SoundStyle?(SoundID.Item23);
    // this.shoot = 59;
    // this.rare = 4;
    // this.value = 54000;
    // this.noMelee = true;
    // this.noUseGraphic = true;
    // this.melee = true;
    // this.channel = true;
    // break;
    public override void SetDefaults(Item item) {
        // so, tml are not the brightest, so they placed this SetDefaults hook *after* vanilla gives the 60% boost to drills.
        // how lovely, so the boost only applies to vanilla.
        // fear not, we can just use maths to undo the change.
        var isDrill = ItemID.Sets.IsDrill[item.type] || ItemID.Sets.IsChainsaw[item.type] ||
                      item.type == ItemID.ChlorophyteJackhammer;
        if (isDrill) {
            var u = item.useTime;
            // apply 25% bonus
            item.useTime = (int)(item.useTime / 0.6 * 0.75);
            item.useAnimation = (int)(item.useAnimation / 0.6 * 0.75);

            // fuck off with that MeleeNoSpeed
            item.DamageType = DamageClass.Melee;
            // we don't want drills to get melee speed boosts for mining though, that would break it
            item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
    }

    public override void SetStaticDefaults() {
        // only vanilla
        for (int i = 0; i < ItemID.Count; i++) {
            var isDrill = ItemID.Sets.IsDrill[i] || ItemID.Sets.IsChainsaw[i] || i == ItemID.ChlorophyteJackhammer;
            if (isDrill) {
                PrefixLegacy.ItemSets.SwordsHammersAxesPicks[i] = true;
            }
        }
    }
}