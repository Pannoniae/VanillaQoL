using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class DynamicDCUPickaxe : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.dynamicDCUPickaxe;
    }

    public override void Load() {
        IL_Mount.UseDrill += fixDCUPickaxePatch;
    }

    public override void Unload() {
        IL_Mount.UseDrill -= fixDCUPickaxePatch;
    }

    private void fixDCUPickaxePatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Mount>("drillPickPower"))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match first drillPickPower in Mount.UseDrill!");
            return;
        }

        ilCursor.EmitLdarg1();
        ilCursor.Emit<DynamicDCUPickaxe>(OpCodes.Call, "modifyPickPower");

        if (!ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Mount>("drillPickPower"))) {
            ZenithQoL.instance.Logger.Warn("Couldn't match second drillPickPower in Mount.UseDrill!");
            return;
        }

        ilCursor.EmitLdarg1();
        ilCursor.Emit<DynamicDCUPickaxe>(OpCodes.Call, "modifyPickPower");
    }

    public static int modifyPickPower(int orig, Player player) {
        return player.GetModPlayer<DynamicDCUPickaxePlayer>().topPickPower;
    }
}

public class DynamicDCUPickaxePlayer : ModPlayer {

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.dynamicDCUPickaxe;
    }

    public int topPickPower;

    public override void PostUpdateMiscEffects() {
        for (int i = 0; i < Player.inventory.Length; i++) {
            if (Player.inventory[i].pick != 0) {
                topPickPower = Player.inventory[i].pick;
                break;
            }
        }
    }
}