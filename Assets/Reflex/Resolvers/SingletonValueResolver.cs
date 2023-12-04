using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonValueResolver : Resolver
    {
        private readonly object _value;

        public SingletonValueResolver(object value)
        {
            RegisterCallSite();
            _value = value;
            Disposables.TryAdd(value);
            Concrete = _value.GetType();
            Lifetime = Lifetime.Singleton;
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            return _value;
        }
    }
}