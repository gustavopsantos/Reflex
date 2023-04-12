using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Reflex.Logging;
using Reflex.Weaving.Extensions;
using UnityEngine;

namespace Reflex.Weaving
{
    internal static class ReflexILEmitter
    {
        public static void WeaveAssembly(string assemblyName)
        {
            var reader = new ReaderParameters
            {
                AssemblyResolver = new UnityAssemblyResolver(),
                ReadWrite = true
            };

            using var asmDef = AssemblyDefinition.ReadAssembly(assemblyName, reader);
            
            if (asmDef.MainModule.AssemblyReferences.Count(r => r.Name == "UnityEngine.TestRunner") != 0)
            {
                return; // We don't need to weave test assemblies
            }

            if (asmDef.Modules.Select(WeaveModule).Any(wasWeaved => wasWeaved))
            {
                asmDef.Write();
            }
        }
        
        private static bool WeaveModule(ModuleDefinition module)
        {
            return module.GetTypes().Select(WeaveType).Any(wasWeaved => wasWeaved);         
        }

        private static bool WeaveType(TypeDefinition type)
        {
            var constructors = type.Methods.Where(m => m.Name == ".ctor");
            var arguments = constructors.SelectMany(c => c.Parameters).ToArray();
            var distinctElements = arguments
                .Select(a => a.ParameterType.IsEnumerable(out var elementType) ? (true, elementType) : (false, default(TypeReference)))
                .Where(pair => pair.Item1)
                .Select(pair => pair.Item2)
                .DistinctBy(e => e.FullName)
                .ToList();
            
            if (distinctElements.Count == 0)
            {
                return false; // Weaving this type is not necessary
            }
            
            ReflexLogger.Log($"Reflex weaving Enumerable.Cast<> for: {distinctElements.Select(e => e.FullName).AsCSV()} at {type.FullName}", LogLevel.Info);

            var voidType = type.Module.ImportReference(typeof(void).AsDefinition());
            var objectType = type.Module.ImportReference(typeof(object).AsDefinition());
            var enumerableType = typeof(Enumerable).AsDefinition();
            var methodDef = new MethodDefinition("Reflex_UsedOnlyForAOTCodeGeneration", MethodAttributes.Private | MethodAttributes.Static, voidType);
            var il = methodDef.Body.GetILProcessor();
            var castMethod = type.Module.ImportReference(enumerableType.Methods.Single(m => m.Name == "Cast" && m.GenericParameters.Count == 1));

            foreach (var element in distinctElements)
            {
                // It generates: new object[0].Cast<TElement>();
                il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
                il.Append(Instruction.Create(OpCodes.Newarr, objectType));
                il.Append(Instruction.Create(OpCodes.Call, castMethod.MakeGeneric(type.Module.ImportReference(element))));
                il.Append(Instruction.Create(OpCodes.Pop));
            }

            il.Append(Instruction.Create(OpCodes.Ret));

            type.Methods.Add(methodDef);
            return true;
        }
    }
}