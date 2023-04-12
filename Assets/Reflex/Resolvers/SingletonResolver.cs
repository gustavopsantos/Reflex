using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class SingletonResolver : Resolver
    {
        private object _instance;

        public SingletonResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public override object Resolve(Container container)
        {
            Resolutions++;

            if (_instance == null)
            {
                _instance = container.Construct(Concrete);
                Disposables.TryAdd(_instance);
            }

            return _instance;
        }
    }
}