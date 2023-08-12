using Reflex.Core;
using System;

namespace Reflex.Resolvers
{
    internal sealed class TransientResolver : Resolver
    {
        private readonly Func<object> _concreteConstructor;

        public TransientResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public TransientResolver(Func<object> concreteConstructor)
        {
            _concreteConstructor = concreteConstructor;
        }

        public override object Resolve(Container container)
        {
            Resolutions++;
            object instance;
            if (_concreteConstructor != null)
            {
                instance = _concreteConstructor();
            }
            else
            {
                instance = container.Construct(Concrete);
            }
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}
