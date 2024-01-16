using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MagicStorage.Common.Systems;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;
using VanillaQoL.Gameplay;
using VanillaQoL.IL;
using VanillaQoL.Items;
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

        // unload *all* the IL edits
        // IL patch static lambdas are leaking memory, wipe them
        // this is now handled in TypeCaching.OnClear


        instance = null!;
    }

    public override void PostSetupContent() {
        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggled");
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery");
        }
        // conditional localisation is not a thing....
        if (QoLConfig.Instance.pannoniaeCat) {
            instance.Logger.Info("meow!");
            LanguagePatch.addToCategory("CatNames_Siamese", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_Black", "Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_OrangeTabby", "Pannoniae",  "Pannoniae");
            LanguagePatch.addToCategory("CatNames_RussianBlue","Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_Silver","Pannoniae", "Pannoniae");
            LanguagePatch.addToCategory("CatNames_White","Pannoniae", "Pannoniae");
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
        if (QoLConfig.Instance.mapSharing) {
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
    public static void load() {
        VanillaQoL.instance.Logger.Info("Handling mod compatibility...");
        var str = new StringBuilder();
        bool atLeastOne = false;
        str.Append(
            "One or more of your mods are incompatible with each other. The incompatible mods are listed below: ");
        foreach (var mod in ModLoader.Mods) {
            // tML doesn't have a file, lol
            if (mod.Name == "ModLoader") {
                continue;
            }

            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var file = (TmodFile)mod.GetType().GetProperty("File", flags)!.GetValue(mod)!;
            try {
                file.Open();
                var compatFile = file.GetBytes("compat.txt");

                if (compatFile == null) {
                    continue;
                }

                str.Append($"\n    {mod.DisplayName} ({mod.Name}):");

                var content = Encoding.ASCII.GetString(compatFile);
                var entries = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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