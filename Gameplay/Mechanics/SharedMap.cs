using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class QoLPlayer : ModPlayer {
    public override void OnEnterWorld() {
        // only on client
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            if (QoLConfig.Instance.autoJoinTeam) {
                var team = QoLConfig.Instance.teamToAutoJoin;
                Main.LocalPlayer.team = (int)team;
                NetMessage.SendData(MessageID.PlayerTeam, number: Main.myPlayer);
            }
        }
    }

    public override void ProcessTriggers(TriggersSet triggersSet) {
        if (QoLConfig.Instance.mapSharingTESTING && Main.netMode == NetmodeID.MultiplayerClient && Main.mapEnabled &&
            QoLSharedMapSystem.shareKeybind.JustPressed) {
            QoLSharedMapSystem.instance.sendSyncRequestPacket();
            Main.NewText("Requested map sync.");
        }
    }
}

// Many thanks to Lans!
// todo only send to same team
public class QoLSharedMapSystem : ModSystem {
    ConcurrentQueue<Point16> updates = new();
    public static QoLSharedMapSystem instance = null!;
    private int counter;
    private int counter2;

    public static ModKeybind shareKeybind { get; private set; } = null!;

    public const int updateBatching = 2000;
    private const int maxUpdateIntervalTicks = 60;
    private const int packetSize = 1024;

    // how often to checksum the entire map and resync if outdated - default 60secs
    private const int fullChecksumIntervalTicks = 60 * 60;

    private const bool isPeriodicIntervalEnabled = true;

