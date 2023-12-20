/*using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver : BaseModuleWeaver {
    public override void Execute() {
        // get all types
        WriteMessage("Removing compiler-generated initonly fields.", MessageImportance.High);
        foreach (var type in ModuleDefinition.Types) {
            // get compiler-generated types
            var nestedTypes = type.NestedTypes.Where(n => n.Name.StartsWith("<>"));
            foreach (var nested in nestedTypes) {
                foreach (var field in nested.Fields) {
                    field.IsInitOnly = false;
                    WriteMessage($"Removed initonly from {field} in {nested}!", MessageImportance.High);
                }
            }
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning() {
        yield return "netstandard";
        yield return "mscorlib";
    }

    public override bool ShouldCleanReference => true;

    void AddConstructor(TypeDefinition newType) {
        var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", attributes, TypeSystem.VoidReference);
        var objectConstructor = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.GetConstructors().First());
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldarg_0);
        processor.Emit(OpCodes.Call, objectConstructor);
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }

    void AddHelloWorld(TypeDefinition newType) {
        var method = new MethodDefinition("World", MethodAttributes.Public, TypeSystem.StringReference);
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldstr, "Hello World");
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }
}*/