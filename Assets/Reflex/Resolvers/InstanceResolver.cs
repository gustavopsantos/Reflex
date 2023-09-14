using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
    internal sealed class InstanceResolver : Resolver
    {
        private readonly object _value;

        public InstanceResolver(object value)
        {
            _value = value;
            Disposables.TryAdd(value);
            Concrete = _value.GetType();
        }

        public override object Resolve(IServiceProvider serviceProvider)
        {
            Resolutions++;
            return _value;
        }
    }
}