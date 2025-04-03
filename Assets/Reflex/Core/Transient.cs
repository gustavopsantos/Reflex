using System;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public class Transient : IBinding
    {
        public Type Concrete { get; }
        public Type[] Contracts { get; }
        public IResolver Resolver { get; }
        public Resolution Resolution { get; }

        private Transient()
        {
        }
        
        private Transient(Type concrete, Type[] contracts, IResolver resolver)
        {
            IBinding.Validate(concrete, contracts);
            Concrete = concrete;
            Contracts = contracts;
            Resolver = resolver;
            Resolution = Resolution.Lazy;
        }
        
        public static Transient FromType(Type type)
        {
            return FromType(type, new[] { type });
        }
        
        public static Transient FromType(Type type, Type[] contracts)
        {
            var resolver = new TransientTypeResolver(type);
            return new Transient(type, contracts, resolver);
        }
        
        public static Transient FromFactory<T>(Func<Container, T> factory)
        {
            return FromFactory<T>(factory, new[] { typeof(T) });
        }
        
        public static Transient FromFactory<T>(Func<Container, T> factory, Type[] contracts)
        {
            var resolver = new TransientFactoryResolver(Proxy);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
            
            return new Transient(typeof(T), contracts, resolver);
        }
    }
}