using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

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
            if (VanillaQoL.isCalamityLoaded()) {
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

public class StationBuffsPersistOnDeath : ModSystem {
    private static readonly int[] stationBuffs =
        [BuffID.AmmoBox, BuffID.Bewitched, BuffID.Clairvoyance, BuffID.Sharpened, BuffID.WarTable, BuffID.SugarRush];

    private static readonly bool[] prevStationBuffs = new bool[stationBuffs.Length];

    public override bool IsLoadingEnabled(Mod mod) => QoLConfig.Instance.deathLessBuffs;

    public override void PostSetupContent() {
        for (var i = 0; i < stationBuffs.Length; i++) {
            var buff = stationBuffs[i];
            prevStationBuffs[i] = Main.persistentBuff[buff];
            Main.persistentBuff[buff] = true;

            // patch calamity because they hardcode persistent buffs.....
            if (VanillaQoL.isCalamityLoaded()) {
                CalamityLogic3.addBuff(buff);
            }
        }
    }

    public override void Unload() {
        for (var i = 0; i < stationBuffs.Length; i++) {
            Main.persistentBuff[stationBuffs[i]] = prevStationBuffs[i];
        }
    }
}