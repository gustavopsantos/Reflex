using System;
using System.Diagnostics;
using System.Linq;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Extensions;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal abstract class Resolver : IDisposable
    {
        protected readonly DisposableCollection Disposables = new();

        public Type Concrete { get; protected set; }
        public Lifetime Lifetime { get; protected set; }

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
        protected void RegisterCallSite()
        {
            var stackTrace = new StackTrace(3, true);
            var frames = stackTrace.GetFrames();

            foreach (var frame in frames.Where(f => f.GetFileName() != null))
            {
                var methodName = frame.GetMethod()?.Name;
                var className = frame.GetMethod()?.DeclaringType?.FullName;
                var lineNumber = frame.GetFileLineNumber();
                var filePath = UnityPathUtilities.GetUnityPath(frame.GetFileName());
                this.GetDebugProperties().Callsite.Add(new CallSite(className, methodName, filePath, lineNumber));
            }
        }
    }
}