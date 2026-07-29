using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using VanillaQoL.IL;

namespace VanillaQoL.Gameplay;

// This is like Census but kinda unfucked, instead of drawing it completely separately, we inject into vanilla properly so the UI spacing and everything works
public class NPCCensus : ModSystem {
    public const byte spawnPacketID = 200;

    // townNPCCanSpawn is server-side, so it should be synced
    public static bool synced;

    private static readonly Dictionary<int, LocalizedText> conditions = new();
    private static List<(int type, LocalizedText conditions)> entries = [];

    private static int lastHoverNPC = -1;
    private static uint lastHoverTick;
    private static string happiness = "";

    private static readonly FieldInfo hidePVPIconsField =
        typeof(Main).GetField("hidePVPIcons", BindingFlags.NonPublic | BindingFlags.Static)!;

    public override bool IsLoadingEnabled(Mod mod) {
        return (QoLConfig.Instance.betterCensus || QoLConfig.Instance.npcHappinessOnHover ||
                QoLConfig.Instance.npcLocatingArrow) &&
               !Spoof.realHasMod("Census");
    }

    public override void Load() {
        IL_Main.DrawNPCHousesInUI += injectRows;
        IL_Main.UpdateTime_SpawnTownNPCs += canSpawnPatch;
    }

    public override void Unload() {
        IL_Main.DrawNPCHousesInUI -= injectRows;
        IL_Main.UpdateTime_SpawnTownNPCs -= canSpawnPatch;
        conditions.Clear();
        entries = [];
        happiness = "";
        lastHoverNPC = -1;
    }

    public override void OnWorldLoad() {
        synced = false;
        lastHoverNPC = -1;
    }

    // mods deliver these through the facade during PostSetupContent
    public static void registerCondition(int type, LocalizedText? conditions) {
        if (conditions is not null) {
            NPCCensus.conditions[type] = conditions;
        }
    }

    public override void PostAddRecipes() {
        entries = [];
        int[] vanillaTownNPCs = [
            NPCID.Guide, NPCID.Merchant, NPCID.Nurse, NPCID.Demolitionist, NPCID.DyeTrader,
            NPCID.Angler, NPCID.BestiaryGirl, NPCID.Dryad, NPCID.Painter, NPCID.Golfer,
            NPCID.ArmsDealer, NPCID.DD2Bartender, NPCID.Stylist, NPCID.GoblinTinkerer,
            NPCID.WitchDoctor, NPCID.Clothier, NPCID.Mechanic, NPCID.PartyGirl, NPCID.Wizard,
            NPCID.TaxCollector, NPCID.Truffle, NPCID.Pirate, NPCID.Steampunker, NPCID.Cyborg,
            NPCID.SantaClaus, NPCID.Princess, NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny,
            NPCID.TownSlimeBlue, NPCID.TownSlimeGreen, NPCID.TownSlimeOld, NPCID.TownSlimePurple,
            NPCID.TownSlimeRainbow, NPCID.TownSlimeRed, NPCID.TownSlimeYellow, NPCID.TownSlimeCopper
        ];

        foreach (var type in vanillaTownNPCs) {
            var key = $"Mods.VanillaQoL.SpawnConditions.{NPCID.Search.GetName(type)}";
            var conditions = Language.Exists(key)
                ? Language.GetText(key)
                : Language.GetText("Mods.VanillaQoL.SpawnConditions.Unknown");
            entries.Add((type, conditions));
        }

        foreach (var modNPC in ModContent.GetContent<ModNPC>()) {
            if (!modNPC.NPC.townNPC || NPC.TypeToDefaultHeadIndex(modNPC.NPC.type) < 0 ||
                modNPC.TownNPCStayingHomeless) {
                continue;
            }

            // Ours > Census key if both exist
            var ourKey = modNPC.GetLocalizationKey("VanillaQoL.SpawnCondition");
            LocalizedText? conditions;
            if (NPCCensus.conditions.TryGetValue(modNPC.NPC.type, out var called)) {
                conditions = called;
            }
            else if (Language.Exists(ourKey)) {
                conditions = Language.GetText(ourKey);
            }
            else {
                conditions = modNPC.GetLocalization("Census.SpawnCondition", () => "Conditions unknown");
            }

            entries.Add((modNPC.NPC.type, conditions));
        }
    }

