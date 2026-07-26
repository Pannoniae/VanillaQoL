using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

// like ildump but for people who can't read IL

if (args.Length == 0) {
    Console.Error.WriteLine("Usage: csdump --list");
    Console.Error.WriteLine("       csdump <ModName|workshop-id|path.tmod|path.dll|tml>      dump whole source tree to csdump/<Name>/src");
    Console.Error.WriteLine("       csdump <target> <TypeName> [MethodName]                  dump one type/method as C# to stdout");
    Console.Error.WriteLine("       csdump <target> --files                                  list .tmod contents");
    Console.Error.WriteLine("       csdump --all                                             dump everything, go make a coffee");
    Console.Error.WriteLine("       csdump --scan <int>                                      find constants in mod (IL)");
    return 1;
}

var outRoot = Path.Combine(Environment.CurrentDirectory, "csdump");
var positional = new List<string>();
bool list = false, all = false, files = false, scan = false;
foreach (var arg in args) {
    switch (arg) {
        case "--list":
            list = true;
            break;
        case "--all":
            all = true;
            break;
        case "--files":
            files = true;
            break;
        case "--scan":
            scan = true;
            break;
        default:
            positional.Add(arg);
            break;
    }
}

var candidates = discover().ToList();

if (list) {
    foreach (var c in candidates.OrderBy(c => c.name, StringComparer.OrdinalIgnoreCase)) {
        Console.WriteLine($"{c.name,-40} {c.workshopId ?? "local",-12} {c.tmlVersion,-8} {c.path}");
    }

    Console.WriteLine($"{candidates.Count} mods");
    return 0;
}

if (all) {
    int failed = 0;
    foreach (var c in candidates) {
        try {
            dumpTree(c.path, outRoot);
        }
        catch (Exception ex) {
            Console.Error.WriteLine($"FAILED {c.name}: {ex.Message}");
            failed++;
        }
    }

    return failed == 0 ? 0 : 1;
}

if (scan) {
    if (positional.Count == 0 || !int.TryParse(positional[0], out var needle)) {
        Console.Error.WriteLine("--scan requires a number (tileid, itemid, anything)");
        return 1;
    }

    scanAll(needle, candidates);
    return 0;
}

if (positional.Count == 0) {
    Console.Error.WriteLine("Give me a mod name / workshop id / path. Or --list if you forgot what you have.");
    return 1;
}

var target = resolve(positional[0], candidates);
if (target == null) {
    Console.Error.WriteLine($"No mod matching '{positional[0]}' found. Try --list.");
    return 1;
}

if (files) {
    using var tmod = TmodArchive.open(target);
    foreach (var e in tmod.entries) {
        Console.WriteLine($"{e.length,10} {e.name}");
    }

    return 0;
}

try {
    if (positional.Count > 1) {
        return dumpType(target, positional[1], positional.Count > 2 ? positional[2] : null);
    }

    dumpTree(target, outRoot);
    return 0;
}
catch (Exception ex) {
    Console.Error.WriteLine($"FAILED {Path.GetFileName(target)}: {ex.Message}");
    return 1;
}

// ---------------------------------------------------------------------------

static string? resolve(string arg, List<ModCandidate> candidates) {
    if (arg is "tml" or "terraria" or "tModLoader") {
        var dll = Path.Combine(tmlDirs().First(), "tModLoader.dll");
        return File.Exists(dll) ? dll : null;
    }

    if (File.Exists(arg)) {
        return Path.GetFullPath(arg);
    }

    return candidates.FirstOrDefault(c =>
        c.name.Equals(arg, StringComparison.OrdinalIgnoreCase) || c.workshopId == arg)?.path;
}

