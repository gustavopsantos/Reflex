using System;
using Reflex.Resolvers;

namespace Reflex.Core
{
    internal class BindingDescriptor
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }

        public BindingDescriptor(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }
    }
}