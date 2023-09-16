using System;
using Reflex.Microsoft.Core;

namespace Reflex.Microsoft.Resolvers
{
    internal sealed class SingletonResolver : Resolver
    {
        private object _instance;

        public SingletonResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public override object Resolve(IServiceProvider serviceProvider)
        {
            Resolutions++;

            if (_instance == null)
            {
                _instance = serviceProvider.GetService(Concrete);
                Disposables.TryAdd(_instance);
            }

            return _instance;
        }
    }
}