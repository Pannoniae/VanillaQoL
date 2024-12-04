using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class DamageReductionShield : ModSystem, ILocalizedModType {

    public string LocalizationCategory => "Shield";

    public static LocalizedText damageReduction = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.drShield;
    }

    public override void Load() {
        IL_Main.DrawDefenseCounter += defenseCounterPatch;
    }

    public override void SetStaticDefaults() {
        damageReduction = this.GetLocalization(nameof(damageReduction));
    }

    // IL_0193: stloc.s      str
    public void defenseCounterPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(4))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match stloc.s for the defense string in Main.DrawDefenseCounter!");
        }
        ilCursor.EmitCall<DamageReductionShield>("DRString");
    }

    public static string DRString(string original) {
        int dr = (int)(Main.player[Main.myPlayer].endurance * 100);
        return original + "\n" + damageReduction.Format(dr);
    }

    public override void Unload() {
        IL_Main.DrawDefenseCounter -= defenseCounterPatch;
    }
}