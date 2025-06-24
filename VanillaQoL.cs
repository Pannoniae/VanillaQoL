using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using MagicStorage.Common.Systems;
using MonoMod.Cil;
using SerousCommonLib.API;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;
using VanillaQoL.Gameplay;
using VanillaQoL.IL;
using VanillaQoL.Shared;
using VanillaQoL.UI;

namespace VanillaQoL;

public class VanillaQoL : Mod {
    public static VanillaQoL instance = null!;

    public bool hasThorium;
    public bool hasCalamity;
    public bool hasCensus;
    public bool hasRecipeBrowser;
    public bool hasMagicStorage;
    public bool hasCheatSheet;
    public bool hasHEROsMod;
    public bool hasCalamityQoL;
    public bool hasQoLCompendium;

    public Mod? QoLCompendium;

    public override uint ExtraPlayerBuffSlots =>
        (uint)QoLConfig.Instance.moreBuffSlots;


    static VanillaQoL() {
    }

    public VanillaQoL() {
        instance = this;
        PreJITFilter = new Filter();
        // register modcompat for start of ModContent.Load
        var modcontent = typeof(ModContent);
        var method = modcontent.GetMethod("Load", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MonoModHooks.Modify(method, modCompat);
        Console.WriteLine("Registered mod compat handler.");
    }

    public static void noop(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitRet();
    }

    private void modCompat(ILContext il) {
        var ilCursor = new ILCursor(il);
        ilCursor.EmitCall(typeof(ModCompat).GetMethod("load")!);
    }

    public override void Load() {
        hasThorium = ModLoader.HasMod("ThoriumMod");
        hasCalamity = ModLoader.HasMod("CalamityMod");
        hasCensus = ModLoader.HasMod("Census");
        hasRecipeBrowser = ModLoader.HasMod("RecipeBrowser");
        hasMagicStorage = ModLoader.HasMod("MagicStorage");
        hasCheatSheet = ModLoader.HasMod("CheatSheet");
        hasHEROsMod = ModLoader.HasMod("HEROsMod");
        hasCalamityQoL = ModLoader.HasMod("CalamityQOL");
        hasQoLCompendium = ModLoader.HasMod("QoLCompendium");
        if (hasQoLCompendium) {
            QoLCompendium = ModLoader.GetMod("QoLCompendium");
        }

        if (QoLConfig.Instance.fixMemoryLeaks) {
            ModLeakFix.addHandler();
        }

        ILEdits.load();
        ModILEdits.load();
    }

    public override void Unload() {
        // unload modded keys
        LanguagePatch.unloadModdedKeys();

        // unregister
        var type = typeof(ChatManager);
        var handlers = type.GetField("_handlers", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ConcurrentDictionary<string, ITagHandler> _handlers =
            (ConcurrentDictionary<string, ITagHandler>)handlers.GetValue(null)!;

        _handlers["npc"] = null!;
        _handlers["t"] = null!;

        // unload
        if (LanguagePatch.loaded) {
            LanguagePatch.unload();
        }

        ILEdits.unload();
        GlobalFeatures.clear();

        // unload Magic Storage ModSystems - they crash the game
        if (hasMagicStorage) {
            ModCompat.unloadMagicStorageModSystems();
        }

        // unload *all* the IL edits
        // IL patch static lambdas are leaking memory, wipe them
        // this is now handled in TypeCaching.OnClear


        instance = null!;
    }

    public override void PostSetupContent() {
        Constants.postSetup();

        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggled");
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery");
        }

        // conditional localisation is not a thing....
        if (QoLConfig.Instance.pannoniaeCat) {
            instance.Logger.Info("meow!");
            LanguagePatch.addToCategory("CatNames_Siamese", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_Black", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_OrangeTabby", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_RussianBlue", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_Silver", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_White", "Pannoniae", "Pannoniae");
        }

        if (QoLConfig.Instance.coincumberingStoneRename) {
            LanguagePatch.modifyKey("ItemName.EncumberingStone", "Coincumbering Stone");
        }


        // load chat tags
        // since recipe browser's broken chat tags are loaded in Load(), we do it later to overwrite it:))
        ChatManager.Register<NPCTagHandler>("npc");
        ChatManager.Register<TextureTagHandler>("t");
        //instance.Logger.Info("cat!");
        //foreach (var cat in LanguageManager.Instance.GetKeysInCategory("CatNames_Siamese")) {
        //    instance.Logger.Info(cat);
        //}
    }

    /// <summary>
    /// Use to check for Calamity at load time.
    /// </summary>
    /// <returns></returns>
    public static bool isCalamityLoaded() {
        return ModLoader.HasMod("CalamityMod");
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        if (QoLConfig.Instance.mapSharingTESTING) {
            QoLSharedMapSystem.instance.HandlePacket(reader, whoAmI);
        }
        else {
            // we know the base stream is a MemoryStream
            var stream = (MemoryStream)reader.BaseStream;
            while (stream.ReadByte() != -1) {
                // read
            }
        }
    }

    public class Filter : PreJITFilter {
        public override bool ShouldJIT(MemberInfo member) {
            // if it's a type, check subtypes as well
            // SORRY FOR THE SPAGHETTI
            return member.DeclaringType?.GetCustomAttributes<MemberJitAttribute>()
                .All(a => a.ShouldJIT(member)) ?? member.GetCustomAttributes<MemberJitAttribute>()
                .All(a => a.ShouldJIT(member));
        }
    }
}

public static class ModCompat {
    public static void stopNulling(ILContext il) {
        var ilCursor = new ILCursor(il);
        // go to null-assignment followed by return
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchLdnull(), i => i.MatchStsfld(out _), i => i.MatchRet())) {
            VanillaQoL.instance.Logger.Error(
                "Failed to find null-assignment-and-return in MagicStorage SolidTopCollisionHackILEdits.LoadEdits");
            return;
        }

        // delete the ldnull/stsfld pair to patch out assignment of null to the field
        ilCursor.RemoveRange(2);
    }

