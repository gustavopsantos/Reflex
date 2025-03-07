using System;

namespace Reflex.Exceptions
{
    public sealed class UnknownIdentifierException : Exception
    {
        public UnknownIdentifierException(string identifier) : base(GenerateMessage(identifier))
        {
        }

        private static string GenerateMessage(string identifier)
        {
            return $"Cannot resolve id '{identifier}'.";
        }
    }
}