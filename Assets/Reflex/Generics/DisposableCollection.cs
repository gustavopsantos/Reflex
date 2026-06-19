using System;
using System.Collections.Generic;

namespace Reflex.Generics
{
    internal sealed class DisposableCollection : IDisposable
    {
        private readonly Stack<IDisposable> _stack = new();
        private readonly bool _autoDisposeEnabled;

        public DisposableCollection(bool autoDisposeEnabled = true)
        {
            _autoDisposeEnabled = autoDisposeEnabled;
        }

        public void Add(IDisposable disposable)
        {
            if (!_autoDisposeEnabled)
            {
                return;
            }

            _stack.Push(disposable);
        }

        public void TryAdd(object obj)
        {
            if (!_autoDisposeEnabled)
            {
                return;
            }

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
