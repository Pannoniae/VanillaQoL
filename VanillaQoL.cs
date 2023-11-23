using Terraria.ModLoader;
using VanillaQoL.Fixes;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;

    public bool hasThorium;
    public bool hasCalamity;
    public bool hasCensus;

    public override void Load() {
        instance = this;
        hasThorium = ModLoader.HasMod("ThoriumMod");
        hasCalamity = ModLoader.HasMod("CalamityMod");
        hasCensus = ModLoader.HasMod("Census");
        ILEdits.load();

    }

    public override void Unload() {
        ILEdits.unload();
        Utils.completelyWipeClass(typeof(ILEdits));
        instance = null!;
    }
}