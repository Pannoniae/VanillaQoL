using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace VanillaQoL.Gameplay.Mechanics.SmartCursor;

public class PlatformTargeting {
    // how thick a gap we're willing to cross
    private const int max = 3;

    // the eight directions
    private static readonly Point16[] compass = [
        new(1, 0), new(1, 1), new(0, 1), new(-1, 1), new(-1, 0), new(-1, -1), new(0, -1), new(1, -1)
    ];

    private static readonly List<Point16> scratch = [];

    // convert Tuple<int, int> -> Point16
    public static void selectVanilla(List<Tuple<int, int>> targets, Vector2 mouse, int sx, int sy,
        int ex, int ey, ref int fx, ref int fy) {
        scratch.Clear();
        foreach (var t in targets) {
            scratch.Add(new Point16(t.Item1, t.Item2));
        }

        selectTarget(scratch, mouse, sx, sy, ex, ey, ref fx, ref fy);
    }

    public static void selectTarget(List<Point16> targets, Vector2 mouse, int sx, int sy,
        int ex, int ey, ref int fx, ref int fy) {
        if (targets.Count == 0) {
            return;
        }

        if (adjacentTile(targets, mouse, sx, sy, ex, ey, ref fx, ref fy)) {
            return;
        }

        // pointing at the tile itself is not a placement
        var hover = Main.tile[(int)(mouse.X / 16f), (int)(mouse.Y / 16f)];
        if (hover.HasTile && TileID.Sets.Platforms[hover.TileType]) {
            return;
        }

        // no platforms in reach (fresh placement off blocks/walls) - vanilla knows best
        if (!findAnchor(mouse, sx, sy, ex, ey, out var hx, out var hy)) {
            nearest(targets, mouse, sx, sy, ex, ey, ref fx, ref fy);
            return;
        }

        var to = mouse - (new Vector2(hx, hy) * 16f + Vector2.One * 8f);
        var bestDot = float.MinValue;
        var dir = compass[0];
        var sq2 = float.Sqrt(2f);
        foreach (var d in compass) {
            var len = d.X != 0 && d.Y != 0 ? sq2 : 1f;
            var dot = (to.X * d.X + to.Y * d.Y) / len;
            if (dot > bestDot) {
                bestDot = dot;
                dir = d;
            }
        }

        var cx = hx + dir.X;
        var cy = hy + dir.Y;
        // vanilla already generated the cell if the position is free and valid, so if it's
        // in the list, we're good
        if (targets.Contains(new Point16(cx, cy))) {
            if (Collision.InTileBounds(cx, cy, sx, sy, ex, ey)) {
                fx = cx;
                fy = cy;
            }

            return;
        }

        var blockTile = Main.tile[cx, cy];
        if (blockTile.HasTile && !Main.tileCut[blockTile.TileType]) {
            if (TileID.Sets.Platforms[blockTile.TileType]) {
                return;
            }

            for (var i = 2; i <= max + 1; i++) {
                var x = hx + dir.X * i;
                var y = hy + dir.Y * i;
                if (!WorldGen.InWorld(x, y, 5)) {
                    return;
                }

                var tile = Main.tile[x, y];
                if (tile.HasTile && TileID.Sets.Platforms[tile.TileType]) {
                    // ran into existing platforms
                    break;
                }

                if (tile.HasTile && !Main.tileCut[tile.TileType]) {
                    continue;
                }

                if (canAnchorPlatform(x, y) && Collision.InTileBounds(x, y, sx, sy, ex, ey)) {
                    fx = x;
                    fy = y;
                    return;
                }

                break;
            }

            return;
        }

        // cos 67.5
        var ringBest = 0.3826834f * to.Length();
        var found = false;
        var rx = 0;
        var ry = 0;
        foreach (var d in compass) {
            var len = d.X != 0 && d.Y != 0 ? sq2 : 1f;
            var dot = (to.X * d.X + to.Y * d.Y) / len;
            if (dot > ringBest && targets.Contains(new Point16(hx + d.X, hy + d.Y))) {
                ringBest = dot;
                rx = hx + d.X;
                ry = hy + d.Y;
                found = true;
            }
        }

        if (found && Collision.InTileBounds(rx, ry, sx, sy, ex, ey)) {
            fx = rx;
            fy = ry;
        }

        // otherwise: nothing > a shit suggestion.
    }

    // direct case, we point at an adjacent tile, we're good
    private static bool adjacentTile(List<Point16> targets, Vector2 mouse, int sx, int sy, int ex, int ey,
        ref int fx, ref int fy) {
        var x = (int)(mouse.X / 16f);
        var y = (int)(mouse.Y / 16f);
        if (targets.Contains(new Point16(x, y)) && Collision.InTileBounds(x, y, sx, sy, ex, ey)) {
            fx = x;
            fy = y;
            return true;
        }

        return false;
    }

    private static bool findAnchor(Vector2 mouse, int sx, int sy, int ex, int ey, out int hx, out int hy) {
        hx = hy = 0;
        var best = -1f;

        // one tile of margin: a platform just outside the box can still own placements inside it
        for (var x = sx - 1; x <= ex + 1; x++) {
            for (var y = sy - 1; y <= ey + 1; y++) {
                var tile = Main.tile[x, y];
                if (!tile.HasTile || !TileID.Sets.Platforms[tile.TileType]) {
                    continue;
                }

                var dist = Vector2.Distance(new Vector2(x, y) * 16f + Vector2.One * 8f, mouse);
                if (best == -1f || dist < best) {
                    best = dist;
                    hx = x;
                    hy = y;
                }
            }
        }

        return best != -1f;
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
