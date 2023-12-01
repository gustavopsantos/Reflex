using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class TransientFactoryResolver : Resolver
    {
        private readonly Func<Container, object> _factory;

        public TransientFactoryResolver(Func<Container, object> factory, Type concrete)
        {
            RegisterCallSite();
            _factory = factory;
            Concrete = concrete;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            var instance = _factory.Invoke(container);
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}