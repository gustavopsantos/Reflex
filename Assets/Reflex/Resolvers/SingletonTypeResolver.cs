using System;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : IResolver
    {
        private object _instance;
        private readonly Type _concreteType;
        private readonly DisposableCollection _disposables = new();
        public Lifetime Lifetime => Lifetime.Singleton;

        public SingletonTypeResolver(Type concreteType)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (_instance == null)
            {
                _instance = container.Construct(_concreteType);
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