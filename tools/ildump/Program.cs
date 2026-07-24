using System;
using System.Linq;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

if (args.Length < 2) {
    Console.Error.WriteLine("Usage: ildump <assembly-path> <TypeName> [MethodName]");
    Console.Error.WriteLine("  TypeName: fully qualified, e.g. Terraria.Item");
    Console.Error.WriteLine("  MethodName: optional, dumps all methods if omitted");
    return 1;
}

string assemblyPath = args[0];
string typeName = args[1];
string? methodName = args.Length >= 3 ? args[2] : null;

try {
    var module = new PEFile(assemblyPath);
    var output = new PlainTextOutput(Console.Out);
    var disassembler = new ReflectionDisassembler(output, default);

    var typeHandle = module.Metadata.TypeDefinitions.FirstOrDefault(td => {
        var typeDef = module.Metadata.GetTypeDefinition(td);
        var ns = module.Metadata.GetString(typeDef.Namespace);
        var name = module.Metadata.GetString(typeDef.Name);
        var fullName = string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        return fullName == typeName || name == typeName;
    });

    if (typeHandle.IsNil) {
        Console.Error.WriteLine($"Type '{typeName}' not found in {assemblyPath}");
        return 1;
    }

    var type = module.Metadata.GetTypeDefinition(typeHandle);

    if (methodName == null) {
        disassembler.DisassembleType(module, typeHandle);
    }
    else {
        var methods = type.GetMethods().Where(mh => {
            var method = module.Metadata.GetMethodDefinition(mh);
            return module.Metadata.GetString(method.Name) == methodName;
        }).ToList();

        if (methods.Count == 0) {
            Console.Error.WriteLine($"Method '{methodName}' not found in type '{typeName}'");
            return 1;
        }

        foreach (var mh in methods) {
            disassembler.DisassembleMethod(module, mh);
            Console.WriteLine();
        }
    }
}
catch (Exception ex) {
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

return 0;