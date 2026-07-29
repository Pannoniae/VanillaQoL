using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay.Calamity.Unnerf;


[JITWhenModsEnabled("CalamityMod")]
public class RecipeUnnerfs : ModSystem {
    private static readonly int[] killedByCalamity = [
        ItemID.MiniNukeI, ItemID.MiniNukeII, ItemID.ReconScope
    ];

    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaQoL.isCalamityLoaded() && CalamityUnnerfConfig.Instance.vanillaRecipes;
    }

    public override void PostAddRecipes() {
        var setter = typeof(Recipe).GetProperty(nameof(Recipe.Disabled))?.GetSetMethod(true);
        if (setter == null) {
            VanillaQoL.instance.Logger.Warn("No setter on Recipe.Disabled?");
            return;
        }

        var x = 0;
        foreach (var recipe in Main.recipe) {
            // vanilla ones only because a mod disabling its own recipe is between it and Terry
            if (recipe is not { Disabled: true, Mod: null }) {
                continue;
            }

            foreach (var type in killedByCalamity) {
                if (recipe.HasResult(type)) {
                    setter.Invoke(recipe, [false]);
                    x++;
                    break;
                }
            }
        }

        if (x != killedByCalamity.Length) {
            VanillaQoL.instance.Logger.Warn(
                $"Brought back {x} vanilla recipes but expected {killedByCalamity.Length}??");
        }
    }
}
