using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientTypeResolver : Resolver
    {
        public TransientTypeResolver(Type concrete) : base(concrete, Lifetime.Transient)
        {
            RegisterCallSite();
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            var instance = container.Construct(ConcreteType);
            Disposables.TryAdd(instance);
            RegisterInstance(instance);
            return instance;
        }
    }
}