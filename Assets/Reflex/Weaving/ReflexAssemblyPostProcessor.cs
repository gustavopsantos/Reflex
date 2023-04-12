using System.Collections.Generic;
using UnityEditor;

namespace Reflex.Weaving
{
    public static class ReflexAssemblyPostProcessor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            var assemblies = new List<string>();

            UnityEditor.Compilation.CompilationPipeline.compilationStarted += (o) =>
            {
                //EditorApplication.LockReloadAssemblies();
                assemblies.Clear();
            };

            UnityEditor.Compilation.CompilationPipeline.assemblyCompilationFinished += (assemblyName, message) =>
            {
                assemblies.Add(assemblyName);
            };

            UnityEditor.Compilation.CompilationPipeline.compilationFinished += (o) =>
            {
                foreach (var assemblyName in assemblies)
                {
                    ReflexILEmitter.WeaveAssembly(assemblyName);
                }

                //EditorApplication.UnlockReloadAssemblies();
                EditorUtility.RequestScriptReload(); // Without this, sometimes unity cant find edit/play mode tests
            };
        }
    }
}