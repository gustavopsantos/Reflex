using Reflex.Scripts.Utilities;

namespace Reflex
{
    internal static class PlatformReflector
    {
        internal static readonly Reflector Current;

        static PlatformReflector()
        {
            Current = GetReflector();
        }

        private static Reflector GetReflector()
        {
            switch (RuntimeScriptingBackend.Current)
            {
                case RuntimeScriptingBackend.Backend.Undefined: throw new UndefinedRuntimeScriptingBackendException();
                case RuntimeScriptingBackend.Backend.Mono: return new MonoReflector();
                case RuntimeScriptingBackend.Backend.IL2CPP: return new IL2CPPReflector();
                default: throw new UnhandledRuntimeScriptingBackendException(RuntimeScriptingBackend.Current);
            }
        }
    }
}