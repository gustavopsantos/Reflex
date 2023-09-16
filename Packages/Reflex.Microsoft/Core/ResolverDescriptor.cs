using System;
using Reflex.Microsoft.Resolvers;

namespace Reflex.Microsoft.Core
{
    internal class ResolverDescriptor
    {
        public Resolver Resolver { get; }
        public Type[] Contracts { get; }

        public ResolverDescriptor(Resolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }
    }
}