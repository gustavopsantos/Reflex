using System;
using System.Diagnostics;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Extensions;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal abstract class Resolver : IDisposable
    {
        protected readonly DisposableCollection Disposables = new();

        public Lifetime Lifetime { get; }

        protected Resolver(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }

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
        
        [Conditional("REFLEX_DEBUG")]
        protected void RegisterInstance(object instance)
        {
            this.GetDebugProperties().Instances.Add((instance, Diagnosis.GetCallSite(3)));
        }
        
        [Conditional("REFLEX_DEBUG")]
        protected void ClearInstances()
        {
            this.GetDebugProperties().Instances.Clear();
        }

        [Conditional("REFLEX_DEBUG")]
        protected void RegisterCallSite()
        {
            this.GetDebugProperties().BindingCallsite.AddRange(Diagnosis.GetCallSite(4));
        }
    }
}