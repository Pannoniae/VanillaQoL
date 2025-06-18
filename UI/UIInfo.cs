using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.Social;

namespace VanillaQoL.UI;

public class UIInfo : ModSystem, ILocalizedModType {
    public string LocalizationCategory => "Info";

    public static LocalizedText demonHeart { get; private set; } = null!;
    public static LocalizedText torchGod { get; private set; } = null!;
    public static LocalizedText artisanBread { get; private set; } = null!;
    public static LocalizedText aegisCrystal { get; private set; } = null!;
    public static LocalizedText aegisFruit { get; private set; } = null!;
    public static LocalizedText arcaneCrystal { get; private set; } = null!;
    public static LocalizedText ambrosia { get; private set; } = null!;
    public static LocalizedText gummyWorm { get; private set; } = null!;
    public static LocalizedText galaxyPearl { get; private set; } = null!;
    public static LocalizedText minecartUpgradeKit { get; private set; } = null!;


    public static LocalizedText hardMode { get; private set; } = null!;
    public static LocalizedText combatBook { get; private set; } = null!;
    public static LocalizedText combatBookVolumeTwo { get; private set; } = null!;
    public static LocalizedText peddlersSatchel { get; private set; } = null!;

    public override void SetStaticDefaults() {
        demonHeart = this.GetLocalization(nameof(demonHeart));
        torchGod = this.GetLocalization(nameof(torchGod));
        artisanBread = this.GetLocalization(nameof(artisanBread));
        aegisCrystal = this.GetLocalization(nameof(aegisCrystal));
        aegisFruit = this.GetLocalization(nameof(aegisFruit));
        arcaneCrystal = this.GetLocalization(nameof(arcaneCrystal));
        ambrosia = this.GetLocalization(nameof(ambrosia));
        gummyWorm = this.GetLocalization(nameof(gummyWorm));
        galaxyPearl = this.GetLocalization(nameof(galaxyPearl));
        minecartUpgradeKit = this.GetLocalization(nameof(minecartUpgradeKit));
        hardMode = this.GetLocalization(nameof(hardMode));
        combatBook = this.GetLocalization(nameof(combatBook));
        combatBookVolumeTwo = this.GetLocalization(nameof(combatBookVolumeTwo));
        peddlersSatchel = this.GetLocalization(nameof(peddlersSatchel));
    }

    // tml logic
    // in the good old tml tradition, we misuse the vanilla terraria info accessory icons...
    // HEH NOT ANYMORE THANK YOU SIRSWERVING!
    // todo this is a massive piece of duplicated shit, maybe write some actually proper code this time?
    public static void playerInfo(UICharacterListItem character) {
        var player = character.Data.Player;
        bool[] things = [
            player.extraAccessory,
            player.unlockedBiomeTorches,
            player.ateArtisanBread,
            player.usedAegisCrystal,
            player.usedAegisFruit,
            player.usedArcaneCrystal,
            player.usedAmbrosia,
            player.usedGummyWorm,
            player.usedGalaxyPearl,
            player.unlockedSuperCart
        ];

        LocalizedText[] locKeys = [
            demonHeart,
            torchGod,
            artisanBread,
            aegisCrystal,
            aegisFruit,
            arcaneCrystal,
            ambrosia,
            gummyWorm,
            galaxyPearl,
            minecartUpgradeKit
        ];

        string[] paths = [
            "Demon_Heart",
            "Torch_God's_Favor",
            "Artisan_Loaf",
            "Vital_Crystal",
            "Aegis_Fruit",
            "Arcane_Crystal",
            "Ambrosia",
            "Gummy_Worm",
            "Galaxy_Pearl",
            "Minecart_Upgrade_Kit"
        ];

        int offset = -40;
        for (int index = 0; index < things.Length; ++index) {
            if (things[index]) {
                UIHoverImage uiHoverImage =
                    new UIHoverImage(
                        ModContent.Request<Texture2D>("VanillaQoL/Assets/" + paths[index],
                            AssetRequestMode.ImmediateLoad),
                        locKeys[index].Format(things[index]));
                uiHoverImage.Left.Pixels = offset;
                uiHoverImage.Left.Percent = 1f;
                character.Append(uiHoverImage);
                offset -= 18;
            }
        }
    }

