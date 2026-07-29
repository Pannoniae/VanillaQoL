using CalamityMod.Items;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class EquipStatUnnerfs : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.vanillaEquipStats;
    }

    public override void OnModLoad() {
        var updateEquip = typeof(CalamityGlobalItem).GetMethod("UpdateEquip");
        if (updateEquip == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.UpdateEquip!");
            return;
        }

        var updateArmorSet = typeof(CalamityGlobalItem).GetMethod("UpdateArmorSet");
        if (updateArmorSet == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find CalamityGlobalItem.UpdateArmorSet!");
            return;
        }

        MonoModHooks.Modify(updateEquip, dontTouchTheStats);
        MonoModHooks.Modify(updateArmorSet, dontTouchTheSets);
    }

    private void dontTouchTheStats(ILContext il) {
        new ILCursor(il).EmitRet();
    }

    // ldstr    "WizardHat"
    // call     string::op_Equality
    // brfalse
    private void dontTouchTheSets(ILContext il) {
        var ilCursor = new ILCursor(il);

        foreach (var set in new[] { "WizardHat", "MagicHat" }) {
            if (!ilCursor.TryGotoNext(MoveType.After,
                    i => i.MatchLdstr(set), i => i.MatchCallOrCallvirt(out _))) {
                VanillaQoL.instance.Logger.Warn(
                    $"Couldn't find the {set} set bonus in CalamityGlobalItem.UpdateArmorSet!");
                continue;
            }

            ilCursor.EmitPop();
            ilCursor.EmitLdcI4(0);
        }
    }
}
