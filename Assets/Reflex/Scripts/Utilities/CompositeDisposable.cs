using System;
using System.Collections.Generic;

namespace Reflex.Scripts.Utilities
{
    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _registry = new List<IDisposable>();

        public void Add(IDisposable disposable)
        {
            _registry.Add(disposable);
        }
        
        public void TryAdd(object obj)
        {
            if (obj is IDisposable disposable)
            {
                Add(disposable);
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _registry)
            {
                disposable.Dispose();
            }
        }
    }
}