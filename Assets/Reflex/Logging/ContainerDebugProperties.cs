using System.Collections.Generic;

namespace Reflex.Logging
{
    public class ContainerDebugProperties
    {
        public List<CallSite> BuildCallsite { get; } = new();
    }
}