using System.Collections.Generic;
using MonoMod.Cil;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class AllHairsOnStart : ModSystem {
    
    public const int maxHair = 165;
    
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.allHairsOnStart;
    }

    public override void Load() {
        IL_HairstyleUnlocksHelper.RebuildList += unlockAllHairsPatch;
    }

    public override void Unload() {
        IL_HairstyleUnlocksHelper.RebuildList -= unlockAllHairsPatch;
    }

    private static void unlockAllHairsPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitLdarg0();
        ilCursor.EmitLdfld<HairstyleUnlocksHelper>("AvailableHairstyles");
        ilCursor.EmitCall<AllHairsOnStart>("unlockAllHairs");
        ilCursor.EmitRet();


    }

    private static void unlockAllHairs(List<int> availableHairstyles) {
        availableHairstyles.Clear();
        for (int i = 0; i < HairLoader.Count; i++) {
            availableHairstyles.Add(i);
        }
    }
}