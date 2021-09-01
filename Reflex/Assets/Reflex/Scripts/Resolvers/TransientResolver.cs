using System;
using Reflex.Injectors;

namespace Reflex
{
    internal static class TransientResolver
    {
        internal static object Resolve(Type contract, Container container)
        {
            return ConstructorInjector.ConstructAndInject(container.GetConcreteTypeFor(contract), container);
        }
    }
}