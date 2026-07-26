using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Items.VanillaArmorChanges;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class ArmourUnnerfs : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.meteorArmour;
    }

    public override void PostSetupContent() {
        var field = typeof(VanillaArmorChangeManager).GetField("ArmorChanges",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is not List<VanillaArmorChange> changes) {
            VanillaQoL.instance.Logger.Warn(
                "Couldn't find VanillaArmorChangeManager.ArmorChanges!, can't apply the Meteor Armour unnerf :(");
            return;
        }

        if (CalamityUnnerfConfig.Instance.meteorArmour) {
            remove<MeteorArmorSetChange>(changes, "Meteor");
        }
    }

    private static void remove<T>(List<VanillaArmorChange> changes, string name) where T : VanillaArmorChange {
        if (changes.RemoveAll(change => change is T) == 0) {
            VanillaQoL.instance.Logger.Warn($"No {name} armour change to remove, someone else removed it?");
        }
    }
}
