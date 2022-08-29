using System;

namespace Reflex
{
    internal class BindingScopeNotHandledException : Exception
    {
        public BindingScopeNotHandledException(BindingScope scope) : base(GenerateMessage(scope))
        {
        }

        private static string GenerateMessage(BindingScope scope)
        {
            return $"BindingScope '{scope}' not handled.";
        }
    }
}