    public static void worldInfo(UIWorldListItem world) {
        var info = world.Data;
        var additionalData = getAdditionalData(info.Path, info.IsCloudSave);
        bool[] things = {
            info.IsHardMode,
            additionalData.combatBook,
            additionalData.combatBookVolumeTwo,
            additionalData.peddlersSatchel
        };

        LocalizedText[] locKeys = {
            hardMode,
            combatBook,
            combatBookVolumeTwo,
            peddlersSatchel
        };

        string[] paths = {
            "Hardmode",
            "Advanced_Combat_Techniques",
            "Advanced_Combat_Techniques_Volume_Two",
            "Peddler's_Satchel"
        };
        const int margin = 2;
        int offset = -40;
        for (int index = 0; index < things.Length; ++index) {
            if (things[index]) {
                UIHoverImage uiHoverImage =
                    new UIHoverImage(
                        ModContent.Request<Texture2D>("VanillaQoL/Assets/" + paths[index],
                            AssetRequestMode.ImmediateLoad),
                        locKeys[index].Format(things[index]));
                uiHoverImage.Left.Pixels = offset;
                uiHoverImage.Left.Percent = 1f;
                world.Append(uiHoverImage);
                offset -= 18 + margin;
            }
        }
    }

    public static AdditionalWorldFileData getAdditionalData(string file, bool cloudSave) {
        var data = new WorldFileData(file, cloudSave);
        var additionalData = new AdditionalWorldFileData();
        try {
            using (Stream input = cloudSave
                       ? new MemoryStream(SocialAPI.Cloud.Read(file))
                       : new FileStream(file, FileMode.Open)) {
                using (BinaryReader reader = new BinaryReader(input)) {
                    int version = reader.ReadInt32();
                    data.Metadata = FileMetadata.Read(reader, FileType.World);
                    _ = (int)reader.ReadInt16();
                    input.Position = reader.ReadInt32();
                    data.Name = reader.ReadString();
                    string seedText = version != 179 ? reader.ReadString() : reader.ReadInt32().ToString();
                    data.SetSeed(seedText);
                    data.WorldGeneratorVersion = reader.ReadUInt64();


                    data.UniqueId = new Guid(reader.ReadBytes(16));
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    int y = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    data.SetWorldSize(x, y);
                    data.GameMode = reader.ReadInt32();
                    data.DrunkWorld = reader.ReadBoolean();
                    data.ForTheWorthy = reader.ReadBoolean();
                    data.Anniversary = reader.ReadBoolean();
                    data.DontStarve = reader.ReadBoolean();
                    data.NotTheBees = reader.ReadBoolean();
                    data.RemixWorld = reader.ReadBoolean();
                    data.NoTrapsWorld = reader.ReadBoolean();
                    data.ZenithWorld =
                        reader.ReadBoolean();


                    data.CreationTime = version < 141
                        ? (cloudSave ? DateTime.Now : File.GetCreationTime(file))
                        : DateTime.FromBinary(reader.ReadInt64());
                    reader.ReadByte();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadDouble();
                    reader.ReadDouble();
                    reader.ReadDouble();
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    data.HasCrimson = reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadByte();
                    reader.ReadInt32();
                    data.IsHardMode = reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadDouble();
                    reader.ReadDouble();
                    reader.ReadByte();

                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadSingle();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadInt32();
                    reader.ReadInt16();
                    reader.ReadSingle();
                    for (int index = reader.ReadInt32(); index > 0; --index) {
                        reader.ReadString();
                    }

                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    int num17 = reader.ReadInt16();
                    for (int index = 0; index < num17; ++index) {
                        reader.ReadInt32();
                    }

                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    data.DefeatedMoonlord = reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    int num2 = reader.ReadInt32();
                    for (int index = 0; index < num2; ++index) {
                        reader.ReadInt32();
                    }


                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadSingle();
                    reader.ReadSingle();


                    // DD2Event.Load
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();


                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    additionalData.combatBook = reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();


                    //TreeTops.Load
                    int num = reader.ReadInt32();
                    for (int index = 0; index < num && index < TreeTopsInfo.AreaId.Count; ++index) {
                        reader.ReadInt32();
                    }

                    reader.ReadBoolean();
                    reader.ReadBoolean();

                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();


                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();

                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    reader.ReadBoolean();

                    additionalData.combatBookVolumeTwo = reader.ReadBoolean();
                    additionalData.peddlersSatchel = reader.ReadBoolean();

                    return additionalData;
                }
            }
        }
        catch (Exception e) {
            // ignored
            VanillaQoL.instance.Logger.Warn(e);
            return additionalData;
        }
    }
}

public class AdditionalWorldFileData {
    public bool combatBookVolumeTwo;
    public bool peddlersSatchel;
    public bool combatBook;
}

internal class UIHoverImage : UIImage {
    internal string HoverText;
    internal bool UseTooltipMouseText; // Not sure if all would benefit from this, opt in.

    public UIHoverImage(Asset<Texture2D> texture, string hoverText) : base(texture) {
        HoverText = hoverText;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering) {
            var bounds = Parent.GetDimensions().ToRectangle();
            bounds.Y = 0;
            bounds.Height = Main.screenHeight;
            if (UseTooltipMouseText)
                UICommon.TooltipMouseText(HoverText);
            else
                UICommon.DrawHoverStringInBounds(spriteBatch, HoverText, bounds);
        }
    }
}