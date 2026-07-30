using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            if (QoLConfig.Instance.mapSharing && Main.mapEnabled) {
                QoLSharedMapSystem.instance.scheduleJoinSync();
            }
        }
    }

    public override void ProcessTriggers(TriggersSet triggersSet) {
        if (QoLConfig.Instance.mapSharing && Main.netMode == NetmodeID.MultiplayerClient && Main.mapEnabled &&
            QoLSharedMapSystem.shareKeybind.JustPressed) {
            if (QoLSharedMapSystem.isForeverAlone()) {
                Main.NewText("Nobody else is here...");
            }
            else {
                QoLSharedMapSystem.instance.requestMaps();
                Main.NewText("Requested everyone's maps.");
            }
        }
    }
}

// Many thanks to Lans!
public class QoLSharedMapSystem : ModSystem {
    public static QoLSharedMapSystem instance = null!;

    public static ModKeybind shareKeybind { get; private set; } = null!;

    // tiles revealed since the last flush
    private readonly ConcurrentQueue<Point16> updates = new();
    private readonly HashSet<Point16> seen = new();
    private int flushCounter;

    // outgoing full-map transfers
    private readonly Dictionary<byte, MapTransfer> transfers = new();
    private int joinSyncDelay = -1;

    private const int updateBatching = 2000;
    private const int maxUpdateIntervalTicks = 60;
    private const int packetSize = 1024;

