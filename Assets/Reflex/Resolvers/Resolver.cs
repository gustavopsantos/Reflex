using System;
using Reflex.Core;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal abstract class Resolver : IDisposable
    {
        protected readonly DisposableCollection Disposables = new();

        public Type Concrete { get; protected set; }
        public int Resolutions { get; protected set; }

        public abstract object Resolve(IServiceProvider serviceProvider);

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}