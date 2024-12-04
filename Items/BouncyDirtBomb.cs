using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZenithQoL.Items;

public class BouncyDirtBomb : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moreExplosives;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
        ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.shootSpeed = 5f;
        Item.shoot = ModContent.ProjectileType<BouncyDirtBombProjectile>();
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.UseSound = SoundID.Item1;
        Item.useAnimation = 25;
        Item.useTime = 25;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.value = Item.buyPrice(0, 0, 8);
        Item.damage = 0;
    }

    public override void AddRecipes() {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.DirtBomb);
        recipe.AddIngredient(ItemID.PinkGel);
        recipe.Register();
        var recipe2 = CreateRecipe();
        recipe2.AddIngredient(ItemID.BouncyBomb);
        recipe2.AddIngredient(ItemID.DirtBlock, 25);
        recipe2.Register();
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Bombs;
    }
}

public class BouncyDirtBombProjectile : ModProjectile {
    public override string Texture => "ZenithQoL/Items/BouncyDirtBomb";

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.moreExplosives;
    }

    public int width = 22;
    public int height = 22;

    public override void SetDefaults() {
        DrawOriginOffsetY = -6;
        Projectile.width = width;
        Projectile.height = height;

        Projectile.friendly = true;
        Projectile.penetrate = -1;
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
        Explosives.dirtExplosionCode(Projectile, width, height, 4.2f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        Explosives.bounce(Projectile, oldVelocity);
        return false;
    }

    public override void AI() {
        Explosives.explosiveAI(Projectile, 48, 48, 100, 12f);
    }
}