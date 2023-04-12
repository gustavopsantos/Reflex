namespace Reflex.Utilities
{
    internal static class ScriptingBackend
    {
        internal enum Backend
        {
            Undefined,
            Mono,
            IL2CPP,
        }

        internal static Backend Current
        {
            get
            {
#if ENABLE_MONO
                return Backend.Mono;
#elif ENABLE_IL2CPP
				return Backend.IL2CPP;
#else
				return Backend.Undefined;
#endif
            }
        }
    }
}