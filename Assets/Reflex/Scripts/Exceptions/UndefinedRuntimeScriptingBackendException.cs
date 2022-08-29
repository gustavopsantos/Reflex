using System;

namespace Reflex
{
    public class UndefinedRuntimeScriptingBackendException : Exception
    {
        public UndefinedRuntimeScriptingBackendException() : base(GenerateMessage())
        {
        }

        private static string GenerateMessage()
        {
            return "Scripting backend could not be defined.";
        }
    }
}