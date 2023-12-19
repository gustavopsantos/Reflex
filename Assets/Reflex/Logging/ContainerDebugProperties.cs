using System.Collections.Generic;

namespace Reflex.Logging
{
    public sealed class ContainerDebugProperties
    {
        public List<CallSite> BuildCallsite { get; } = new();
    }
}