    private const int border = WorldMap.BlackEdgeWidth; // nothing to share there
    private const int chunkTiles = 6000;
    private const int chunkScanBudget = 250000;
    private const int sendWindow = 4;
    private const int transferTimeoutTicks = 600;
    private const int joinSyncGraceTicks = 120;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.mapSharing;
    }

    public override void Load() {
        instance = this;
        IL_WorldMap.UpdateLighting += updateMapPatch;

        shareKeybind = KeybindLoader.RegisterKeybind(Mod, "ShareMap", "P");
    }

    public override void Unload() {
        IL_WorldMap.UpdateLighting -= updateMapPatch;
        instance = null!;
        shareKeybind = null!;
    }

    public override void OnWorldUnload() {
        updates.Clear();
        transfers.Clear();
        joinSyncDelay = -1;
        flushCounter = 0;
    }

    public static bool isForeverAlone() {
        var n = 0;
        for (var i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active) {
                n++;
            }
        }

        return n == 1;
    }

    public void scheduleJoinSync() {
        joinSyncDelay = joinSyncGraceTicks;
    }

    // [51 7 - 51 19]
    // IL_004a: ldc.i4.1
    // IL_004b: ret
    // we only want to inject if it actually changed the thing, no need to if it's the same / less light
    public static void updateMapPatch(ILContext il) {
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
            instance.updates.Enqueue(new Point16(x, y));
        }
    }

    public override void PostUpdateEverything() {
        if (Main.netMode != NetmodeID.MultiplayerClient || !Main.mapEnabled) {
            return;
        }

        updateTransfers();

        // the map file streams in after entering the world, don't aask for maps until it has settled in
        if (joinSyncDelay > 0 && !Main.loadMap && !Main.loadMapLock) {
            joinSyncDelay--;
            if (joinSyncDelay == 0 && !isForeverAlone()) {
                requestMaps();
            }
        }

        if (isForeverAlone()) {
            // nobody to talk to, drop the backlog - joining players get the full map anyway
            flushCounter = 0;
            updates.Clear();
            return;
        }

        flushCounter++;
        if (updates.Count > updateBatching || flushCounter > maxUpdateIntervalTicks) {
            flushUpdates();
            flushCounter = 0;
        }
    }

    // ===== delta stream =====

    private void flushUpdates() {
        // the lighting engine re-reveals the same tiles constantly, dedup them before sending
        // todo we could optimise this
        seen.Clear();
        while (updates.TryDequeue(out var p)) {
            seen.Add(p);
        }

        if (seen.Count == 0) {
            return;
        }

        var map = Main.Map;
        var remaining = seen.Count;
        var it = seen.GetEnumerator();
        // todo simd?
        while (remaining > 0) {
            var count = Math.Min(packetSize, remaining);
            byte[] data;
            using (var stream = new MemoryStream()) {
                using (var writer = new BinaryWriter(stream)) {
                    for (var i = 0; i < count; i++) {
                        it.MoveNext();
                        var p = it.Current;
                        writer.Write(p.X);
                        writer.Write(p.Y);
                        var tile = map[p.X, p.Y];
                        writer.Write(tile.Type);
                        writer.Write(tile.Light);
                        writer.Write(tile.Color);
                    }
                }

                data = compress(stream.ToArray());
            }

            var packet = Mod.GetPacket(data.Length + 8);
            packet.Write((byte)SharedMapMessages.MapUpdate);
            packet.Write((byte)Main.myPlayer);
            packet.Write((short)data.Length);
            packet.Write((short)count);
            packet.Write(data);
            packet.Send();
            remaining -= count;
        }
    }

    // ===== full sync (pull model: joiner asks, everyone streams their lit tiles back) =====

    public void requestMaps() {
        var packet = Mod.GetPacket();
        packet.Write((byte)SharedMapMessages.MapRequest);
        packet.Write((byte)Main.myPlayer);
        packet.Send();
        Mod.Logger.Info("Requested map sync");
    }

    private void startTransfer(byte target) {
        var t = new MapTransfer { target = target, x = border, y = border };
        transfers[target] = t;
        for (var i = 0; i < sendWindow && !t.done; i++) {
            sendChunk(t);
        }

        Mod.Logger.Info($"Started map transfer to {Main.player[target].name}");
    }

    private void sendChunk(MapTransfer t) {
        var map = Main.Map;
        var maxX = map.MaxWidth - border;
        var maxY = map.MaxHeight - border;

        byte[] data;
        using (var stream = new MemoryStream()) {
            using (var writer = new BinaryWriter(stream)) {
                // todo simd?
                var count = 0;
                var cells = 0;
                while (t.x < maxX && count < chunkTiles && cells < chunkScanBudget) {
                    var tile = map[t.x, t.y];
                    if (tile.Light > 0) {
                        writer.Write((short)t.x);
                        writer.Write((short)t.y);
                        writer.Write(tile.Type);
                        writer.Write(tile.Light);
                        writer.Write(tile.Color);
                        count++;
                    }

                    cells++;
                    t.y++;
                    if (t.y >= maxY) {
                        t.y = border;
                        t.x++;
                    }
                }
            }

            data = compress(stream.ToArray());
        }

        t.done = t.x >= maxX;

        var packet = Mod.GetPacket(data.Length + 16);
        packet.Write((byte)SharedMapMessages.TransferChunk);
        packet.Write(t.target);
        packet.Write((byte)Main.myPlayer);
        packet.Write(t.index);
        packet.Write(data.Length);
        packet.Write(data);
        packet.Send();
        t.index++;
    }

    private void updateTransfers() {
        if (transfers.Count == 0) {
            return;
        }

        List<byte>? dead = null;
        foreach (var (target, t) in transfers) {
            t.idle++;
            if (t.idle > transferTimeoutTicks || !Main.player[target].active) {
                (dead ??= []).Add(target);
            }
        }

        if (dead != null) {
            foreach (var target in dead) {
                transfers.Remove(target);
                Mod.Logger.Info($"Dropped map transfer to player {target}, we got ghosted :(");
            }
        }
    }

    // ===== packets =====

    // the type byte is read upstream in VanillaQoL.HandlePacket these days, so census packets
    // can also share the channel
    // todo implement some kind of structured packet handling? or we can just keep copypasting terraria style lol
    public void HandlePacket(SharedMapMessages msgType, BinaryReader reader, int whoAmI) {
        var routing = reader.ReadByte();
        switch (msgType) {
            case SharedMapMessages.MapUpdate: {
                handleMapUpdate(reader, routing, whoAmI);
                break;
            }
            case SharedMapMessages.MapRequest: {
                handleMapRequest(routing, whoAmI);
                break;
            }
            case SharedMapMessages.TransferChunk: {
                handleTransferChunk(reader, routing, whoAmI);
                break;
            }
            case SharedMapMessages.TransferAck: {
                handleTransferAck(reader, routing, whoAmI);
                break;
            }
            default:
                throw new ArgumentException("VanillaQoL encountered an unhandled packet, there are problems.");
        }
    }

    private void handleMapUpdate(BinaryReader reader, byte src, int whoAmI) {
        var length = reader.ReadInt16();
        var count = reader.ReadInt16();
        var data = reader.ReadBytes(length);

        if (Main.netMode == NetmodeID.Server) {
            // no spoofing on this channel!
            var packet = Mod.GetPacket(length + 8);
            packet.Write((byte)SharedMapMessages.MapUpdate);
            packet.Write((byte)whoAmI);
            packet.Write(length);
            packet.Write(count);
            packet.Write(data);
            sendToOthers(packet, whoAmI);
            return;
        }

        var map = Main.Map;
        using var ms = new MemoryStream(decompress(data));
        using var rd = new BinaryReader(ms);
        // todo simd?
        for (var i = 0; i < count; i++) {
            var x = rd.ReadInt16();
            var y = rd.ReadInt16();
            var type = rd.ReadUInt16();
            var light = rd.ReadByte();
            var colour = rd.ReadByte();
            if (x < 0 || y < 0 || x >= map.MaxWidth || y >= map.MaxHeight) {
                continue;
            }

            // the sender just watched this tile change so trust the type but max() the light
            var tile = map[x, y];
            tile.Type = type;
            tile.Light = Math.Max(light, tile.Light);
            tile.Color = colour;
            tile.IsChanged = true;
            map.SetTile(x, y, ref tile);
            updateMapTile(x, y);
        }
    }

    private void handleMapRequest(byte src, int whoAmI) {
        if (Main.netMode == NetmodeID.Server) {
            var packet = Mod.GetPacket();
            packet.Write((byte)SharedMapMessages.MapRequest);
            packet.Write((byte)whoAmI);
            sendToOthers(packet, whoAmI);
            return;
        }

        if (src == Main.myPlayer || !Main.mapEnabled) {
            return;
        }

        startTransfer(src);
    }

    private void handleTransferChunk(BinaryReader reader, byte target, int whoAmI) {
        var src = reader.ReadByte();
        var index = reader.ReadInt32();
        var length = reader.ReadInt32();
        var data = reader.ReadBytes(length);

        if (Main.netMode == NetmodeID.Server) {
            if (target >= Main.maxPlayers || !Main.player[target].active) {
                return;
            }

            var packet = Mod.GetPacket(length + 16);
            packet.Write((byte)SharedMapMessages.TransferChunk);
            packet.Write(target);
            packet.Write((byte)whoAmI);
            packet.Write(index);
            packet.Write(length);
            packet.Write(data);
            packet.Send(target);
            return;
        }

        if (index == 0) {
            Main.NewText($"{Main.player[src].name} is beaming their map over.");
        }

        var map = Main.Map;
        // todo simd?
        using (var ms = new MemoryStream(decompress(data))) {
            using var rd = new BinaryReader(ms);
            while (ms.Position < ms.Length) {
                var x = rd.ReadInt16();
                var y = rd.ReadInt16();
                var type = rd.ReadUInt16();
                var light = rd.ReadByte();
                var colour = rd.ReadByte();
                if (x < 0 || y < 0 || x >= map.MaxWidth || y >= map.MaxHeight) {
                    continue;
                }

                // bulk history, only fill in what we know less about. we NEVER wipe
                var tile = map[x, y];
                if (light > tile.Light) {
                    tile.Type = type;
                    tile.Light = light;
                    tile.Color = colour;
                    tile.IsChanged = true;
                    map.SetTile(x, y, ref tile);
                    updateMapTile(x, y);
                }
            }
        }

        // ack it so the next chunk comes
        var ack = Mod.GetPacket();
        ack.Write((byte)SharedMapMessages.TransferAck);
        ack.Write(src);
        ack.Write((byte)Main.myPlayer);
        ack.Write(index);
        ack.Send();
    }

    private void handleTransferAck(BinaryReader reader, byte target, int whoAmI) {
        var src = reader.ReadByte();
        var index = reader.ReadInt32();

        if (Main.netMode == NetmodeID.Server) {
            if (target >= Main.maxPlayers || !Main.player[target].active) {
                return;
            }

            var packet = Mod.GetPacket();
            packet.Write((byte)SharedMapMessages.TransferAck);
            packet.Write(target);
            packet.Write((byte)whoAmI);
            packet.Write(index);
            packet.Send(target);
            return;
        }

        if (!transfers.TryGetValue(src, out var t)) {
            return;
        }

        t.idle = 0;
        if (t.done) {
            transfers.Remove(src);
            Mod.Logger.Info($"Finished map transfer to {Main.player[src].name}, {t.index} chunks");
        }
        else {
            sendChunk(t);
        }
    }

    // on the server. relays to everyone unless teamOnly says otherwise - team 0 is not a team!
    private void sendToOthers(ModPacket packet, int src) {
        var teamOnly = QoLConfig.Instance.mapSharingTeamOnly;
        var team = Main.player[src].team;
        for (var i = 0; i < Main.maxPlayers; i++) {
            if (i == src || !Main.player[i].active) {
                continue;
            }

            if (teamOnly && (team == 0 || Main.player[i].team != team)) {
                continue;
            }

            packet.Send(i);
        }
    }

    private static byte[] compress(byte[] data) {
        using var memoryStream = new MemoryStream();
        using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress)) {
            deflateStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    private static byte[] decompress(byte[] data) {
        using var inStream = new MemoryStream(data);
        using var outStream = new MemoryStream();
        using (var deflateStream = new DeflateStream(inStream, CompressionMode.Decompress)) {
            deflateStream.CopyTo(outStream);
        }

        return outStream.ToArray();
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

    // todo should be a ref struct?
    private class MapTransfer {
        public byte target;
        public int x;
        public int y;
        public int index;
        public bool done;
        public int idle;
    }

    public enum SharedMapMessages : byte {
        // incremental map updates, client -> server -> everyone else
        MapUpdate,

        // "send me your maps", client -> server -> everyone else
        MapRequest,

        // one chunk of somebody's tiles, responder -> server -> requester
        TransferChunk,

        // ack a chunk so the responder sends the next one, requester -> server -> responder
        TransferAck
    }
}
