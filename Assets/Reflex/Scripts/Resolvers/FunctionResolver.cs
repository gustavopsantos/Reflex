using System;

namespace Reflex
{
    internal class FunctionResolver : IResolver
    {
        private readonly Func<object> _function;

        public FunctionResolver(Func<object> function)
        {
            _function = function;
        }

        public object Resolve(Container container)
        {
            return _function.Invoke();
        }
    }
}