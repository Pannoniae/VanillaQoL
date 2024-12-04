using System;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using ZenithQoL.IL;

namespace ZenithQoL.Gameplay;

public class RespawningRework : ModSystem {
    public bool bossAlive;
    public bool eventAlive;

    public ThreatChecker checker = null!;

    public static RespawningRework instance = null!;

    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.respawningRework;
    }

    public override void Load() {
        instance = this;
        // sorry calamity, we do the respawning instead.
        // this one makes it configurable, so the calamity logic needs to be bypassed
        // the defaults are close to the calamity values
        if (ZenithQoL.isCalamityLoaded()) {
            CalamityLogic2.load();
            checker = new CalamityThreatChecker();
        }
        else {
            checker = new VanillaThreatChecker();
        }

        IL_Player.UpdateDead += deathPatch;
    }

    public override void Unload() {
        IL_Player.UpdateDead -= deathPatch;
        instance = null!;
    }

    public override void PostUpdateNPCs() {
        bossAlive = checker.isBossAlive();
        eventAlive = checker.isEvent();
    }

    // [10192 11 - 10192 79]
    // IL_0410: ldarg.0      // this
    // IL_0411: ldarg.0      // this
    // IL_0412: ldfld        int32 Terraria.Player::respawnTimer
    // IL_0417: ldc.i4.1
    // IL_0418: sub
    // --IL_0419: ldc.i4.0
    // --IL_041a: ldc.i4       3600 // 0x00000e10
    // --IL_041f: call         !!0/*int32*/ Terraria.Utils::Clamp<int32>(!!0/*int32*/, !!0/*int32*/, !!0/*int32*/)
    // IL_0424: stfld        int32 Terraria.Player::respawnTimer
    private void deathPatch(ILContext il) {
        Func<Instruction, bool>[] preds = {
            i => i.MatchLdcI4(0),
            i => i.MatchLdcI4(3600),
            i => i.MatchCall(out _),
            i => i.MatchStfld<Player>("respawnTimer")
        };
        ILCursor ilCursor = new ILCursor(il);
        // IL_0424: stfld        int32 Terraria.Player::respawnTimer
        if (!ilCursor.TryGotoNext(MoveType.Before, preds)) {
            ZenithQoL.instance.Logger.Warn("Couldn't match first respawnTimer set in Player.UpdateDead!");
        }

        ilCursor.RemoveRange(3);

        if (!ilCursor.TryGotoNext(MoveType.Before, preds)) {
            ZenithQoL.instance.Logger.Warn("Couldn't match second respawnTimer set in Player.UpdateDead!");
        }

        ilCursor.RemoveRange(3);
    }
}

public class RespawningReworkPlayer : ModPlayer {
    public int bossDeaths;

    public override void Kill(
        double damage,
        int hitDirection,
        bool pvp,
        PlayerDeathReason damageSource) {
        if (RespawningRework.instance.bossAlive) {
            bossDeaths++;
        }

        // time for calculations!
        var config = QoLConfig.Instance;
        float respawnTime = config.respawnTime;
        if (RespawningRework.instance.bossAlive) {
            respawnTime = config.bossRespawnTime;
        }

        if (RespawningRework.instance.eventAlive) {
            respawnTime = config.eventRespawnTime;
        }

        int c = 0;
        for (int index = 0; index < Main.maxPlayers; ++index) {
            Player player = Main.player[index];
            if (player != Main.LocalPlayer && player.active) {
                c++;
            }
        }

        // first, base * expertMode
        if (Main.expertMode) {
            respawnTime *= config.respawnFactorExpertMode;
        }

        if (Main.masterMode) {
            respawnTime *= config.respawnFactorMasterMode;
        }

        if (c > 0 && RespawningRework.instance.bossAlive) {
            respawnTime *= MathF.Pow(config.bossMultiplayerMultiplier, c);
        }

        Player.respawnTimer = (int)(respawnTime * 60f);
    }
}

public interface ThreatChecker {
    bool isBossAlive();

    bool isEvent();
}

// thank you calamity!
public class VanillaThreatChecker : ThreatChecker {
    public bool isBossAlive() {
        // not entirely accurate but we don't want to be hard on performance
        return Main.CurrentFrameFlags.AnyActiveBossNPC;
    }

    // This function follows the behavior of Adrenaline.
    // Vanilla worm segments and Slime God slimes are specifically included.
    // Martian Saucers are specifically excluded.
    public static bool isABoss(NPC? npc) {
        if (npc is null || !npc.active)
            return false;
        if (npc.boss && npc.type != NPCID.MartianSaucerCore)
            return true;
        return npc.type is NPCID.EaterofWorldsBody or NPCID.EaterofWorldsHead or NPCID.EaterofWorldsTail;
    }

    public bool isEvent() {
        // thank you calamity
        int closestPlayer =
            // ReSharper disable once PossibleLossOfFraction
            Player.FindClosest(new Vector2(Main.maxTilesX / 2, (float)Main.worldSurface / 2f) * 16f, 0, 0);
        Player player = Main.player[closestPlayer];
        return anyEvents(player);
    }

    public static bool anyEvents(Player player, bool checkBloodMoon = false) {
        if (Main.invasionType > InvasionID.None && Main.invasionProgressNearInvasion)
            return true;
        if (player.ZoneTowerStardust || player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula)
            return true;
        if (DD2Event.Ongoing && player.ZoneOldOneArmy)
            return true;
        if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && (Main.eclipse || Main.pumpkinMoon || Main.snowMoon))
            return true;
        if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.bloodMoon && checkBloodMoon)
            return true;
        return false;
    }
}

[JITWhenModsEnabled("CalamityMod")]
public class CalamityThreatChecker : ThreatChecker {
    public bool isBossAlive() {
        return CalamityPlayer.areThereAnyDamnBosses;
    }

    public bool isEvent() {
        // thank you calamity
        return CalamityPlayer.areThereAnyDamnEvents;
    }
}