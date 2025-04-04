using System;
using Reflex.Enums;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public class Singleton : IBinding
    {
        public Type[] Contracts { get; }
        public IResolver Resolver { get; }
        public Resolution Resolution { get; }

        private Singleton()
        {
        }

        private Singleton(Type concrete, Type[] contracts, IResolver resolver, Resolution resolution)
        {
            IBinding.Validate(concrete, contracts);
            Contracts = contracts;
            Resolver = resolver;
            Resolution = resolution;
        }

        public static Singleton FromValue(object value, Type[] contracts)
        {
            var resolver = new SingletonValueResolver(value);
            var concrete = value.GetType();
            return new Singleton(concrete, contracts, resolver, Resolution.Lazy);
        }

        public static Singleton FromType(Type type, Type[] contracts, Resolution resolution)
        {
            var resolver = new SingletonTypeResolver(type, resolution);
            return new Singleton(type, contracts, resolver, resolution);
        }

        public static Singleton FromFactory<T>(Func<Container, T> factory, Type[] contracts, Resolution resolution)
        {
            var resolver = new SingletonFactoryResolver(Proxy, resolution);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }

            return new Singleton(typeof(T), contracts, resolver, resolution);
        }
    }
}