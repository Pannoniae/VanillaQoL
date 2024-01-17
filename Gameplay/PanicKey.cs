using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class PanicKey : ModSystem {
    public static PanicKey instance = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.panicKey;
    }

    public static ModKeybind panicKeybind { get; private set; } = null!;

    public override void Load() {
        instance = this;
        panicKeybind = KeybindLoader.RegisterKeybind(Mod, "PanicKey", "Q");
    }
}

public class PanicKeyPlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.panicKey;
    }

    public override void ProcessTriggers(TriggersSet triggersSet) {
        if (PanicKey.panicKeybind.JustPressed) {
            useTeleportItem();
        }
    }

    public void useTeleportItem() {
        // priority: recall potion -> magic mirror

        var itemCheck_CheckCanUse =
            typeof(Player).GetMethod("ItemCheck_CheckCanUse", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var magicMirrorIdx = -1;
        for (int index = 0; index < 58; ++index) {
            var item = Player.inventory[index];
            if (Constants.isRecall(item) &&
                // ReSharper disable once CoVariantArrayConversion
                (bool)itemCheck_CheckCanUse.Invoke(Player, new[] { item })!) {
                // mark magic mirror
                magicMirrorIdx = index;

                // we don't want to continue searching if we don't have to
                break;
            }

            if (item.type == ItemID.RecallPotion) {
                useItem(index);
                return;
            }
        }

        // if there's a magic mirror
        if (magicMirrorIdx != -1) {
            useItem(magicMirrorIdx);
        }
    }

    public void useItem(int idx) {
        // TODO: As of 1.4, this doesn't seem to honor not activating while an item is already in use.
        if (Player.inventory[idx].type != ItemID.None) {
            Player.selectedItem = idx;
            Player.controlUseItem = true;
            Player.ItemCheck();
        }
    }
}