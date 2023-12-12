using System;
using Reflex.Resolvers;

namespace Reflex.Core
{
    internal class ResolverDescriptor
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }

        public ResolverDescriptor(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }
    }
}