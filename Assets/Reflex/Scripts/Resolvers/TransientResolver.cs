using System;
using Reflex.Injectors;
using Reflex.Scripts;

namespace Reflex
{
    internal class TransientResolver : IResolver
    {
        private readonly Type _concrete;

        public TransientResolver(Type concrete)
        {
            _concrete = concrete;
        }
        
        public object Resolve(IContainer container)
        {
            return ConstructorInjector.ConstructAndInject(_concrete, container);
        }
    }
}