using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class TransientResolver : Resolver
    {
        public TransientResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public override object Resolve(Container container)
        {
            Resolutions++;
            var instance = container.Construct(Concrete);
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}