    // this many tiles can be wrong before a full resend happens
    // 8100 tiles are on the screen on 1920x1080....
    private const int lightTolerance = 15000;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.mapSharingTESTING;
    }


    public override void Load() {
        instance = this;
        // we apply to two places, UpdateLighting and Update.
        IL_WorldMap.UpdateLighting += updateMapPatch2;
        //IL_WorldMap.Update += updateMapPatch;

        // we don't *actually* need to sync straight up on world load.... it seems to corrupt map files, probably due to the world being empty
        //IL_WorldMap.Load += joinWorldPatch;

        shareKeybind = KeybindLoader.RegisterKeybind(Mod, "ShareMap", "P");
    }

    public override void Unload() {
        IL_WorldMap.UpdateLighting -= updateMapPatch2;
        instance = null!;
        shareKeybind = null!;
    }

    public static bool isForeverAlone() {
        return Main.player.Count(p => p.active) == 1;
    }

    /*private void joinWorldPatch(ILContext il) {
        var c = new ILCursor(il);
        var emitted = false;
        while (c.TryGotoNext(MoveType.Before, i => i.MatchRet())) {ene
            c.Emit<QoLSharedMapSystem>(OpCodes.Call, "joinWorld");
            // increment so no infinite loop!
            c.Index++;
            emitted = true;
            //VanillaQoL.instance.Logger.Info("Patched WorldMap.Load!");
        }

        if (!emitted) {
            //VanillaQoL.instance.Logger.Warn("Couldn't match return in WorldMap.Load");
        }
    }

    public static void joinWorld() {
        // sync on load
        if (Main.netMode == NetmodeID.MultiplayerClient && Main.mapEnabled && !isForeverAlone()) {
            instance.sendSyncRequestPacket();
        }
    }*/

    public static void updateMapPatch(ILContext il) {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_1);
        c.Emit(OpCodes.Ldarg_2);
        c.Emit(OpCodes.Ldarg_3);
        c.Emit<QoLSharedMapSystem>(OpCodes.Call, "onUpdate");
    }

    // [51 7 - 51 19]
    // IL_004a: ldc.i4.1
    // IL_004b: ret
    // we only want to inject if it actually changed the thing, no need to if it's the same / less light
    public static void updateMapPatch2(ILContext il) {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1), i => i.MatchRet())) {
            c.Emit(OpCodes.Ldarg_1);
            c.Emit(OpCodes.Ldarg_2);
            c.Emit(OpCodes.Ldarg_3);
            c.Emit<QoLSharedMapSystem>(OpCodes.Call, "onUpdate");
        }
        else {
            VanillaQoL.instance.Logger.Warn("Couldn't match return in WorldMap.UpdateLighting");
        }
    }

    public static void onUpdate(int x, int y, byte light) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            var newTile = new Point16(x, y);
            instance.updates.Enqueue(newTile);
        }
    }

    // on client
    public void sendSyncRequestPacket() {
        var packet = Mod.GetPacket();
        var checksum = calculateLightedTiles(Main.Map);
        packet.Write((byte)SharedMapMessages.SyncRequest);
        packet.Write((byte)Main.myPlayer);
        packet.Write(checksum);
        packet.Send();
        Mod.Logger.Info("Sent sync request packet");
    }

    public int sendSyncPacket(MapTile[] data, int index, int targetPlayer) {
        var originalLength = 2 + 8 + data.Length * 4;
        var compressedData = compress(convertToBytes(data));
        var compressedLength = compressedData.Length;
        var packet = Mod.GetPacket(originalLength);
        packet.Write((byte)SharedMapMessages.Sync);
        packet.Write((byte)targetPlayer);
        packet.Write(index);
        packet.Write(compressedLength);
        packet.Write(compressedData);

        var length = 2 + 8 + compressedLength;
        packet.Send();
        return length;
    }

    public void sendSyncPackets(int targetPlayer) {
        var map = Main.Map;
        int size = map.MaxWidth;
        MapTile[] data = new MapTile[size];
        var sum = 0;
        var count = 0;
        // send it in chunks
        for (int y = 0; y < map.MaxHeight; y++) {
            var index = y;
            for (int x = 0; x < map.MaxWidth; x++) {
                data[x] = map[x, y];
            }

            sum += sendSyncPacket(data, index, targetPlayer);
            count++;
        }

        Mod.Logger.Info($"Sent {count} sync packets, size {sum}");
    }

    private byte[] convertToBytes(MapTile[] data) {
        using (var stream = new MemoryStream()) {
            using (var writer = new BinaryWriter(stream)) {
                for (var i = 0; i < Main.Map.MaxWidth; i++) {
                    var tile = data[i];
                    writer.Write(tile.Type);
                    writer.Write(tile.Light);
                    // we don't need to send IsChanged, we set it to true anyway
                    writer.Write(tile.Color);
                }
            }

            return stream.ToArray();
        }
    }

    private byte[] compress(byte[] data) {
        using (MemoryStream memoryStream = new MemoryStream()) {
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress)) {
                deflateStream.Write(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }
    }

    private byte[] decompress(byte[] data) {
        using (MemoryStream inStream = new MemoryStream(data)) {
            using (MemoryStream outStream = new MemoryStream()) {
                using (DeflateStream deflateStream = new DeflateStream(inStream, CompressionMode.Decompress)) {
                    deflateStream.CopyTo(outStream);
                }

                return outStream.ToArray();
            }
        }
    }

    private void relaySyncPacket(BinaryReader reader, byte whichPlayer) {
        // just relay the packet
        var startIndex = reader.ReadInt32();
        var compressedLength = reader.ReadInt32();
        var data = reader.ReadBytes(compressedLength);
        var packet = Mod.GetPacket();
        packet.Write((byte)SharedMapMessages.Sync);
        packet.Write(whichPlayer);
        packet.Write(startIndex);
        packet.Write(compressedLength);
        packet.Write(data);
        // don't send it back to the original client
        packet.Send(whichPlayer);
    }

    public static long calculateLightedTiles(WorldMap map) {
        long v = 0;
        for (int x = 0; x < map.MaxWidth; x++) {
            for (int y = 0; y < map.MaxHeight; y++) {
                var t = map[x, y].Light;
                v += t;
            }
        }

        return v;
    }

    public override void PostUpdateEverything() {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            return;
        }

        if (isForeverAlone()) {
            counter = 0;
            counter2 = 0;
            return;
        }

        counter++;
        // if too many have accumulated, send
        if (instance.updates.Count > updateBatching || instance.counter > maxUpdateIntervalTicks) {
            _ = Task.Run(() => {
                //var totalUpdates = 0;
                //var size = 0;
                while (!updates.IsEmpty) {
                    var maxUpdates = Math.Min(packetSize, updates.Count);
                    // size is maxUpdates * 5 (2 shorts+byte) + 4 bytes for misc shit
                    // this can't be more than a short obviously
                    var length = (short)(2 + 2 + maxUpdates * 8);
                    var packet = Mod.GetPacket(length);
                    packet.Write((byte)SharedMapMessages.MapUpdate);
                    packet.Write((byte)Main.myPlayer);
                    byte[] data;
                    using (var stream = new MemoryStream()) {
                        using (var writer = new BinaryWriter(stream)) {
                            for (var i = 0; i < maxUpdates; i++) {
                                updates.TryDequeue(out var t);
                                writer.Write(t.X);
                                writer.Write(t.Y);
                                var mapTile = Main.Map[t.X, t.Y];
                                writer.Write(mapTile.Type);
                                writer.Write(mapTile.Light);
                                writer.Write(mapTile.Color);
                            }
                        }

                        data = stream.ToArray();
                        data = compress(data);
                    }

                    packet.Write((short)data.Length);
                    packet.Write((short)maxUpdates);
                    packet.Write(data);
                    packet.Send();
                    //totalUpdates += maxUpdates;
                    //size += length;
                }

                //Mod.Logger.Debug($"Sent {totalUpdates} map updates (size {size}), {updates.Count} remaining");

                instance.counter = 0;
            });
        }

        counter2++;
        if (isPeriodicIntervalEnabled && instance.counter2 > fullChecksumIntervalTicks) {
            sendSyncRequestPacket();
            instance.counter2 = 0;
        }
    }

    public void HandlePacket(BinaryReader reader, int whoAmI) {
        SharedMapMessages msgType = (SharedMapMessages)reader.ReadByte();
        byte whichPlayer = reader.ReadByte();
        switch (msgType) {
            case SharedMapMessages.MapUpdate: {
                handleMapUpdate(reader, whichPlayer);
                break;
            }

            case SharedMapMessages.SyncRequest: {
                handleSyncRequest(reader, whichPlayer);
                break;
            }
            case SharedMapMessages.Sync: {
                handleSyncPacket(reader, whichPlayer);
                break;
            }
            default:
                throw new ArgumentException("VanillaQoL encountered an unhandled packet, there are problems.");
        }
    }

    private void handleMapUpdate(BinaryReader reader, byte whichPlayer) {
        // running on server
        if (Main.netMode == NetmodeID.Server) {
            var length = reader.ReadInt16();
            var maxUpdates = reader.ReadInt16();

            var packet = Mod.GetPacket(length);
            var data = reader.ReadBytes(length);
            packet.Write((byte)SharedMapMessages.MapUpdate);
            packet.Write(whichPlayer);
            packet.Write(length);
            packet.Write(maxUpdates);
            packet.Write(data);


            //packet.Send(-1, whichPlayer);
            sendToSameTeam(packet, whichPlayer);
        }
        // running on client
        else {
            short length = reader.ReadInt16();
            short maxUpdates = reader.ReadInt16();
            var compressedData = reader.ReadBytes(length);
            var data = decompress(compressedData);
            using (var ms = new MemoryStream(data)) {
                using (var rd = new BinaryReader(ms)) {
                    for (int i = 0; i < maxUpdates; i++) {
                        var x = rd.ReadInt16();
                        var y = rd.ReadInt16();
                        var t = rd.ReadUInt16();
                        byte l = rd.ReadByte();
                        byte c = rd.ReadByte();

                        var map = Main.Map;

                        var tile = map[x, y];
                        tile.Type = t;
                        tile.Light = Math.Max(l, tile.Light);
                        tile.Color = c;
                        tile.IsChanged = true;
                        //map.SetTile(x, y, ref tile);
                        updateMapTile(x, y);
                    }
                }
            }
        }
    }

    private void sendToSameTeam(ModPacket packet, byte whichPlayer) {
        var team = Main.player[whichPlayer].team;
        for (int i = 0; i < Main.player.Length; i++) {
            if (i != whichPlayer && Main.player[i].team > 0 && Main.player[i].team == team) {
                packet.Send(i);
            }
        }
    }

    private void handleSyncRequest(BinaryReader reader, byte whichPlayer) {
        // running on server
        if (Main.netMode == NetmodeID.Server) {
            var checksum = reader.ReadInt64();
            // just relay the packet
            var packet = Mod.GetPacket();
            packet.Write((byte)SharedMapMessages.SyncRequest);
            packet.Write(whichPlayer);
            packet.Write(checksum);
            // don't send it back to the original client
            sendToSameTeam(packet, whichPlayer);
        }

        // running on client
        else {
            var otherChecksum = reader.ReadInt64();
            var localChecksum = calculateLightedTiles(Main.Map);
            if (Math.Abs(otherChecksum - localChecksum) > lightTolerance * 255) {
                Mod.Logger.Info($"Sent sync packets, {localChecksum} vs {otherChecksum} checksum");
                _ = Task.Run(() => sendSyncPackets(whichPlayer));
                //sendSyncPackets(whichPlayer);
            }
        }
    }

    private void handleSyncPacket(BinaryReader reader, byte whichPlayer) {
        // running on server
        // relay the packet with the data back
        if (Main.netMode == NetmodeID.Server) {
            relaySyncPacket(reader, whichPlayer);
        }

        // running on client
        else {
            // apply light updates
            var index = reader.ReadInt32();
            var compressedLength = reader.ReadInt32();
            var compressedData = reader.ReadBytes(compressedLength);
            var data = decompress(compressedData);

            var map = Main.Map;
            // data contains Map[all, index]
            // there are two passes of updating in Main.DrawToMap
            // one is a global map update but capped at 250000 per frame?
            // another is immediate, capped at 1000 for the exploring and stuff?
            // well the slower one is good for us
            //VanillaQoL.instance.Logger.Warn($"{compressedLength}, {compressedData.Length}, {data.Length}");
            using (var memoryStream = new MemoryStream(data)) {
                using (var bytes = new BinaryReader(memoryStream)) {
                    for (int i = 0; i < map.MaxWidth; i++) {
                        var tile = map[i, index];
                        var type = bytes.ReadUInt16();
                        var light = bytes.ReadByte();
                        var color = bytes.ReadByte();
                        tile.Type = type;
                        tile.Light = Math.Max(light, tile.Light);
                        tile.Color = color;
                        tile.IsChanged = true;
                        map.SetTile(i, index, ref tile);
                    }
                }
            }

            Main.refreshMap = true;
        }
    }

    public static void updateMapTile(int i, int j) {
        if (MapHelper.numUpdateTile < MapHelper.maxUpdateTile - 1) {
            MapHelper.updateTileX[MapHelper.numUpdateTile] = (short)i;
            MapHelper.updateTileY[MapHelper.numUpdateTile] = (short)j;
            MapHelper.numUpdateTile++;
        }
        else {
            Main.refreshMap = true;
        }
    }


    public enum SharedMapMessages : byte {
        // send an incremental update of the map, client -> server -> client
        MapUpdate,

        // request a sync, send checksum client -> server -> client
        SyncRequest,

        // this is what syncrequest sends back
        Sync
    }
}