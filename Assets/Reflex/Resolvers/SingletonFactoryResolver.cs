using System;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal sealed class SingletonFactoryResolver : IResolver
    {
        private object _instance;
        private readonly Func<Container, object> _factory;
        private readonly DisposableCollection _disposables = new();

        public Lifetime Lifetime => Lifetime.Singleton;

        public SingletonFactoryResolver(Func<Container, object> factory)
        {
            Diagnosis.RegisterCallSite(this);
            _factory = factory;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (_instance == null)
            {
                _instance = _factory.Invoke(container);
                _disposables.TryAdd(_instance);
                Diagnosis.RegisterInstance(this, _instance);
            }

            return _instance;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}