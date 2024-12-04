using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using ZenithQoL.IL;

namespace ZenithQoL.Gameplay;

public class BuffsPersistOnDeath : ModSystem {
    private static Dictionary<int, bool> _persistentBuffs = null!;

    public override bool IsLoadingEnabled(Mod mod) => QoLConfig.Instance.persistentBuffs;

    public override void PostSetupContent() {
        _persistentBuffs = new();

        // Make all saveable buffs persistent
        for (int buff = 0; buff < BuffLoader.BuffCount; buff++) {
            if (Main.buffNoSave[buff]) {
                continue;
            }

            if (Main.debuff[buff]) {
                continue;
            }

            // if permabuff, don't
            if (Main.buffNoTimeDisplay[buff]) {
                continue;
            }

            if (Main.vanityPet[buff]) {
                continue;
            }

            if (Main.lightPet[buff]) {
                continue;
            }

            // patch calamity because they hardcode persistent buffs.....
            if (ZenithQoL.isCalamityLoaded()) {
                CalamityLogic3.addBuff(buff);
            }

            // Cache the original value so we can unload
            _persistentBuffs.Add(buff, Main.persistentBuff[buff]);
            Main.persistentBuff[buff] = true;
        }
    }

    // We have to manually undo changes to the array ;-;
    public override void Unload() {
        if (_persistentBuffs != null) {
            foreach ((int buff, bool originalValue) in _persistentBuffs) {
                Main.persistentBuff[buff] = originalValue;
            }

            _persistentBuffs.Clear();
            _persistentBuffs = null!;
        }
    }
}