using System;
using Reflex.Utilities;

namespace Reflex.Reflectors
{
    internal static class ActivatorFactoryManager
    {
        internal static readonly IActivatorFactory Factory;

        static ActivatorFactoryManager()
        {
            Factory = GetFactory();
        }

        private static IActivatorFactory GetFactory()
        {
            switch (ScriptingBackend.Current)
            {
                case ScriptingBackend.Backend.Mono: return new MonoActivatorFactory();
                case ScriptingBackend.Backend.IL2CPP: return new IL2CPPActivatorFactory();
                case ScriptingBackend.Backend.Undefined: throw new Exception("UndefinedRuntimeScriptingBackend");
                default: throw new Exception($"UnhandledRuntimeScriptingBackend {ScriptingBackend.Current}");
            }
        }
    }
}