using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : Resolver
    {
        private object _instance;

        public SingletonTypeResolver(Type concrete) : base(concrete, Lifetime.Singleton)
        {
            RegisterCallSite();
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();

            if (_instance == null)
            {
                _instance = container.Construct(ConcreteType);
                Disposables.TryAdd(_instance);
                RegisterInstance(_instance);
            }

            return _instance;
        }
    }
}