using System;

namespace Reflex
{
    internal class UnknownMethodException : Exception
    {
        public UnknownMethodException(Type contract) : base(GenerateMessage(contract))
        {
        }

        private static string GenerateMessage(Type contract)
        {
            return $"Cannot resolve method of type '{contract}'.";
        }
    }
}