using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class SingletonValueResolver : Resolver
    {
        private readonly object _value;

        public SingletonValueResolver(object value)
        {
            _value = value;
            Disposables.TryAdd(value);
            Concrete = _value.GetType();
            RegisterCallSite();
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();
            return _value;
        }
    }
}