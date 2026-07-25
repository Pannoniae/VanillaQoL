using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using VanillaQoL.Config;

namespace VanillaQoL.IL;

public class ConfigSyncFix : ModSystem {
    private static IDictionary<Mod, List<ModConfig>>? configs;

    public override bool IsLoadingEnabled(Mod mod) {
        return UIConfig.Instance.fixConfigSyncCrash;
    }

    public override void Load() {
        var method = typeof(ModNet).GetMethod("AnyModNeedsReloadFromNetConfigsCheckOnly",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null) {
            VanillaQoL.instance.Logger.Warn("Couldn't find AnyModNeedsReloadFromNetConfigsCheckOnly!");
            return;
        }

        MonoModHooks.Modify(method, skipMissingConfigs);
    }

    // IL_0042: ldloc.1
    // IL_0043: ldfld        class ModNet/NetConfig ModNet/'<>c__DisplayClass42_0'::pendingConfig
    // IL_0048: ldfld        string ModNet/NetConfig::modname
    // IL_004d: call         bool ModLoader::HasMod(string)
    // IL_0052: brfalse      IL_0125
    private void skipMissingConfigs(ILContext il) {
        var ilCursor = new ILCursor(il);
        FieldReference? pendingConfig = null;
        FieldReference? modname = null;
        ILLabel? skip = null;
        if (!ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(1),
                i => i.MatchLdfld(out pendingConfig),
                i => i.MatchLdfld(out modname),
                i => i.MatchCall(typeof(ModLoader).GetMethod("HasMod")!),
                i => i.MatchBrfalse(out skip))) {
            VanillaQoL.instance.Logger.Warn("Couldn't find the HasMod check in the netconfig reload check!");
            return;
        }

        var configname = il.Module.ImportReference(
            modname!.DeclaringType.Resolve().Fields.First(f => f.Name == "configname"));

        // the brfalse target is the MoveNext target, so "borrowing" it is just continuing
        ilCursor.Emit(OpCodes.Ldloc_1);
        ilCursor.Emit(OpCodes.Ldfld, pendingConfig!);
        ilCursor.Emit(OpCodes.Ldfld, modname);
        ilCursor.Emit(OpCodes.Ldloc_1);
        ilCursor.Emit(OpCodes.Ldfld, pendingConfig!);
        ilCursor.Emit(OpCodes.Ldfld, configname);
        ilCursor.Emit<ConfigSyncFix>(OpCodes.Call, "configExists");
        ilCursor.Emit(OpCodes.Brfalse, skip!);
    }

    public static bool configExists(string modName, string configName) {
        configs ??= (IDictionary<Mod, List<ModConfig>>)typeof(ConfigManager)
            .GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        return ModLoader.TryGetMod(modName, out var mod) && configs.TryGetValue(mod, out var list) &&
               list.Any(c => c.Name == configName);
    }
}
