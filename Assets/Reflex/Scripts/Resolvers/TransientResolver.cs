using System;
using Reflex.Injectors;

namespace Reflex
{
    internal class TransientResolver : IResolver
    {
        private readonly Type _concrete;
        public int Resolutions { get; private set; }

        public TransientResolver(Type concrete)
        {
            _concrete = concrete;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            return ConstructorInjector.ConstructAndInject(_concrete, container);
        }
    }
}