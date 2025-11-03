using System;
using System.Runtime.CompilerServices;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class ScopedTypeResolver : IResolver
    {
        private readonly Type _concreteType;
        private readonly ConditionalWeakTable<Container, object> _instances = new();
        public Lifetime Lifetime => Lifetime.Scoped;

        public ScopedTypeResolver(Type concreteType)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
        }

        public object Resolve(Container resolvingContainer)
        {
            Diagnosis.IncrementResolutions(this);

            if (!_instances.TryGetValue(resolvingContainer, out var instance))
            {
                instance = resolvingContainer.Construct(_concreteType);
                _instances.Add(resolvingContainer, instance);
                resolvingContainer.Disposables.TryAdd(instance);
                Diagnosis.RegisterInstance(this, instance);
            }
            
            return instance;
        }

        public void Dispose()
        {
        }
    }
}