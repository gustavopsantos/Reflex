using System;
using System.Collections.Generic;

namespace Reflex.Generics
{
    internal sealed class DisposableCollection : IDisposable
    {
        private readonly Stack<IDisposable> _stack = new();

        public void Add(IDisposable disposable)
        {
            _stack.Push(disposable);
        }

        public void TryAdd(object obj)
        {
            if (obj is IDisposable disposable)
            {
                _stack.Push(disposable);
            }
        }

        public void Dispose()
        {    
            List<Exception> exceptions = null;
            while (_stack.TryPop(out var disposable))
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    exceptions ??= new();
                    exceptions.Add(e);
                }
            }

            if (exceptions is not null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
