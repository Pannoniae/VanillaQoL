using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.API;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

public class DrillRework : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.drillRework;
    }

    public override void Load() {
        IL_Player.ItemCheck_OwnerOnlyCode += drillPatch;
        IL_Player.DoesPickTargetTransformOnKill += drillGrassPatch;
        IL_Item.GetPrefixCategory += moddedDrillPrefixesPatch;
        //IL_Player.CanAutoReuseItem += autoReusePatch;
    }

    public override void Unload() {
        IL_Player.ItemCheck_OwnerOnlyCode -= drillPatch;
        IL_Player.DoesPickTargetTransformOnKill -= drillGrassPatch;
        //IL_Player.CanAutoReuseItem -= autoReusePatch;
    }

    // if the player has a drill, just kill every tile immediately
    public void drillGrassPatch(ILContext il) {
        // Item sItem1 = this.inventory[this.selectedItem];
        var ilCursor = new ILCursor(il);
        ILLabel label = null!;
        // match blt
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchBlt(out label!))) {
            // more conditions, yey!
            ilCursor.EmitLdarg0();
            ilCursor.Emit<DrillRework>(OpCodes.Call, "isDrill");
            // if it's a drill, just return true
            ilCursor.EmitBrtrue(label);
        }
        else {
            VanillaQoL.instance.Logger.Warn("Couldn't match jump in DoesPickTargetTransformOnKill!");
        }
    }

    public static bool isDrill(Player player) {
        var item = player.inventory[player.selectedItem];
        return Constants.isDrill(item.type);
    }

    private void moddedDrillPrefixesPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // we match each of them, or'ing it with our method
        //IL_000f: call         bool Terraria.ModLoader.ItemLoader::MeleePrefix(class Terraria.Item)
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.ItemLoader", "MeleePrefix"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match ItemLoader.MeleePrefix in Item.GetPrefixCategory!");
        }

        ilCursor.EmitLdarg0();
        ilCursor.Emit<DrillRework>(OpCodes.Call, "meleePrefix");
        ilCursor.EmitOr();

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.ItemLoader", "RangedPrefix"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match ItemLoader.RangedPrefix in Item.GetPrefixCategory!");
        }

        ilCursor.EmitLdarg0();
        ilCursor.Emit<DrillRework>(OpCodes.Call, "rangedPrefix");
        ilCursor.EmitOr();

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.ItemLoader", "MagicPrefix"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match ItemLoader.MagicPrefix in Item.GetPrefixCategory!");
        }

        ilCursor.EmitLdarg0();
        ilCursor.Emit<DrillRework>(OpCodes.Call, "magicPrefix");
        ilCursor.EmitOr();

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.ItemLoader", "WeaponPrefix"))) {
            VanillaQoL.instance.Logger.Warn("Couldn't match ItemLoader.WeaponPrefix in Item.GetPrefixCategory!");
        }

        ilCursor.EmitLdarg0();
        ilCursor.Emit<DrillRework>(OpCodes.Call, "weaponPrefix");
        ilCursor.EmitOr();
    }

    // METHODS
    public static bool meleePrefix(Item item) {
        if (Constants.isDrill(item.type)) {
            return true;
        }

        return false;
    }

    public static bool rangedPrefix(Item item) {
        return false;
    }

    public static bool magicPrefix(Item item) {
        return false;
    }

    public static bool weaponPrefix(Item item) {
        return false;
    }

    // make drills autoReuse
    public void autoReusePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // IL_005a: ldfld        bool Terraria.Item::channel
        // IL_005f: brfalse.s    IL_0069
        if (ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchLdfld<Player>("channel"))) {
            ilCursor.Next!.OpCode = OpCodes.Br_S;
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
                i => i.MatchBrtrue(out l!))) {
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
        if (Constants.isDrill(item.type)) {
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
            if (Constants.isDrill(i)) {
                PrefixLegacy.ItemSets.SwordsHammersAxesPicks[i] = true;
            }
        }
    }
}