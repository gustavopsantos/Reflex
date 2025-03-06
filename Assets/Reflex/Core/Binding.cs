using System;
using Reflex.Exceptions;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Binding
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }
        public string Identifier { get; } = null;
        
        private Binding()
        {
        }

        private Binding(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }

        private Binding(IResolver resolver, Type[] contracts, string identifier)
        {
            Resolver = resolver;
            Contracts = contracts;
            Identifier = identifier;
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

        public static Binding Validated(IResolver resolver, Type concrete, string identifier, params Type[] contracts)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ContractDefinitionException(concrete, contract);
                }
            }

            return new Binding(resolver, contracts, identifier);
        }
    }
}