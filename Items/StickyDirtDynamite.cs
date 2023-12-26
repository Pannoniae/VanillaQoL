using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Items;

public class StickyDirtDynamite : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moreExplosives;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
        ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.shootSpeed = 4f;
        Item.shoot = ModContent.ProjectileType<StickyDirtDynamiteProjectile>();
        Item.width = 8;
        Item.height = 28;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.UseSound = SoundID.Item1;
        Item.useAnimation = 40;
        Item.useTime = 40;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.value = Item.buyPrice(0, 0, 30);
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes() {
        var recipe = CreateRecipe();
        recipe.AddIngredient<DirtDynamite>();
        recipe.AddIngredient(ItemID.Gel);
        recipe.Register();
        var recipe2 = CreateRecipe();
        recipe2.AddIngredient(ItemID.StickyDynamite);
        recipe2.AddIngredient(ItemID.DirtBlock, 25);
        recipe2.Register();
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Bombs;
    }
}

public class StickyDirtDynamiteProjectile : ModProjectile {
    public override string Texture => "VanillaQoL/Items/StickyDirtDynamite";

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moreExplosives;
    }

    public int width = 10;
    public int height = 10;

    public override void SetDefaults() {
        DrawOriginOffsetY = -11;
        Projectile.width = width;
        Projectile.height = height;

        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (Main.expertMode) {
            if (target.type is >= NPCID.EaterofWorldsHead and <= NPCID.EaterofWorldsTail) {
                modifiers.FinalDamage /= 5; // don't hurt NPCs, vanilla doesn't either
            }
        }
        Explosives.NPCDamage(Projectile, target);
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) {
        Explosives.PlayerDamage(Projectile, target);
    }

    public override void OnKill(int timeLeft) {
        Projectile.tileCollide = false;
        Explosives.dirtExplosionCode(Projectile, width, height, 7);
    }

    public override void AI() {
        Explosives.explosiveAI(Projectile, 250, 250, 250, 10f);
    }
}