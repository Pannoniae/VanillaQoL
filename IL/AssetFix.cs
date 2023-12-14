using System;
using System.IO;
using Terraria.ModLoader;
using VanillaQoL.Config;

namespace VanillaQoL.IL;

public class AssetFix : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        // if not on linux just don't do anything
        var linux = OperatingSystem.IsLinux();
        var a = (VanillaQoL)Mod;
        return QoLConfig.Instance.assetFix && linux;
    }

    public override void OnModLoad() {
        // fix the file paths in terraria
        VanillaQoL.instance.Logger.Warn(Environment.CurrentDirectory);
        var currentPath = Environment.CurrentDirectory;
        var steamAppsCommon = Directory.GetParent(currentPath)!.FullName;
        var terraria = Path.Join(steamAppsCommon, "Terraria");
        var content = Path.Join(terraria, "Content");
        var images = Path.Join(content, "Images");
        try {
            foreach (var file in Directory.EnumerateFiles(images)) {
                var name = Path.GetFileName(file);
                if (char.IsLower(name[0]) &&
                    (name.Contains("Projectile", StringComparison.InvariantCultureIgnoreCase) ||
                     name.Contains("Tile", StringComparison.InvariantCultureIgnoreCase) ||
                     name.Contains("Item", StringComparison.InvariantCultureIgnoreCase))) {
                    // move file to uppercase
                    var s = char.ToUpperInvariant(name[0]);
                    var rem = name[1..];
                    File.Move(file, Path.Join(images, s + rem));
                    VanillaQoL.instance.Logger.Info($"Moved {file} to {Path.Join(images, s + rem)}!");
                }

                // special-case "TIles"
                if (name.Contains("Tiles", StringComparison.InvariantCultureIgnoreCase) &&
                    !name.Contains("Tiles", StringComparison.InvariantCulture)) {
                    var s = char.ToUpperInvariant(name[0]);
                    var rem = name[1..].ToLowerInvariant();
                    File.Move(file, Path.Join(images, s + rem));
                    VanillaQoL.instance.Logger.Info($"Moved {file} to {Path.Join(images, s + rem)}!");
                }
            }
        }
        catch (Exception e) {
            VanillaQoL.instance.Logger.Warn(
                $"Couldn't fix file paths in the vanilla Terraria folder! Is it write-protected? {e}");
        }
    }
}