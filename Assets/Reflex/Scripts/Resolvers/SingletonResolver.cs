using System;

namespace Reflex
{
    internal class SingletonResolver : Resolver
    {
        private readonly Type _concrete;

        public SingletonResolver(Type concrete)
        {
            _concrete = concrete;
        }
        
        internal override object Resolve(Type contract, Container container)
        {
            if (container.Singletons.TryGetValue(contract, out var instance))
            {
                return instance;
            }

            return CreateAndRegisterSingletonInstance(contract, container);
        }

        private object CreateAndRegisterSingletonInstance(Type contract, Container container)
        {
            var instance = container.Construct(_concrete);
            return container.RegisterSingletonInstance(contract, instance);
        }
    }
}