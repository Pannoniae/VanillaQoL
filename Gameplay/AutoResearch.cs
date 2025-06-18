using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class AutoResearch : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.autoResearchFavourite;
    }

    public override void UpdateInventory(Item item, Player player) {
        if (item.favorited && item.stack >= CreativeUI.GetSacrificeCount(item.type, out bool researched) &&
            CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type,
                out int numResearch) && !researched && player.difficulty == PlayerDifficultyID.Creative &&
            item.stack >= numResearch) {
            if (item.type is ItemID.ShellphoneDummy or ItemID.ShellphoneHell or ItemID.ShellphoneOcean or ItemID.ShellphoneSpawn or ItemID.Shellphone) {
                return;
            }

            CreativeUI.ResearchItem(item.type);
            SoundEngine.PlaySound(in SoundID.ResearchComplete);
            item.stack -= numResearch;
            if (item.stack <= 0) {
                item.TurnToAir();
            }
        }
    }
}