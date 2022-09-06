using System;

namespace Reflex
{
    internal class FunctionResolver : Resolver
    {
        private readonly Func<object> _function;

        public FunctionResolver(Func<object> function)
        {
            _function = function;
        }
        
        internal override object Resolve(Type contract, Container container)
        {
            return _function.Invoke();
        }
    }
}