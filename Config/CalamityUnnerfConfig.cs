using Terraria.ModLoader.Config;
using VanillaQoL.API;

namespace VanillaQoL.Config;

/// Undoing what Calamity's "vanilla changes" (https://calamitymod.wiki.gg/wiki/Vanilla_changes)
[BackgroundColor(16, 0, 2, 1)]
[EnabledIf(typeof(CalamityLoaded))]
public class CalamityUnnerfConfig : ModConfig {
    // Field automagically set by tML
#pragma warning disable CS8618
    public static CalamityUnnerfConfig Instance;
#pragma warning restore CS8618

    public override ConfigScope Mode => ConfigScope.ServerSide;
}
