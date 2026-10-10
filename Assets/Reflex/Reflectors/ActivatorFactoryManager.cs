using System;
using Reflex.Utilities;

namespace Reflex.Reflectors
{
    internal static class ActivatorFactoryManager
    {
        internal static readonly IActivatorFactory Factory;

        static ActivatorFactoryManager()
        {
            Factory = ScriptingBackend.Current switch
            {
                ScriptingBackend.Backend.Mono => new MonoActivatorFactory(),
                ScriptingBackend.Backend.IL2CPP => new IL2CPPActivatorFactory(),
#if UNITY_6000_7_OR_NEWER
                ScriptingBackend.Backend.CoreCLR => new IL2CPPActivatorFactory(),
#endif
                _ => throw new Exception($"UnhandledRuntimeScriptingBackend {ScriptingBackend.Current}")
            };
        }
    }
}