using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientTypeResolver : Resolver
    {
        private readonly Type _concreteType;

        public TransientTypeResolver(Type concreteType) : base(Lifetime.Transient)
        {
            RegisterCallSite();
            _concreteType = concreteType;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            var instance = container.Construct(_concreteType);
            Disposables.TryAdd(instance);
            RegisterInstance(instance);
            return instance;
        }
    }
}