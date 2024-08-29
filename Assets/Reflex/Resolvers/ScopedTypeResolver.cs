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

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (!_instances.TryGetValue(container, out var instance))
            {
                instance = container.Construct(_concreteType);
                _instances.Add(container, instance);
                container.Disposables.TryAdd(instance);
                Diagnosis.RegisterInstance(this, instance);
            }
            
            return instance;
        }

        public void Dispose()
        {
        }
    }
}