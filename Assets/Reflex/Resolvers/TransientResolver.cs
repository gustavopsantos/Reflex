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
            if (_concreteConstructor is null)
            {
                instance = container.Construct(Concrete);
            }
            else
            {
                instance = _concreteConstructor();
            }
            Disposables.TryAdd(instance);
            return instance;
        }
    }
}
