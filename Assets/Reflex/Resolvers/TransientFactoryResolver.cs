using System;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal sealed class TransientFactoryResolver : IResolver
    {
        private readonly Func<Container, object> _factory;
        private readonly DisposableCollection _disposables = new();
        public Lifetime Lifetime => Lifetime.Transient;

        public TransientFactoryResolver(Func<Container, object> factory)
        {
            Diagnosis.RegisterCallSite(this);
            _factory = factory;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);
            var instance = _factory.Invoke(container);
            _disposables.TryAdd(instance);
            Diagnosis.RegisterInstance(this, instance);
            return instance;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}