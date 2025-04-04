using System;
using Reflex.Exceptions;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public interface IBinding
    {
        Type[] Contracts { get; }
        IResolver Resolver { get; }

        protected static void Validate(Type concrete, Type[] contracts)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ContractDefinitionException(concrete, contract);
                }
            }
        }
    }
}