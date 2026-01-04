using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : IResolver
    {
        private object _instance;
        private readonly Type _concreteType;
        public Lifetime Lifetime => Lifetime.Singleton;
        public Container DeclaringContainer { get; set; }
        public Resolution Resolution { get; }

        public SingletonTypeResolver(Type concreteType, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
            Resolution = resolution;
        }

        public object Resolve(Container resolvingContainer)
        {
            Diagnosis.IncrementResolutions(this);

            if (_instance == null)
            {
                _instance = DeclaringContainer.Construct(_concreteType);
                DeclaringContainer.Disposables.TryAdd(_instance);
                Diagnosis.RegisterInstance(this, _instance);
            }

            return _instance;
        }

        public void Dispose()
        {
        }
    }
}