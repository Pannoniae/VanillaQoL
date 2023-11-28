using System;
using System.Collections.Concurrent;
using System.IO;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.Gameplay;

// Many thanks to Lans!
// todo only send to same team
public class QoLSharedMapSystem : ModSystem {
    ConcurrentQueue<Point16> updates = new();
    public static QoLSharedMapSystem instance;
    private int counter;
    private int counter2;

    public const int updateBatching = 2000;
    private const int maxUpdateIntervalTicks = 60;
    private const int packetSize = 1024;

    // how often to checksum the entire map and resync if outdated - default 10secs
    private const int fullChecksumIntervalTicks = 10 * 60;

    private const bool isPeriodicIntervalEnabled = true;

    // this many tiles can be wrong before a full resend happens
    private const int lightTolerance = 20;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.mapSharing;
    }


    public override void Load() {
        instance = this;
        // we apply to two places, UpdateLighting and Update.
        IL_WorldMap.UpdateLighting += updateMapPatch2;
        IL_WorldMap.Update += updateMapPatch;
        IL_WorldMap.Load += joinWorldPatch;
    }

    public override void Unload() {
        instance = null!;
    }

    private void joinWorldPatch(ILContext il) {
        var c = new ILCursor(il);
        var emitted = false;
        while (c.TryGotoNext(MoveType.Before, i => i.MatchRet())) {
            c.Emit<QoLSharedMapSystem>(OpCodes.Call, "joinWorld");
            // increment so no infinite loop!
            c.Index++;
            emitted = true;
        }

        if (!emitted) {
            VanillaQoL.instance.Logger.Warn("Couldn't match return in WorldMap.Load");
        }
    }

    public static void joinWorld() {
        // sync on load
        if (Main.netMode == NetmodeID.MultiplayerClient && Main.mapEnabled) {
            instance.sendSyncRequestPacket();
        }
    }

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
            MapTile t = Main.Map[x, y];
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

    public void sendSyncPacket(MapTile[] data, int index, int targetPlayer) {
        var length = 2 + 8 + data.Length * 4;
        var packet = Mod.GetPacket(length);
        packet.Write((byte)SharedMapMessages.Sync);
        packet.Write((byte)targetPlayer);
        packet.Write(index);
        packet.Write(length);
        foreach (var tile in data) {
            packet.Write(tile.Type);
            packet.Write(tile.Light);
            // we don't need to send IsChanged, we set it to true anyway
            packet.Write(tile.Color);
        }

        packet.Send();
    }

    public void sendSyncPackets(int targetPlayer) {
        var map = Main.Map;
        int size = map.MaxWidth;
        MapTile[] data = new MapTile[size];
        // send it in chunks
        for (int y = 0; y < map.MaxHeight; y++) {
            var index = y;
            for (int x = 0; x < map.MaxWidth; x++) {
                data[x] = map[x, y];
            }

            sendSyncPacket(data, index, targetPlayer);
        }

        Mod.Logger.Info($"Sent {size} sync packets");
    }

    private void relaySyncPacket(BinaryReader reader, byte whichPlayer) {
        // just relay the packet
        var startIndex = reader.ReadInt32();
        var length = reader.ReadInt32();
        var dataLength = length - 10;
        var data = reader.ReadBytes(dataLength);
        var packet = Mod.GetPacket();
        packet.Write((byte)SharedMapMessages.Sync);
        packet.Write(whichPlayer);
        packet.Write(startIndex);
        packet.Write(length);
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

        counter++;
        // if too many have accumulated, send
        if (instance.updates.Count > updateBatching || instance.counter > maxUpdateIntervalTicks) {
            while (!updates.IsEmpty) {
                var maxUpdates = Math.Min(packetSize, updates.Count);
                // size is maxUpdates * 5 (2 shorts+byte) + 4 bytes for misc shit
                // this can't be more than a short obviously
                var length = (short)(2 + 2 + maxUpdates * 8);
                var packet = Mod.GetPacket(length);
                packet.Write((byte)SharedMapMessages.MapUpdate);
                packet.Write((byte)Main.myPlayer);
                packet.Write(length);

                for (var i = 0; i < maxUpdates; i++) {
                    var succ = updates.TryDequeue(out var t);
                    packet.Write(t.X);
                    packet.Write(t.Y);
                    var mapTile = Main.Map[t.X, t.Y];
                    packet.Write(mapTile.Type);
                    packet.Write(mapTile.Light);
                    packet.Write(mapTile.Color);
                }

                packet.Send();
                Mod.Logger.Info($"Sent {maxUpdates} map updates, {updates.Count} remaining");
            }

            instance.counter = 0;
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
            var updateLength = (length - 4) / 8;

            var packet = Mod.GetPacket(length);
            packet.Write((byte)SharedMapMessages.MapUpdate);
            packet.Write(whichPlayer);
            packet.Write(length);
            for (var i = 0; i < updateLength; i++) {
                var x = reader.ReadInt16();
                var y = reader.ReadInt16();
                var t = reader.ReadUInt16();
                var l = reader.ReadByte();
                var c = reader.ReadByte();
                packet.Write(x);
                packet.Write(y);
                packet.Write(t);
                packet.Write(l);
                packet.Write(c);
            }

            ///packet.Send(-1, whichPlayer);
            sendToSameTeam(packet, whichPlayer);
        }
        // running on client
        else {
            short length = reader.ReadInt16();
            var updateLength = (short)((length - 4) / 8);

            for (int i = 0; i < updateLength; i++) {
                var x = reader.ReadInt16();
                var y = reader.ReadInt16();
                var t = reader.ReadUInt16();
                byte l = reader.ReadByte();
                byte c = reader.ReadByte();

                var map = Main.Map;

                var tile = map[x, y];
                tile.Type = Math.Max(t, tile.Light);
                tile.Light = l;
                tile.Color = c;
                tile.IsChanged = true;
                map.SetTile(x, y, ref tile);
                updateMapTile(x, y);
                Main.updateMap = true;
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
            Mod.Logger.Info($"{localChecksum} vs {otherChecksum} checksum");
            if (Math.Abs(otherChecksum - localChecksum) > lightTolerance * 255) {
                Mod.Logger.Info($"Should have sent sync packets, {localChecksum} vs {otherChecksum} checksum");
                sendSyncPackets(whichPlayer);
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
            var length = reader.ReadInt32();
            var dataLength = length - 10;
            var data = reader.ReadBytes(dataLength);

            var map = Main.Map;
            // data contains Map[all, index]
            // there are two passes of updating in Main.DrawToMap
            // one is a global map update but capped at 250000 per frame?
            // another is immediate, capped at 1000 for the exploring and stuff?
            // well the slower one is good for us
            for (int i = 0; i < map.MaxWidth; i++) {
                var tile = map[i, index];
                var type = BitConverter.ToUInt16(data, i * 4);
                var light = data[i * 4 + 2];
                var color = data[i * 4 + 3];
                tile.Type = type;
                tile.Light = Math.Max(light, tile.Light);
                tile.Color = color;
                tile.IsChanged = true;
                map.SetTile(i, index, ref tile);
            }

            Main.refreshMap = true;
        }
    }

    public static bool updateMapTile(int i, int j) {
        bool result = false;

        result = true;
        if (MapHelper.numUpdateTile < MapHelper.maxUpdateTile - 1) {
            MapHelper.updateTileX[MapHelper.numUpdateTile] = (short)i;
            MapHelper.updateTileY[MapHelper.numUpdateTile] = (short)j;
            MapHelper.numUpdateTile++;
        }
        else {
            Main.refreshMap = true;
        }

        return result;
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