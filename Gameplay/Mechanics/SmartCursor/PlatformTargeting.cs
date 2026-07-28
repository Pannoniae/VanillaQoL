using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace VanillaQoL.Gameplay.Mechanics.SmartCursor;

public class PlatformTargeting {
    // how far the mouse can drift off a line before we stop giving a fuck
    private const int erows = 1;
    private const int ecols = 2;

    // how thick a gap we're willing to cross
    private const int max = 3;

    // the six ways a platform line can point
    private static readonly Point16[] directions = [
        new(1, 0), new(-1, 0), new(1, 1), new(-1, -1), new(1, -1), new(-1, 1)
    ];

    public static void selectTarget(List<Point16> targets, Vector2 mouse, int sx, int sy,
        int ex, int ey, ref int fx, ref int fy) {
        if (targets.Count == 0) {
            return;
        }

        var mx = (int)(mouse.X / 16f);
        var my = (int)(mouse.Y / 16f);

        if (findLineEnd(mx, my, mouse, out var lx, out var ly, out var dx, out var dy)) {
            var cx = lx + dx;
            var cy = ly + dy;
            // vanilla already generated the continue if the position is free and valid
            // so if it's there, we're good
            if (targets.Contains(new Point16(cx, cy))) {
                if (Collision.InTileBounds(cx, cy, sx, sy, ex, ey)) {
                    fx = cx;
                    fy = cy;
                }

                return;
            }

            // the line is blocked. if something actually occupies the next tile we can try
            // one past it if the real placement rules would accept the click.
            var blockTile = Main.tile[cx, cy];
            if (blockTile.HasTile && !Main.tileCut[blockTile.TileType]) {
                for (var i = 2; i <= max + 1; i++) {
                    var x = lx + dx * i;
                    var y = ly + dy * i;
                    if (!WorldGen.InWorld(x, y, 5)) {
                        return;
                    }

                    var tile = Main.tile[x, y];
                    if (tile.HasTile && !Main.tileCut[tile.TileType]) {
                        continue;
                    }

                    // first free tile decides: either it anchors or we suggest nothing.
                    if (canAnchorPlatform(x, y) && Collision.InTileBounds(x, y, sx, sy, ex, ey)) {
                        fx = x;
                        fy = y;
                    }

                    return;
                }
            }

            // nothing to continue on, no suggestion > a shit one
            return;
        }

        nearest(targets, mouse, sx, sy, ex, ey, ref fx, ref fy);
    }

    private static bool findLineEnd(int mouseX, int mouseY, Vector2 mouse, out int lx, out int ly,
        out int dx, out int dy) {
        lx = ly = dx = dy = 0;
        var best = -1f;

        var l = Math.Clamp(mouseX - ecols - 1, 5, Main.maxTilesX - 5);
        var r = Math.Clamp(mouseX + ecols + 1, 5, Main.maxTilesX - 5);
        var t = Math.Clamp(mouseY - erows - 1, 5, Main.maxTilesY - 5);
        var b = Math.Clamp(mouseY + erows + 1, 5, Main.maxTilesY - 5);

        for (var x = l; x <= r; x++) {
            for (var y = t; y <= b; y++) {
                foreach (var point in directions) {
                    if (!isOnLine(x, y, point.X, point.Y) || !isOnLine(x - point.X, y - point.Y, point.X, point.Y)) {
                        continue;
                    }

                    var next = Main.tile[x + point.X, y + point.Y];
                    if (next.HasTile && TileID.Sets.Platforms[next.TileType]) {
                        continue;
                    }

                    var cx = x + point.X;
                    var cy = y + point.Y;
                    if (Math.Abs(cy - mouseY) > erows || Math.Abs(cx - mouseX) > ecols + 1) {
                        continue;
                    }

                    var dist = Vector2.Distance(new Vector2(cx, cy) * 16f + Vector2.One * 8f, mouse);
                    if (best == -1f || dist < best) {
                        best = dist;
                        lx = x;
                        ly = y;
                        dx = point.X;
                        dy = point.Y;
                    }
                }
            }
        }

        return best != -1f;
    }

    private static bool isOnLine(int x, int y, int dx, int dy) {
        var tile = Main.tile[x, y];
        if (!tile.HasTile || !TileID.Sets.Platforms[tile.TileType]) {
            return false;
        }

        if (dy == 0) {
            return tile.Slope == SlopeType.Solid;
        }

        return tile.Slope == (dx == dy ? SlopeType.SlopeDownLeft : SlopeType.SlopeDownRight);
    }

    // this is the real placement rule for platforms instead of a heuristic so we're not going to suggest a placement that would be rejected by the game...
    private static bool canAnchorPlatform(int x, int y) {
        return isAnchors(Main.tile[x - 1, y]) || isAnchors(Main.tile[x + 1, y]) ||
               isAnchors(Main.tile[x, y - 1]) || isAnchors(Main.tile[x, y + 1]) ||
               Main.tile[x, y].WallType != WallID.None;
    }

    private static bool isAnchors(Tile tile) {
        if (tile.WallType != WallID.None) {
            return true;
        }

        return tile.HasTile && (Main.tileSolid[tile.TileType] || TileID.Sets.IsBeam[tile.TileType] ||
                                Main.tileRope[tile.TileType] || tile.TileType == TileID.MinecartTrack);
    }

    // copied from vanilla
    private static void nearest(List<Point16> targets, Vector2 mouse, int sx, int sy,
        int ex, int ey, ref int fx, ref int fy) {
        var best = -1f;
        var target = targets[0];
        for (var i = 0; i < targets.Count; i++) {
            var dist = Vector2.Distance(new Vector2(targets[i].X, targets[i].Y) * 16f + Vector2.One * 8f,
                mouse);
            if (best == -1f || dist < best) {
                best = dist;
                target = targets[i];
            }
        }

        if (Collision.InTileBounds(target.X, target.Y, sx, sy, ex, ey)) {
            fx = target.X;
            fy = target.Y;
        }
    }
}
