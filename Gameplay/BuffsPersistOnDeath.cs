using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class BuffsPersistOnDeath : ModSystem
{
    private static Dictionary<int, bool>? _persistentBuffs;

    public override bool IsLoadingEnabled(Mod mod) => QoLConfig.Instance.persistentBuffs;

    public override void Load()
    {
        _persistentBuffs = new();

        // Make all saveable buffs persistent
        for (int buff = 0; buff < BuffLoader.BuffCount; buff++)
        {
            if (Main.buffNoSave[buff])
                continue;

            // Cache the original value so we can unload
            _persistentBuffs.Add(buff, Main.persistentBuff[buff]);
            Main.persistentBuff[buff] = true;
        }
    }

    // We have to manually undo changes to the array ;-;
    public override void Unload()
    {
        foreach ((int buff, bool originalValue) in _persistentBuffs)
        {
            Main.persistentBuff[buff] = originalValue;
        }

        _persistentBuffs?.Clear();
        _persistentBuffs = null;
    }
}
