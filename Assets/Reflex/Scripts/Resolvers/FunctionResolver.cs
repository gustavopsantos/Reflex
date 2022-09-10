using System;
using Reflex.Scripts;

namespace Reflex
{
    internal class FunctionResolver : Resolver
    {
        private readonly Func<object> _function;

        public FunctionResolver(Func<object> function)
        {
            _function = function;
        }
        
        internal override object Resolve(IContainer container)
        {
            return _function.Invoke();
        }
    }
}