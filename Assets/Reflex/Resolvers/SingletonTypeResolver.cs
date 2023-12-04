using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : Resolver
    {
        private object _instance;

        public SingletonTypeResolver(Type concrete)
        {
            RegisterCallSite();
            Concrete = concrete;
            Lifetime = Lifetime.Singleton;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();

            if (_instance == null)
            {
                _instance = container.Construct(Concrete);
                Disposables.TryAdd(_instance);
            }

            return _instance;
        }
    }
}