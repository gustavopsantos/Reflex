using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientValueResolver : Resolver
    {
        private object _value;

        public TransientValueResolver(object value) : base(value.GetType(), Lifetime.Transient)
        {
            RegisterCallSite();
            RegisterInstance(value);
            _value = value;
            Disposables.TryAdd(value);
        }

        public override object Resolve(Container container)
        {
            IncrementResolutions();

            if (_value == null)
            {
                throw new Exception("Trying to resolve a second time from a TransientValueResolver");
            }
            
            var value = _value;
            _value = null;
            ClearInstances();
            return value;
        }
    }
}