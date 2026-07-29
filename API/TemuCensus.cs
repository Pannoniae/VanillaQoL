using System;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.Gameplay;

namespace VanillaQoL.API;

// the bogus mod name keeps tML from ever autoloading this or counting it as a second Mod class -
// it's not IsLoadable (good!)
[ExtendsFromMod("VanillaQoLFacadesAreNotRealMods")]
public class TemuCensus : Mod {
    public override string Name => "Census";

    // the actual version of the mod!
    public override Version Version => new(0, 5, 2, 7);

    public override object Call(params object[] args) {
        try {
            if (args[0] is not string message) {
                return "Failure";
            }

            if (message == "TownNPCCondition") {
                var type = Convert.ToInt32(args[1]);
                if (args.Length >= 3 && args[2] is string legacy) {
                    NPCCensus.registerCondition(type,
                        ModContent.GetModNPC(type).GetLocalization("Census.SpawnCondition", () => legacy));
                    throw new Exception(
                        "Call Error: The 2nd parameter of TownNPCCondition is now LocalizedText and is optional. Also, localization is now automatic, keys will appear in your hjson files. This TownNPCCondition Mod.Call is only needed if using LocalizedText.WithFormatArgs");
                }

                if (args[2] is LocalizedText conditions) {
                    NPCCensus.registerCondition(type, conditions);
                }

                return "Success";
            }

            VanillaQoL.instance.Logger.Error($"Census Call Error: Unknown Message: {message}");
        }
        catch (Exception e) {
            VanillaQoL.instance.Logger.Error($"Census Call Error: {e.StackTrace} {e}");
        }

        return "Failure";
    }
}
