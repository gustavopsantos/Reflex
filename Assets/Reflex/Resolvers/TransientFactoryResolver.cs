using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientFactoryResolver : Resolver
    {
        private readonly Func<Container, object> _factory;

        public TransientFactoryResolver(Func<Container, object> factory, Type concrete) : base(concrete, Lifetime.Transient)
        {
            RegisterCallSite();
            _factory = factory;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            var instance = _factory.Invoke(container);
            Disposables.TryAdd(instance);
            RegisterInstance(instance);
            return instance;
        }
    }
}