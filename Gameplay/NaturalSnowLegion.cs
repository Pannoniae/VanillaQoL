using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace VanillaQoL.Gameplay;

/// <summary>
/// Sorry for the big spaghetti.
/// </summary>
public class NaturalSnowLegion : ModSystem {
    public override void PostUpdateTime() {
        if (QoLConfig.Instance.naturalFrostLegion) {
            if (!Main.dayTime) {
                // yes, fucking method is named the other way around
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                if (!Main.IsFastForwardingTime() && !stopEvents) {
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        int nightTicks = (int)Main.nightLength;
                        // 1/8 chance of spawning at night when blizzard
                        if (Main.raining && Main.rand.NextBool(8 * nightTicks)) {
                            for (int m = 0; m < 255; m++) {
                                Player player = Main.player[m];
                                if (Main.hardMode && player.active && !player.dead &&
                                    !(player.position.Y >= Main.worldSurface * 16.0) && !(player.townNPCs > 0f) &&
                                    (player.statLifeMax2 >= 400 || player.statDefense >= 20) && !NPC.AnyDanger()) {
                                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                                        if (Main.invasionType == 0) {
                                            Main.invasionDelay = 0;
                                            Main.StartInvasion(2);
                                        }
                                    }
                                    else {
                                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null,
                                            player.whoAmI, -2f);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}