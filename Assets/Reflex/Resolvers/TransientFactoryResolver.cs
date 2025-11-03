using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientFactoryResolver : IResolver
    {
        private readonly Func<Container, object> _factory;
        public Lifetime Lifetime => Lifetime.Transient;

        public TransientFactoryResolver(Func<Container, object> factory)
        {
            Diagnosis.RegisterCallSite(this);
            _factory = factory;
        }

        public object Resolve(Container resolvingContainer)
        {
            Diagnosis.IncrementResolutions(this);
            var instance = _factory.Invoke(resolvingContainer);
            resolvingContainer.Disposables.TryAdd(instance);
            Diagnosis.RegisterInstance(this, instance);
            return instance;
        }

        public void Dispose()
        {
        }
    }
}