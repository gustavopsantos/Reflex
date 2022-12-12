using System;
using System.Collections.Generic;
using Reflex.Injectors;

namespace Reflex
{
    internal class TransientResolver : IResolver
    {
        public Type Concrete { get; }
        public int Resolutions { get; private set; }
        private readonly Stack<object> _instances = new Stack<object>();

        public TransientResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            var instance = ConstructorInjector.ConstructAndInject(Concrete, container);
            _instances.Push(instance);
            return instance;
        }

        public void Dispose()
        {
            while (_instances.Count > 0)
            {
                var instance = _instances.Pop();
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}