// whole tree. if src/ already has stuff in it (like a checkout you put there yourself), we don't touch it
static void dumpTree(string path, string outRoot) {
    if (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
        var name = Path.GetFileNameWithoutExtension(path);
        Console.WriteLine($"{name} -> {Path.Combine(outRoot, name)}");
        decompile(path, Path.Combine(outRoot, name, "src"), [Path.GetDirectoryName(path)!]);
        return;
    }

    using var tmod = TmodArchive.open(path);
    var modDir = Path.Combine(outRoot, tmod.name);
    var srcDir = Path.Combine(modDir, "src");
    if (Directory.Exists(srcDir) && Directory.EnumerateFileSystemEntries(srcDir).Any()) {
        Console.WriteLine($"{tmod.name}: src/ already there, not touching it");
        return;
    }

    Console.WriteLine($"{tmod.name} v{tmod.modVersion} (tML {tmod.tmlVersion}, {tmod.entries.Count} files) -> {modDir}");

    // the hjson is always worth having
    string[] textExt = [".txt", ".json", ".hjson", ".csv", ".md", ".toml", ".yml", ".yaml", ".cs", ".fx", ".xml"];
    var libDir = Path.Combine(modDir, "lib");
    int assets = 0;
    foreach (var e in tmod.entries) {
        // NO PATH TRAVERSAL YOU MORON (mayybeee don't extract malware)
        if (e.name.Contains("..") || Path.IsPathRooted(e.name)) {
            throw new IOException($"suspicious entry path: {e.name}");
        }

        var ext = Path.GetExtension(e.name).ToLowerInvariant();
        string? dest = null;
        if (ext is ".dll" or ".pdb") {
            dest = Path.Combine(libDir, Path.GetFileName(e.name));
        }
        else if (textExt.Contains(ext)) {
            dest = Path.Combine(modDir, "assets", e.name.Replace('/', Path.DirectorySeparatorChar));
            assets++;
        }

        if (dest == null) {
            continue;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        File.WriteAllBytes(dest, tmod.readEntry(e));
    }

    Console.WriteLine($"  {assets} text assets -> assets/");

    var mainDll = Path.Combine(libDir, tmod.name + ".dll");
    if (!File.Exists(mainDll)) {
        Console.WriteLine("  no main assembly in there, nothing to decompile");
        return;
    }

    decompile(mainDll, srcDir, [libDir, .. tmlDirs()]);
}

static void decompile(string dllPath, string srcDir, string[] searchDirs) {
    using var module = new PEFile(dllPath);
    var resolver = new UniversalAssemblyResolver(dllPath, false, module.DetectTargetFrameworkId());
    foreach (var dir in searchDirs) {
        resolver.AddSearchDirectory(dir);
    }

    var settings = new DecompilerSettings(LanguageVersion.Latest) {
        ThrowOnAssemblyResolveErrors = false,
        UseNestedDirectoriesForNamespaces = true
    };
    var decompiler = new WholeProjectDecompiler(settings, resolver, null, null, null);
    Directory.CreateDirectory(srcDir);
    decompiler.DecompileProject(module, srcDir);
    Console.WriteLine("  decompiled -> src/");
}

// one type (or method) as C# to stdout, for actually reading code
static int dumpType(string target, string typeName, string? methodName) {
    PEFile module;
    var searchDirs = tmlDirs().ToList();
    if (target.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
        module = new PEFile(target);
        searchDirs.Add(Path.GetDirectoryName(target)!);
    }
    else {
        // pull the dll straight out of the tmod, no point writing it anywhere
        using var tmod = TmodArchive.open(target);
        var entry = tmod.entries.FirstOrDefault(e => e.name == tmod.name + ".dll");
        if (entry == null) {
            Console.Error.WriteLine($"{tmod.name} has no main assembly");
            return 1;
        }

        module = new PEFile(tmod.name + ".dll", new MemoryStream(tmod.readEntry(entry)));
    }

    using var _ = module;
    var resolver = new UniversalAssemblyResolver(module.FileName, false, module.DetectTargetFrameworkId());
    foreach (var dir in searchDirs) {
        resolver.AddSearchDirectory(dir);
    }

    var settings = new DecompilerSettings(LanguageVersion.Latest) {
        ThrowOnAssemblyResolveErrors = false
    };
    var decompiler = new CSharpDecompiler(module, resolver, settings);

    var type = decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault(t =>
        t.FullName == typeName || t.Name == typeName);
    if (type == null) {
        Console.Error.WriteLine($"Type '{typeName}' not found in {module.FileName}");
        return 1;
    }

    if (methodName == null) {
        Console.WriteLine(decompiler.DecompileTypeAsString(type.FullTypeName));
        return 0;
    }

    var handles = type.Methods.Where(m => m.Name == methodName)
        .Select(m => (System.Reflection.Metadata.EntityHandle)m.MetadataToken).ToList();
    if (handles.Count == 0) {
        Console.Error.WriteLine($"Method '{methodName}' not found in type '{type.FullName}'");
        return 1;
    }

    Console.WriteLine(decompiler.DecompileAsString(handles));
    return 0;
}

// "which mod is doing this to me", the tool
// this is a VERY bad and hacky way of doing things, todo proper disassembly?
static void scanAll(int needle, List<ModCandidate> candidates) {
    byte[] pattern = [0x20, .. BitConverter.GetBytes(needle)];
    int hits = 0;
    foreach (var c in candidates.OrderBy(c => c.name, StringComparer.OrdinalIgnoreCase)) {
        try {
            using var tmod = TmodArchive.open(c.path);
            var entry = tmod.entries.FirstOrDefault(e => e.name == tmod.name + ".dll");
            if (entry == null) {
                continue;
            }

            using var module = new PEFile(tmod.name, new MemoryStream(tmod.readEntry(entry)));
            foreach (var m in methodContains(module, pattern)) {
                Console.WriteLine($"{c.name,-32} {m}");
                hits++;
            }
        }
        catch (Exception ex) {
            Console.Error.WriteLine($"skipped {c.name}: {ex.Message}");
        }
    }

    Console.WriteLine($"{hits} method(s) mentioning {needle}");
}

static IEnumerable<string> methodContains(PEFile module, byte[] pattern) {
    var md = module.Metadata;
    foreach (var handle in md.MethodDefinitions) {
        var method = md.GetMethodDefinition(handle);
        if (method.RelativeVirtualAddress == 0) {
            continue;
        }

        byte[] il;
        try {
            il = module.GetMethodBody(method.RelativeVirtualAddress).GetILBytes()!;
        }
        catch {
            continue;
        }

        if (!contains(il, pattern)) {
            continue;
        }

        var type = md.GetTypeDefinition(method.GetDeclaringType());
        var ns = md.GetString(type.Namespace);
        yield return $"{(ns.Length > 0 ? ns + "." : "")}{md.GetString(type.Name)}::{md.GetString(method.Name)}";
    }
}

static bool contains(byte[] haystack, byte[] needle) {
    for (int i = 0; i <= haystack.Length - needle.Length; i++) {
        int j = 0;
        while (j < needle.Length && haystack[i + j] == needle[j]) {
            j++;
        }

        if (j == needle.Length) {
            return true;
        }
    }

    return false;
}

// the actual tML install, dll at the root and everything else squirrelled away under Libraries/.
static IEnumerable<string> tmlDirs() {
    const string root = @"D:\SteamLibrary\steamapps\common\tModLoader";
    yield return root;
    var libs = Path.Combine(root, "Libraries");
    if (!Directory.Exists(libs)) {
        yield break;
    }

    // resolver search dirs aren't recursive, so feed it every dir that has a dll in it
    foreach (var dir in Directory.EnumerateFiles(libs, "*.dll", SearchOption.AllDirectories)
                 .Select(Path.GetDirectoryName).Distinct()) {
        yield return dir!;
    }
}

static IEnumerable<ModCandidate> discover() {
    var mods = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "my games", "Terraria", "tModLoader", "Mods");
    if (Directory.Exists(mods)) {
        foreach (var f in Directory.EnumerateFiles(mods, "*.tmod")) {
            yield return new ModCandidate(Path.GetFileNameWithoutExtension(f), null, "local", f);
        }
    }

    foreach (var root in steamWorkshopRoots()) {
        foreach (var idDir in Directory.EnumerateDirectories(root)) {
            // one subfolder per tML version, newest wins
            var best = Directory.EnumerateDirectories(idDir)
                .Select(d => (dir: d, ver: Version.TryParse(Path.GetFileName(d), out var v) ? v : new Version(0, 0)))
                .OrderByDescending(t => t.ver)
                .Select(t => t.dir)
                .FirstOrDefault();
            if (best == null) {
                continue;
            }

            var tmod = Directory.EnumerateFiles(best, "*.tmod").FirstOrDefault();
            if (tmod == null) {
                continue;
            }

            yield return new ModCandidate(Path.GetFileNameWithoutExtension(tmod),
                Path.GetFileName(idDir), Path.GetFileName(best), tmod);
        }
    }
}

