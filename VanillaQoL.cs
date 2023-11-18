using Terraria.ModLoader;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;


    public override void Load() {
        instance = this;
    }
}