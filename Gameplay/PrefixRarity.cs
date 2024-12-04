using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace ZenithQoL.Gameplay;

public class PrefixRarity : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.disablePrefixChangingRarity;
    }

    public override void Load() {
        IL_Item.Prefix += prefixRarityPatch;
    }

    public override void Unload() {
        IL_Item.Prefix -= prefixRarityPatch;
    }

    // basically, we skip the entire section with the prefix code.
    private void prefixRarityPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        // match this: // [977 7 - 977 27]
        // IL_0312: ldarg.0      // this
        // IL_0313: ldfld        int32 Terraria.Item::rare
        // IL_0318: stloc.s      rare
        if (ilCursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(),
                i => i.MatchLdfld<Item>("rare"),
                i => i.MatchStloc(out var _))) {
            // should be 12
        }
        else {
            ZenithQoL.instance.Logger.Warn("Failed to locate start of prefix code at Item.Prefix (Item.rare)");
            return;
        }

        // then find this and match before: // [997 7 - 997 29]
        // IL_03fd: ldloc.s      valueMult
        // IL_03ff: ldloc.s      valueMult
        // IL_0401: mul
        // IL_0402: stloc.s      valueMult

        var ilCursor2 = new ILCursor(ilCursor);
        if (ilCursor2.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(out var _),
                i => i.MatchLdloc(out var _),
                i => i.MatchMul(),
                i => i.MatchStloc(out var _))) {
            var label = ilCursor2.MarkLabel();
            ilCursor.EmitBr(label);
        }
        else {
            ZenithQoL.instance.Logger.Warn("Failed to locate end of prefix code at Item.Prefix (valueMult)");
        }
    }
}