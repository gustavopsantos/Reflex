using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonFactoryResolver : Resolver
    {
        private readonly Func<Container, object> _factory;
        private object _instance;

        public SingletonFactoryResolver(Func<Container, object> factory, Type concrete)
        {
            RegisterCallSite();
            _factory = factory;
            Concrete = concrete;
            Lifetime = Lifetime.Singleton;
        }
        
        public override object Resolve(Container container)
        {
            IncrementResolutions();
            
            if (_instance == null)
            {
                _instance = _factory.Invoke(container);
                Disposables.TryAdd(_instance);
            }

            return _instance;
        }
    }
}