    public static void unloadMagicStorageModSystems() {
        var magicStorage = ModLoader.GetMod("MagicStorage");

        VanillaQoL.instance.Logger.Info("Unloading MagicStorage ModSystems...");
        // foreach (var loadable in GetContent().Reverse()) {
        //    loadable.Unload();
        // }
        // Content.Clear();
        // Content = null;

        // internal ContentCache Content { get; private set; }
        var Content = magicStorage.GetType()
            .GetProperty("Content", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.GetValue(magicStorage);
        var contentClear = Content?.GetType()
            .GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var loadable in magicStorage.GetContent().Reverse()) {
            loadable.Unload();
        }

        //Content.Clear();
        //Content = null;
        if (contentClear != null) {
            contentClear.Invoke(Content, null);
        }

        // set content to null
        magicStorage.GetType().GetField("Content", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(magicStorage, null);

        // clear SystemLoader.Unload
        var systemLoaderType = typeof(SystemLoader);
        var unloadMethod2 =
            systemLoaderType.GetMethod("Unload", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (unloadMethod2 != null) {
            unloadMethod2.Invoke(null, []);
        }
        else {
            VanillaQoL.instance.Logger.Error("Failed to find SystemLoader Unload method");
        }

        // call SystemLoader.RebuildHooks to clear
        var rebuildHooksMethod = systemLoaderType.GetMethod("RebuildHooks",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (rebuildHooksMethod != null) {
            rebuildHooksMethod.Invoke(null, null);
        }
        else {
            VanillaQoL.instance.Logger.Error("Failed to find SystemLoader RebuildHooks method");
        }

        // clear watchdogs on MagicUI
        var magicUIType = magicStorage.GetType().Assembly.GetType("MagicStorage.Common.Systems.MagicUI");
        if (magicUIType == null) {
            VanillaQoL.instance.Logger.Error("Failed to find MagicUI type in MagicStorage assembly");
            return;
        }

        // private static readonly ConcurrentBag<RefreshUIWatchdog> _watchdogs = new();
        var watchdogsField = magicUIType.GetField("_watchdogs",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (watchdogsField == null) {
            VanillaQoL.instance.Logger.Error("Failed to find MagicUI _watchdogs field");
            return;
        }

        var watchdogs = watchdogsField.GetValue(null)!;
        // clear the watchdogs
        var clearMethod = watchdogs.GetType()
            .GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (clearMethod == null) {
            VanillaQoL.instance.Logger.Error("Failed to find MagicUI _watchdogs Clear method");
        }

        clearMethod?.Invoke(watchdogs, null);

        // dispose the watchdogs ThreadLocal field
        // ConcurrentBag<RefreshUIWatchdog>._locals
        var localsField = watchdogs.GetType()
            .GetField("_locals", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (localsField == null) {
            VanillaQoL.instance.Logger.Error("Failed to find MagicUI _locals field");
            return;
        }

        var locals = localsField.GetValue(watchdogs)!;
        // dispose the ThreadLocal
        // find the argumentless Dispose method
        var disposeMethod = locals.GetType()
            .GetMethod("Dispose", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, []);
        if (disposeMethod != null) {
            disposeMethod.Invoke(locals, null);
        }
        else {
            VanillaQoL.instance.Logger.Error("Failed to find ThreadLocal Dispose method");
        }

        // set watchdogs to null (it STILL exists because a local holds it - better to crash with a nulllpointerexception than to crash the entire process)
        //watchdogsField.SetValue(null, null);
    }

    public static void load() {
        VanillaQoL.instance.Logger.Info("Handling mod compatibility...");

        // fix magic storage stuff suppressing exceptions with the stupid logger
        // can't reference it directly because it might not be loaded

        var ilHelper = ModLoader.TryGetMod("SerousCommonLib", out var serousMod)
            ? serousMod.GetType().Assembly.GetType("SerousCommonLib.API.ILHelper")
            : null;
        var logMethodBody = ilHelper?.GetMethod("LogMethodBody",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (logMethodBody != null) {
            MonoModHooks.Modify(logMethodBody, VanillaQoL.noop);
        }

        // patch magic storage to stop nulling its delegates
        var hasMagicStorage = ModLoader.TryGetMod("MagicStorage", out var magicStorageMod);
        if (hasMagicStorage && magicStorageMod.Version.Major == 0 && magicStorageMod.Version.Minor == 7 && magicStorageMod.Version.Build == 0 && magicStorageMod.Version.Revision < 4) {
            var solidTopCollisionHackILEdits = magicStorageMod.GetType().Assembly
                .GetType("MagicStorage.Edits.SolidTopCollisionHackILEdits");
            var loadEdits =
                solidTopCollisionHackILEdits.GetMethod("LoadEdits", BindingFlags.Instance | BindingFlags.Public);
            MonoModHooks.Modify(loadEdits, stopNulling);
        }

        var str = new StringBuilder();
        bool atLeastOne = false;
        str.Append(
            "One or more of your mods are incompatible with each other. The incompatible mods are listed below: ");
        foreach (var mod in ModLoader.Mods) {
            // tML doesn't have a file, lol
            if (mod.Name == "ModLoader") {
                continue;
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var file = (TmodFile)mod.GetType().GetProperty("File", flags)!.GetValue(mod)!;
            try {
                file.Open();
                var compatFile = file.GetBytes("compat.txt");

                if (compatFile == null) {
                    continue;
                }

                str.Append($"\n    {mod.DisplayName} ({mod.Name}):");

                var content = Encoding.ASCII.GetString(compatFile);
                var entries = content.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries) {
                    var halves = entry.Split("=");
                    var resolvedMod = ModLoader.TryGetMod(halves[0], out var incompatMod);
                    if (resolvedMod) {
                        str.Append($"\n        {incompatMod.DisplayName} ({incompatMod.Name}): {halves[1]}");
                        atLeastOne = true;
                    }
                }
            }
            finally {
                file.GetType().GetMethod("Close", flags)!.Invoke(file, null);
            }
        }

        if (atLeastOne) {
            Terraria.Utils.ShowFancyErrorMessage(str.ToString(), 10006);
        }
    }
}

[NoJIT]
public static class ModLeakFix {
    public static bool hasMagicStorage;

    public static void unload() {
        if (hasMagicStorage) {
            // always use separate methods to avoid having to resolve the type if it isn't loaded
            magicStorageUnload();
        }

        ALCUnload(typeof(ModLeakFix));

        removeHandler();
    }

    public static void magicStorageUnload() {
        ALCUnload(typeof(MagicCache));
    }

    public static void addHandler() {
        hasMagicStorage = VanillaQoL.instance.hasMagicStorage;

        var typeCaching = typeof(AssemblyManager).Assembly.GetType("Terraria.ModLoader.Core.TypeCaching");
        var ev = typeCaching!.GetEvent("OnClear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ev.AddEventHandler(null, unload);
    }

    public static void removeHandler() {
        var typeCaching = typeof(AssemblyManager).Assembly.GetType("Terraria.ModLoader.Core.TypeCaching");
        var ev = typeCaching!.GetEvent("OnClear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        ev.RemoveEventHandler(null, unload);

        // wipe *this* action
        ALCUnload(typeof(ModLeakFix));

        // GC first because tml doesn't GC - thinks the mod reference is alive
        //for (int i = 0; i < 3; i++) {
        //    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        //    GC.WaitForPendingFinalizers();
        //}
    }

    public static void ALCUnload(Type type, bool onlyCompilerGeneratedClasses = false) {
        // LoaderAllocator hacking time
        // This is a RuntimeType
        // get the LoaderAllocator
        var loaderallocator =
            // ReSharper disable once PossibleMistakenCallToGetType.2
            type.GetType().GetField("m_keepalive", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(type)!;
        object[] m_slots =
            (object[])loaderallocator.GetType().GetField("m_slots", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(loaderallocator)!;
        // m_slots is an object[]
        // we loop over it, find the object[] arrays then clear each one of them
        for (int i = 0; i < m_slots.Length; i++) {
            var slot = m_slots[i];
            if (slot is object[] obj) {
                clear(obj, onlyCompilerGeneratedClasses);
            }
        }
    }

    private static void clear(object[] objects, bool onlyCompilerGeneratedClasses) {
        for (int i = 0; i < objects.Length; i++) {
            if (onlyCompilerGeneratedClasses) {
                var type = objects[i];
                // we got an already cleared reference?
                if (type == null!) {
                    continue;
                }

                var theType = type.GetType();
                // wtf
                if (theType == null!) {
                    continue;
                }

                var typename = theType.FullName ?? "N/A";

                var isFuncOrAction = typename.Contains("System.Func") || typename.Contains("System.Action");
                // we also clear System.Func<> and System.Action
                // ILContext.Manipulator
                if (typename.Contains("<>") || isFuncOrAction) {
                    objects[i] = null!;
                }
            }
            else {
                objects[i] = null!;
            }
        }
    }

    public static void ALCUnloadSpecific(Type type, Type typeToClear) {
        // LoaderAllocator hacking time
        // This is a RuntimeType
        // get the LoaderAllocator
        var loaderallocator =
            // ReSharper disable once PossibleMistakenCallToGetType.2
            type.GetType().GetField("m_keepalive", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(type)!;
        object[] m_slots =
            (object[])loaderallocator.GetType().GetField("m_slots", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(loaderallocator)!;
        // m_slots is an object[]
        // we loop over it, find the object[] arrays then clear each one of them
        for (int i = 0; i < m_slots.Length; i++) {
            var slot = m_slots[i];
            if (slot is object[] obj) {
                clearObj(obj, typeToClear);
            }
        }
    }

    private static void clearObj(object[] objects, Type typeToClear) {
        for (int i = 0; i < objects.Length; i++) {
            if (typeToClear.IsInstanceOfType(objects[i])) {
                objects[i] = null!;
            }
        }
    }
}
