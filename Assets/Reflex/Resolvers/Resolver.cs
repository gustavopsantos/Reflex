using System;
using System.Diagnostics;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal abstract class Resolver : IDisposable
    {
        protected readonly DisposableCollection Disposables = new();

        public Type Concrete { get; protected set; }

        public abstract object Resolve(Container container);

        public void Dispose()
        {
            Disposables.Dispose();
        }
        
        [Conditional("REFLEX_DEBUG")]
        protected void IncrementResolutions()
        {
            this.GetDebugProperties().Resolutions++;
        }
    }
}