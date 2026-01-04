using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class SingletonValueResolver : IResolver
    {
        private readonly object _value;
        public Lifetime Lifetime => Lifetime.Singleton;
        public Container DeclaringContainer { get; set; }
        public Resolution Resolution => Resolution.Lazy;

        public SingletonValueResolver(object value)
        {
            Diagnosis.RegisterCallSite(this);
            Diagnosis.RegisterInstance(this, value);
            _value = value;
        }

        public object Resolve(Container resolvingContainer)
        {
            Diagnosis.IncrementResolutions(this);
            return _value;
        }

        public void Dispose()
        {
            if (_value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}