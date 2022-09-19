using System;
using Reflex.Injectors;

namespace Reflex
{
    internal class TransientResolver : IResolver
    {
        public Type Concrete { get; }
        public int Resolutions { get; private set; }

        public TransientResolver(Type concrete)
        {
            Concrete = concrete;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            return ConstructorInjector.ConstructAndInject(Concrete, container);
        }
    }
}