using Terraria.ModLoader.Config;
using VanillaQoL.API;

namespace VanillaQoL.Config;

/// Calamity's QoL changes, for people who don't have Calamity. (used to be Calamity QOL for Vanilla)
[BackgroundColor(16, 0, 2, 1)]
[EnabledIf(typeof(CalamityAbsent))]
public class CalamityQoLConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618
    public static CalamityQoLConfig Instance;
#pragma warning restore CS8618

    public override ConfigScope Mode => ConfigScope.ServerSide;
}
