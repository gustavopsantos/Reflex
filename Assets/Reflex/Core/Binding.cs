using System;
using Reflex.Exceptions;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Binding
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }

        private Binding()
        {
        }

        private Binding(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }

        public static Binding Validated(IResolver resolver, Type concrete, params Type[] contracts)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ContractDefinitionException(concrete, contract);
                }
            }

            return new Binding(resolver, contracts);
        }
    }
}