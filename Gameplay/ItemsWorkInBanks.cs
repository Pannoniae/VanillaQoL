using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class ItemsWorkInBanks : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.itemsWorkInBanks;
    }

    public override void UpdateEquips() {
        foreach (var item in Player.bank.item) {
            checkForStorage(item);
        }

        foreach (var item in Player.bank2.item) {
            checkForStorage(item);
        }

        foreach (var item in Player.bank3.item) {
            checkForStorage(item);
        }

        foreach (var item in Player.bank4.item) {
            checkForStorage(item);
        }
    }

    public void checkForStorage(Item item) {
        if (Constants.bankItems.Contains(item.type)) {
            Player.ApplyEquipFunctional(item, true);
            Player.RefreshInfoAccsFromItemType(item);
            Player.RefreshMechanicalAccsFromItemType(item.type);

            //Royal Gel Compatibility
            if (item.type == ItemID.RoyalGel) {
                //Thorium Slimes
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "Clot")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "GelatinousCube")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "GelatinousSludge")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "GildedSlime")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "GildedSlimeling")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "GraniteFusedSlime")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "LivingHemorrhage")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "SpaceSlime")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "CrownofThorns")] = true;
                Player.npcTypeNoAggro[Constants.GetModNPC(ModContentCompat.thoriumMod, "BloodDrop")] = true;
            }
        }
    }
}