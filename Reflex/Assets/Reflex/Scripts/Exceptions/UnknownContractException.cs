using System;
using System.Linq;

namespace Reflex
{
    internal class UnknownContractException : Exception
    {
        public UnknownContractException(Type contract) : base(GenerateMessage(contract))
        {
        }

        private static string GenerateMessage(Type contract)
        {
            if (!contract.IsGenericType)
            {
                return $"Cannot resolve contract type '{contract}'.";
            }

            var genericContract = contract.Name.Remove(contract.Name.IndexOf('`'));
            var genericArguments = contract.GenericTypeArguments.Select(args => args.FullName);
            var commaSeparatedArguments = string.Join(", ", genericArguments);
            return $"Cannot resolve contract type '{genericContract}<{commaSeparatedArguments}>'.";
        }
    }
}