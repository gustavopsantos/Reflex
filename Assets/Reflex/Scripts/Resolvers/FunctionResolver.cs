using System;

namespace Reflex
{
    internal class FunctionResolver : IResolver
    {
        private readonly Func<object> _function;
        public int Resolutions { get; private set; }

        public FunctionResolver(Func<object> function)
        {
            _function = function;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            return _function.Invoke();
        }
    }
}