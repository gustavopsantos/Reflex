using System;

namespace Reflex
{
    internal static class SingletonResolver
    {
        internal static object Resolve(Type contract, Container container)
        {
            if (container.TryGetSingletonInstance(contract, out var instance))
            {
                return instance;
            }

            return CreateAndRegisterSingletonInstance(contract, container);
        }

        private static object CreateAndRegisterSingletonInstance(Type contract, Container container)
        {
            var instance = TransientResolver.Resolve(contract, container);
            return container.RegisterSingletonInstance(contract, instance);
        }
    }
}