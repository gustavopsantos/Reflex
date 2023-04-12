using System;
using Reflex.Extensions;

namespace Reflex.Exceptions
{
    internal sealed class ContractDefinitionException : Exception
    {
        public ContractDefinitionException(Type concrete, Type contract) : base(GenerateMessage(concrete, contract))
        {
        }

        private static string GenerateMessage(Type concrete, Type contract)
        {
            return $"{concrete.GetFullName()} does not implement {contract.GetFullName()} contract";
        }
    }
}