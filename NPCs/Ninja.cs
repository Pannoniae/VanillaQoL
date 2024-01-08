using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace VanillaQoL.NPCs;

[AutoloadHead]
public class Ninja : ModNPC {
    public static LocalizedText chat1 = null!;
    public static LocalizedText chat2 = null!;
    public static LocalizedText chat3 = null!;
    public static LocalizedText chat4 = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.ninja;
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 26;
        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 90;
        NPCID.Sets.AttackAverageChance[Type] = 25;
        NPCID.Sets.HatOffsetY[Type] = 4;

        NPC.Happiness
            .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
            .SetBiomeAffection<OceanBiome>(AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Nurse, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Hate);

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new() {
            Velocity = 1f, // Draws the NPC in the bestiary as if it's walking +1 tiles in the x direction
            Direction = -1 // Faces left
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);


        // chat
        chat1 = this.GetLocalization("chat1");
        chat2 = this.GetLocalization("chat2");
        chat3 = this.GetLocalization("chat3");
        chat4 = this.GetLocalization("chat4");
    }

    public override void SetDefaults() {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive; // Copies the AI of passive NPCs. This is AI Style 7.
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;

        AnimationType = NPCID.Guide;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) {
        return QoLConfig.Instance.ninja && NPC.downedSlimeKing;
    }

    public override string GetChat() {
        WeightedRandom<string> chat = new();

        chat.Add(chat1.Value);
        chat.Add(chat2.Value);
        chat.Add(chat3.Value);
        chat.Add(chat4.Value);

        return chat;
    }

    public override List<string> SetNPCNameList() {
        return new List<string> {
            "Danzo",
            "Ebisu",
            "Hatake",
            "Kakuzu",
            "Tenzen"
        };
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
    }

    public override void AddShops() {
        var shop = new NPCShop(Type);
        shop.Add(ItemID.DynastyWood);
        shop.Add(ItemID.RedDynastyShingles);
        shop.Add(ItemID.BlueDynastyShingles);
        shop.Add(ItemID.BreathingReed);
        shop.Add(ItemID.Katana);
        shop.Add(ItemID.PandaEars);
        shop.Add(ItemID.Fedora);
        shop.Add(ItemID.BambooLeaf);
        shop.Add(ItemID.Kimono);
        shop.Add(ItemID.GameMasterShirt);
        shop.Add(ItemID.GameMasterPants);
        shop.Add(ItemID.TigerSkin);
        shop.Add(ItemID.LeopardSkin);
        shop.Add(ItemID.ZebraSkin);
        shop.Add(ItemID.Gi);
        shop.Add(ItemID.Pho);
        shop.Add(ItemID.PadThai);
        shop.Register();
    }

    public override bool UsesPartyHat() {
        return false;
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        damage = 15;
        knockback = 4f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        cooldown = 30;
        randExtraCooldown = 30;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        // Throwing
        projType = ProjectileID.Shuriken;
        attackDelay = 10;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection,
        ref float randomOffset) {
        // Throwing
        multiplier = 12f;
        gravityCorrection = 2f;
        randomOffset = 1f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            // The first line is for the background. Auto complete is recommended to see the available options.
            // Generally, this is set to the background of the biome that the Town NPC most loves/likes, but it is not automatic.
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            // Examples for how to modify the background
            // BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Blizzard,
            // BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

            // This line is for the description of the entry. We are accessing a localization key here.
            new FlavorTextBestiaryInfoElement("Mods.VanillaQoL.NPCs.Ninja.Bestiary")
        });
    }
}