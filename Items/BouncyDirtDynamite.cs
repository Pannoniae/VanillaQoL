using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Items;

public class BouncyDirtDynamite : ModItem {
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
        Item.shoot = ModContent.ProjectileType<BouncyDirtDynamiteProjectile>();
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
        recipe.AddIngredient(ItemID.PinkGel);
        recipe.Register();
        var recipe2 = CreateRecipe();
        recipe2.AddIngredient(ItemID.BouncyDynamite);
        recipe2.AddIngredient(ItemID.PinkGel);
        recipe2.Register();
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Bombs;
    }
}

public class BouncyDirtDynamiteProjectile : ModProjectile {
    public override string Texture => "VanillaQoL/Items/BouncyDirtDynamite";

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
        Projectile.timeLeft = 300;
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

    public override bool OnTileCollide(Vector2 oldVelocity) {
        Explosives.bounce(Projectile, oldVelocity);
        return false;
    }

    public override void AI() {
        Explosives.explosiveAI(Projectile, 250, 250, 250, 10f);
    }
}