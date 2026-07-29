using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;

/**
 * The Obsidian Skull line, the Ankh Shield and the Terraspark Boots used to make you immune to On Fire!.... they removed this. Time to fix
 */
[JITWhenModsEnabled("CalamityMod")]
public class FireImmunityRestore : GlobalItem {
    private static HashSet<int> fireproof = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.fireImmunity;
    }

    public override void SetStaticDefaults() {
        fireproof = [
            ItemID.ObsidianSkull, ItemID.ObsidianHorseshoe, ItemID.ObsidianShield,
            ItemID.ObsidianWaterWalkingBoots, ItemID.LavaWaders, ItemID.ObsidianSkullRose, ItemID.MoltenCharm,
            ItemID.LavaSkull, ItemID.MoltenSkullRose, ItemID.AnkhShield, ItemID.TerrasparkBoots
        ];
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
        if (fireproof.Contains(item.type)) {
            player.buffImmune[BuffID.OnFire] = true;
        }
    }
}
