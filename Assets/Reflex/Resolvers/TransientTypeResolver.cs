using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class TransientTypeResolver : Resolver
    {
        public TransientTypeResolver(Type concrete)
        {
            Concrete = concrete;
            RegisterCallSite();
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