using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonValueResolver : Resolver
    {
        private readonly object _value;

        public SingletonValueResolver(object value) : base(value.GetType(), Lifetime.Singleton)
        {
            RegisterCallSite();
            RegisterInstance(value);
            _value = value;
            Disposables.TryAdd(value);
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            return _value;
        }
    }
}