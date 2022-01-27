using System;

namespace Reflex
{
    internal class ScopeNotHandledException : Exception
    {
        public ScopeNotHandledException(BindingScope scope) : base(GenerateMessage(scope))
        {
        }

        private static string GenerateMessage(BindingScope scope)
        {
            return $"BindingScope '{scope}' not handled.";
        }
    }
}