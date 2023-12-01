using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : Resolver
    {
        private object _instance;

        public SingletonTypeResolver(Type concrete)
        {
            Concrete = concrete;
            RegisterCallSite();
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