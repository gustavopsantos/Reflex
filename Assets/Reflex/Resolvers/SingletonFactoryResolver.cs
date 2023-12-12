using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonFactoryResolver : Resolver
    {
        private readonly Func<Container, object> _factory;
        private object _instance;

        public SingletonFactoryResolver(Func<Container, object> factory) : base(Lifetime.Singleton)
        {
            RegisterCallSite();
            _factory = factory;
        }
        
        public override object Resolve(Container container)
        {
            IncrementResolutions();
            
            if (_instance == null)
            {
                _instance = _factory.Invoke(container);
                Disposables.TryAdd(_instance);
                RegisterInstance(_instance);
            }

            return _instance;
        }
    }
}