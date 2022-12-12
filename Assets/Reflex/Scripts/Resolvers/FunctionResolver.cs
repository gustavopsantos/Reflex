using System;

namespace Reflex
{
    internal class FunctionResolver : IResolver
    {
        public Type Concrete { get; }
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

        public void Dispose()
        {
            // Objects created by user functions, should be disposed by users
        }
    }
}