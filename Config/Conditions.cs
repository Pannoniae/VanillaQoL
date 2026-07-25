using Terraria.Localization;
using Terraria.ModLoader;
using VanillaQoL.API;

namespace VanillaQoL.Config;

public abstract class QoLCondition : Condition {
    public abstract bool enabled();

    protected abstract string key { get; }

    public string reason() => Language.GetTextValue($"Mods.VanillaQoL.ConfigDisabled.{key}");
}

public abstract class ModLoaded(string mod) : QoLCondition {
    public override bool enabled() => ModLoader.HasMod(mod);
}

public abstract class ModMissing(string mod) : QoLCondition {
    public override bool enabled() => !ModLoader.HasMod(mod);
}

public class ThoriumLoaded() : ModLoaded("ThoriumMod") {
    protected override string key => "needsThorium";
}

public class CalamityLoaded() : ModLoaded("CalamityMod") {
    protected override string key => "needsCalamity";
}

public class CalamityAbsent() : ModMissing("CalamityMod") {
    protected override string key => "onlyWithoutCalamity";
}

public class StarlightRiverLoaded() : ModLoaded("StarlightRiver") {
    protected override string key => "needsStarlightRiver";
}

public class ColouredDamageTypesLoaded() : ModLoaded("ColoredDamageTypes") {
    protected override string key => "needsColouredDamageTypes";
}
