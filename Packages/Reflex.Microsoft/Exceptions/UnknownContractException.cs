using System;
using Reflex.Microsoft.Extensions;

namespace Reflex.Microsoft.Exceptions
{
    public sealed class UnknownContractException : Exception
    {
        public UnknownContractException(Type contract) : base(GenerateMessage(contract))
        {
        }

        private static string GenerateMessage(Type contract)
        {
            return $"Cannot resolve contract '{contract.GetFullName()}'.";
        }
    }
}