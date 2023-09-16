using System;
using Reflex.Microsoft.Core;

namespace Reflex.Microsoft.Resolvers
{
    internal sealed class TransientResolver : Resolver
    {
        public TransientResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public override object Resolve(IServiceProvider serviceProvider)
        {
            Resolutions++;
			object instance = serviceProvider.GetService(Concrete);
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}