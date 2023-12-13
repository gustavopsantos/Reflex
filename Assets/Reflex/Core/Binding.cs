using System;
using Reflex.Resolvers;

namespace Reflex.Core
{
    internal class Binding
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }

        public Binding(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }
    }
}