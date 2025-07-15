using System;
using Reflex.Injectors;

namespace Reflex.Core
{
    public class ParentOverrideScope : IDisposable
    {
        private readonly Container _parentOverride;

        public ParentOverrideScope(Container parentOverride)
        {
            _parentOverride = parentOverride;
            UnityInjector.ContainerParentOverride.Push(_parentOverride);
        }

        public void Dispose()
        {
            if (UnityInjector.ContainerParentOverride.TryPop(out var popped) && popped == _parentOverride)
            {
                // All good, we popped the correct parent override
            }
            else
            {
                throw new InvalidOperationException("ParentOverrideScope was not disposed in the correct order.");
            }
        }
    }
}