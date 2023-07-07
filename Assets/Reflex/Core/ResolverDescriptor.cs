using System;
using Reflex.Resolvers;

namespace Reflex.Core
{
    internal class ResolverDescriptor
    {
        public Func<Resolver> ResolverFactory { get; }
        public Type[] Contracts { get; }

        public ResolverDescriptor(Func<Resolver> resolverFactory, Type[] contracts)
        {
            ResolverFactory = resolverFactory;
            Contracts = contracts;
        }
    }
}