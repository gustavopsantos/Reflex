using System;
using IL2CPPTest.Models;
using UnityEngine.Scripting;

namespace IL2CPPTest
{
    // In some extreme cases, you'll need to hint compiler so your code doesn't get stripped
    // https://docs.unity3d.com/Manual/ScriptingRestrictions.html
    public static class AOTCodeGeneration
    {
        [Preserve]
        private static void UsedOnlyForAOTCodeGeneration()
        {
            new TestGenericStructure<int>(default);
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }
    }
}