static IEnumerable<string> steamWorkshopRoots() {
    var bases = new List<string> { @"C:\Program Files (x86)\Steam" };
    var vdf = Path.Combine(bases[0], "steamapps", "libraryfolders.vdf");
    if (File.Exists(vdf)) {
        foreach (Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"([^\"]+)\"")) {
            bases.Add(m.Groups[1].Value.Replace(@"\\", @"\"));
        }
    }

    return bases.Distinct(StringComparer.OrdinalIgnoreCase)
        .Select(b => Path.Combine(b, "steamapps", "workshop", "content", "1281930"))
        .Where(Directory.Exists);
}

record ModCandidate(string name, string? workshopId, string tmlVersion, string path);

// skidded reader, shhhh, too lazy to do it properly
sealed class TmodArchive : IDisposable {
    public string name = null!;
    public string modVersion = null!;
    public string tmlVersion = null!;
    public List<TmodEntry> entries = null!;
    private FileStream stream = null!;

    public static TmodArchive open(string path) {
        var fs = File.OpenRead(path);
        var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen: true);
        if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "TMOD") {
            throw new IOException("not a .tmod file (bad magic)");
        }

        var tmlVersion = br.ReadString();
        br.ReadBytes(20);
        br.ReadBytes(256);
        br.ReadInt32();
        if (!Version.TryParse(tmlVersion, out var v) || v < new Version(0, 11)) {
            throw new IOException($"ancient pre-0.11 .tmod, where did you even find this (tML {tmlVersion})");
        }

        var name = br.ReadString();
        var modVersion = br.ReadString();
        var count = br.ReadInt32();
        var entries = new List<TmodEntry>(count);
        for (int i = 0; i < count; i++) {
            entries.Add(new TmodEntry(br.ReadString(), br.ReadInt32(), br.ReadInt32(), 0));
        }

        long offset = fs.Position;
        for (int i = 0; i < count; i++) {
            entries[i] = entries[i] with { offset = offset };
            offset += entries[i].compressedLength;
        }

        return new TmodArchive {
            name = name, modVersion = modVersion, tmlVersion = tmlVersion, entries = entries, stream = fs
        };
    }

    public byte[] readEntry(TmodEntry e) {
        stream.Seek(e.offset, SeekOrigin.Begin);
        var raw = new byte[e.compressedLength];
        stream.ReadExactly(raw);
        if (e.compressedLength == e.length) {
            return raw;
        }

        using var ds = new DeflateStream(new MemoryStream(raw), CompressionMode.Decompress);
        var buf = new byte[e.length];
        ds.ReadExactly(buf);
        return buf;
    }

    public void Dispose() {
        stream.Dispose();
    }
}

record TmodEntry(string name, int length, int compressedLength, long offset);