    // continue vanilla's own row loop with the same locals
    public static void drawExtraRows(ref int num, ref string text, ref int num2, ref int num3,
        ref int num4, ref int num5) {
        var mH = (int)GlobalHooks.mHField.GetValue(null)!;

        if (QoLConfig.Instance.betterCensus) {
            var present = new HashSet<int>();
            foreach (var npc in Main.ActiveNPCs) {
                if (npc.townNPC) {
                    present.Add(npc.type);
                }
            }

            // arrival order: whoever's next, then the ready ones, then the rest
            List<(int type, LocalizedText conditions)> shown = [];
            List<(int type, LocalizedText conditions)> waiting = [];
            foreach (var entry in entries) {
                if (present.Contains(entry.type)) {
                    continue;
                }

                if (synced && WorldGen.prioritizedTownNPCType == entry.type) {
                    shown.Insert(0, entry);
                }
                else if (synced && Main.townNPCCanSpawn[entry.type]) {
                    shown.Add(entry);
                }
                else {
                    waiting.Add(entry);
                }
            }

            shown.AddRange(waiting);

            foreach (var (type, conditions) in shown) {
                var num7 = Main.screenWidth - 64 - 28 + num3;
                var num8 = (int)(174 + mH + num * 56 * Main.inventoryScale) + num2;
                if (num8 > Main.screenHeight - 80) {
                    num3 -= 48;
                    num2 -= num8 - (174 + mH);
                    num7 = Main.screenWidth - 64 - 28 + num3;
                    num8 = (int)(174 + mH + num * 56 * Main.inventoryScale) + num2;
                    UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn = num5;
                    // vanilla hides the pvp icons at four columns; our fixPvPUI shifts them instead
                    if (num3 <= -144 && !QoLConfig.Instance.fixPvPUI) {
                        hidePVPIconsField.SetValue(null, true);
                    }

                    num5 = 0;
                }

                if (Main.mouseX >= num7 &&
                    Main.mouseX <= num7 + TextureAssets.InventoryBack.Width() * Main.inventoryScale &&
                    Main.mouseY >= num8 &&
                    Main.mouseY <= num8 + TextureAssets.InventoryBack.Height() * Main.inventoryScale) {
                    Main.mouseText = true;
                    var name = Lang.GetNPCNameValue(type);
                    if (synced && WorldGen.prioritizedTownNPCType == type) {
                        text = name + "\n" + Language.GetTextValue("Mods.VanillaQoL.Census.next");
                    }
                    else if (synced && Main.townNPCCanSpawn[type]) {
                        text = name + "\n" + Language.GetTextValue("Mods.VanillaQoL.Census.ready");
                    }
                    else {
                        // spoiler mode: you get to know who, you don't get to know how
                        text = QoLConfig.Instance.censusSpoilerMode ? name : name + " - " + conditions.Value;
                    }

                    if (!PlayerInput.IgnoreMouseInterface) {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                }

                Main.spriteBatch.Draw(TextureAssets.InventoryBack7.Value, new Vector2(num7, num8),
                    null, Main.inventoryBack, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);

                var headIndex = NPC.TypeToDefaultHeadIndex(type);
                var head = TextureAssets.NpcHead[headIndex].Value;
                var scale = 1f;
                float largest = Math.Max(head.Width, head.Height);
                if (largest > 36f) {
                    scale = 36f / largest;
                }

                var centre = new Vector2(num7 + 26f * Main.inventoryScale, num8 + 26f * Main.inventoryScale);
                Main.spriteBatch.Draw(head, centre, null, Color.White, 0f,
                    new Vector2(head.Width / 2f, head.Height / 2f), scale, SpriteEffects.None, 0f);

                var marker = !synced ? "?" : Main.townNPCCanSpawn[type] ? "✓" : "X";
                var markerColour = !synced ? Color.MediumPurple :
                    Main.townNPCCanSpawn[type] ? Color.LightGreen : Color.LightSalmon;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value,
                    marker, centre + new Vector2(6f, 6f), markerColour, 0f, Vector2.Zero, new Vector2(0.7f));

                num++;
                num4++;
                num5++;
            }

            // vanilla only has icons-per-column when a column overflows, which leaves it at 1
            // for a single column and made shiftButtons guess, we should actually make it truthful...
            if (num5 > UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) {
                UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn = num5;
            }
        }

        // the happiness readout for whoever's already moved in (don't spoiler it!)
        if (QoLConfig.Instance.npcHappinessOnHover && text != "") {
            var hovered = UILinkPointNavigator.Shortcuts.NPCS_LastHovered;
            if (hovered >= 0 && hovered < Main.maxNPCs) {
                var npc = Main.npc[hovered];
                if (npc.active && npc.townNPC) {
                    if (hovered != lastHoverNPC || Main.GameUpdateCount - lastHoverTick > 30) {
                        var settings = Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, npc);
                        happiness = settings.HappinessReport == ""
                            ? ""
                            : Language.GetTextValue("Mods.VanillaQoL.Census.prices",
                                  (int)Math.Round(settings.PriceAdjustment * 100)) + "\n" +
                              settings.HappinessReport;
                        lastHoverNPC = hovered;
                        lastHoverTick = Main.GameUpdateCount;
                    }

                    if (happiness != "") {
                        text += "\n" + happiness;
                    }
                }
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        if (!QoLConfig.Instance.npcLocatingArrow || !Main.playerInventory || Main.EquipPage != 1) {
            return;
        }

        var idx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
        if (idx == -1) {
            return;
        }

        layers.Insert(idx, new LegacyGameInterfaceLayer("VanillaQoL: NPC Locating Arrow", drawLocatingArrow));
    }

    private static bool drawLocatingArrow() {
        var hovered = UILinkPointNavigator.Shortcuts.NPCS_LastHovered;
        if (hovered <= -1 || hovered >= Main.maxNPCs) {
            return true;
        }

        var npc = Main.npc[hovered];
        var headIndex = NPC.TypeToDefaultHeadIndex(npc.type);
        var from = Main.LocalPlayer.Center + new Vector2(0f, Main.LocalPlayer.gfxOffY);
        var delta = npc.Center - from;
        var distance = delta.Length();
        if (!npc.active || headIndex <= -1 || distance <= 40f) {
            return true;
        }

        var direction = Vector2.Normalize(delta);
        var fade = Math.Min(1f, (distance - 20f) / 70f);
        var tip = from - Main.screenPosition + direction * Math.Min(70f, distance - 20f);
        var cursor = TextureAssets.Cursors[0].Value;
        Main.spriteBatch.Draw(cursor, tip, null, QoLConfig.Instance.npcArrowColour * fade,
            delta.ToRotation() + MathF.PI * 3f / 4f, cursor.Size() / 2f, 1.5f, SpriteEffects.None, 0f);

        var head = TextureAssets.NpcHead[headIndex].Value;
        Main.spriteBatch.Draw(head, tip - direction * 20f, null, Color.White * fade, 0f, head.Size() / 2f, 1f,
            npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        return true;
    }

    private static void injectRows(ILContext il) {
        var ilCursor = new ILCursor(il);

        int numIdx = -1, textIdx = -1, num2Idx = -1, num3Idx = -1, num4Idx = -1, num5Idx = -1;

        // int num = 0; string text = ""; int num2 = 0; int num3 = 0; at the top of the method
        if (!ilCursor.TryGotoNext(
                i => i.MatchLdcI4(0), i => i.MatchStloc(out numIdx),
                i => i.MatchLdstr(""), i => i.MatchStloc(out textIdx),
                i => i.MatchLdcI4(0), i => i.MatchStloc(out num2Idx),
                i => i.MatchLdcI4(0), i => i.MatchStloc(out num3Idx))) {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate the counter initialisation at DrawNPCHousesInUI (num/text/num2/num3)");
            return;
        }

        // the per-column count feeds NPCS_IconsPerColumn inside the column break
        ilCursor.Index = 0;
        if (!ilCursor.TryGotoNext(
                i => i.MatchLdloc(out num5Idx),
                i => i.MatchStsfld(out var perColumn) && perColumn.Name == "NPCS_IconsPerColumn")) {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate the icons-per-column stfld at DrawNPCHousesInUI (NPCS_IconsPerColumn)");
            return;
        }

        // slot in right before vanilla publishes the total, so our rows count towards it
        ilCursor.Index = 0;
        if (!ilCursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdloc(out num4Idx),
                i => i.MatchStsfld(out var total) && total.Name == "NPCS_IconsTotal")) {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate the icon total stfld at DrawNPCHousesInUI (NPCS_IconsTotal)");
            return;
        }

        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[numIdx]);
        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[textIdx]);
        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[num2Idx]);
        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[num3Idx]);
        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[num4Idx]);
        ilCursor.Emit(OpCodes.Ldloca, il.Body.Variables[num5Idx]);
        ilCursor.Emit<NPCCensus>(OpCodes.Call, "drawExtraRows");
        ILEdits.updateOffsets(ilCursor);
    }

    private static void canSpawnPatch(ILContext il) {
        var ilCursor = new ILCursor(il);
        if (!ilCursor.TryGotoNext(MoveType.After,
                i => i.MatchCall(typeof(NPCLoader), "CanTownNPCSpawn"))) {
            VanillaQoL.instance.Logger.Warn(
                "Failed to locate the town NPC spawn check at UpdateTime_SpawnTownNPCs (NPCLoader.CanTownNPCSpawn)");
            return;
        }

        ilCursor.Emit<NPCCensus>(OpCodes.Call, "afterTownNPCSpawnCheck");
        ILEdits.updateOffsets(ilCursor);
    }

    public static void afterTownNPCSpawnCheck() {
        synced = true;
        if (Main.netMode == NetmodeID.Server) {
            var packet = VanillaQoL.instance.GetPacket();
            packet.Write(spawnPacketID);
            writeCanSpawn(packet);
            packet.Send();
        }
    }

    public static void handleCanSpawnPacket(BinaryReader reader) {
        WorldGen.prioritizedTownNPCType = reader.ReadInt32();
        var length = reader.ReadInt32();
        if (length != Main.townNPCCanSpawn.Length) {
            VanillaQoL.instance.Logger.Warn(
                $"townNPCCanSpawn length mismatch, got {length}, have {Main.townNPCCanSpawn.Length}?");
        }

        for (var i = 0; i < length; i += 8) {
            BitsByte bits = reader.ReadByte();
            for (var j = 0; j < 8 && i + j < Main.townNPCCanSpawn.Length; j++) {
                if (i + j < Main.townNPCCanSpawn.Length) {
                    Main.townNPCCanSpawn[i + j] = bits[j];
                }
            }
        }

        synced = true;
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(synced);
        if (synced) {
            writeCanSpawn(writer);
        }
    }

    public override void NetReceive(BinaryReader reader) {
        if (reader.ReadBoolean()) {
            handleCanSpawnPacket(reader);
        }
    }

    private static void writeCanSpawn(BinaryWriter writer) {
        writer.Write(WorldGen.prioritizedTownNPCType);
        writer.Write(Main.townNPCCanSpawn.Length);
        for (var i = 0; i < Main.townNPCCanSpawn.Length; i += 8) {
            var bits = default(BitsByte);
            for (var j = 0; j < 8 && i + j < Main.townNPCCanSpawn.Length; j++) {
                bits[j] = Main.townNPCCanSpawn[i + j];
            }

            writer.Write(bits);
        }
    }
}