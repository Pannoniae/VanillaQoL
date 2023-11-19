using Terraria.ModLoader;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;

    public bool hasThorium;

    public override void Load() {
        instance = this;
        hasThorium = ModLoader.HasMod("ThoriumMod");
    }
}