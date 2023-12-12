using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : Resolver
    {
        private object _instance;
        private readonly Type _concreteType;

        public SingletonTypeResolver(Type concreteType) : base(Lifetime.Singleton)
        {
            RegisterCallSite();
            _concreteType = concreteType;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();

            if (_instance == null)
            {
                _instance = container.Construct(_concreteType);
                Disposables.TryAdd(_instance);
                RegisterInstance(_instance);
            }

            return _instance;
        }
    }
}