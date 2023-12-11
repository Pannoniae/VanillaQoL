using System.IO;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using VanillaQoL.API;
using VanillaQoL.Config;
using VanillaQoL.Gameplay;
using VanillaQoL.IL;
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

    public VanillaQoL() {
        instance = this;

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
        ILEdits.load();
        ModILEdits.load();
    }

    public override void Unload() {
        if (LanguagePatch.loaded) {
            LanguagePatch.unload();
        }

        ILEdits.unload();

        // IL patch static lambdas are leaking memory, wipe them
        Utils.completelyWipeClass(typeof(ILEdits));
        Utils.completelyWipeClass(typeof(ModILEdits));
        Utils.completelyWipeClass(typeof(RecipeBrowserLogic));
        Utils.completelyWipeClass(typeof(MagicStorageLogic));
        Utils.completelyWipeClass(typeof(QoLSharedMapSystem));
        Utils.completelyWipeClass(typeof(LockOn));
        Utils.completelyWipeClass(typeof(DisableTownSlimes));
        Utils.completelyWipeClass(typeof(NurseHealing));
        Utils.completelyWipeClass(typeof(AccessoryLoadoutSupport));
        Utils.completelyWipeClass(typeof(AccessorySlotUnlock));
        // Func<bool> is a static lambda, this would leak as well

        // memory leak fix
        //if (QoLConfig.Instance.fixMemoryLeaks) {
        //    if (instance.hasHEROsMod) {

        //    }
        //}


        instance = null!;
    }

    public override void PostSetupContent() {
        if (QoLConfig.Instance.removeThoriumEnabledCraftingTooltips) {
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggled");
            LanguagePatch.hideKey("Mods.ThoriumMod.Conditions.DonatorItemToggledSteamBattery");
        }

        // load chat tags
        // since recipe browser's broken chat tags are loaded in Load(), we do it later to overwrite it:))
        ChatManager.Register<NPCTagHandler>("npc");
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
}