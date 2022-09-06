using System;
using Reflex.Injectors;

namespace Reflex
{
    internal class TransientResolver : Resolver
    {
        private readonly Type _concrete;

        public TransientResolver(Type concrete)
        {
            _concrete = concrete;
        }
        
        internal override object Resolve(Type contract, Container container)
        {
            return ConstructorInjector.ConstructAndInject(_concrete, container);
        }
    }
}