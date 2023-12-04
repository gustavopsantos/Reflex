using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientTypeResolver : Resolver
    {
        public TransientTypeResolver(Type concrete)
        {
            RegisterCallSite();
            Concrete = concrete;
            Lifetime = Lifetime.Transient;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            var instance = container.Construct(Concrete);
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}