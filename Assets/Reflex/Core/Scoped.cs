using System;
using Reflex.Enums;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public class Scoped : IBinding
    {
        public Type[] Contracts { get; }
        public IResolver Resolver { get; }
        public Resolution Resolution { get; }

        private Scoped()
        {
        }
        
        private Scoped(Type concrete, Type[] contracts, IResolver resolver, Resolution resolution)
        {
            IBinding.Validate(concrete, contracts);
            Contracts = contracts;
            Resolver = resolver;
            Resolution = resolution;
        }
        
        public static Scoped FromType(Type type, Type[] contracts, Resolution resolution)
        {
            var resolver = new ScopedTypeResolver(type, resolution);
            return new Scoped(type, contracts, resolver, resolution);
        }
        
        public static Scoped FromFactory<T>(Func<Container, T> factory, Type[] contracts, Resolution resolution)
        {
            var resolver = new ScopedFactoryResolver(Proxy, resolution);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
            
            return new Scoped(typeof(T), contracts, resolver, resolution);
        }
    }
}