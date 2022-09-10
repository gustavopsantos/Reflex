using System;
using Reflex.Injectors;
using Reflex.Scripts;

namespace Reflex
{
    internal class TransientResolver : Resolver
    {
        private readonly Type _concrete;

        public TransientResolver(Type concrete)
        {
            _concrete = concrete;
        }
        
        internal override object Resolve(IContainer container)
        {
            return ConstructorInjector.ConstructAndInject(_concrete, container);
        }
    }
}