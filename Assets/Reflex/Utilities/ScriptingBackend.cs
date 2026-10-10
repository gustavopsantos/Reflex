namespace Reflex.Utilities
{
    internal static class ScriptingBackend
    {
        internal enum Backend
        {
            Undefined,
            Mono,
            IL2CPP,
            CoreCLR,
        }

        internal static Backend Current
        {
            get
            {
#if ENABLE_MONO
                return Backend.Mono;
#elif ENABLE_IL2CPP
                return Backend.IL2CPP;
#elif ENABLE_CORECLR
                return Backend.CoreCLR;
#else
                return Backend.Undefined;
#endif
            }
        }
    }
}