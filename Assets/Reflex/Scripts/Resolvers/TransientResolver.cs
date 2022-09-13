using System;
using Reflex.Injectors;

namespace Reflex
{
    internal class TransientResolver : IResolver
    {
        private readonly Type _concrete;

        public TransientResolver(Type concrete)
        {
            _concrete = concrete;
        }
        
        public object Resolve(Container container)
        {
            return ConstructorInjector.ConstructAndInject(_concrete, container);
        }
    }
}