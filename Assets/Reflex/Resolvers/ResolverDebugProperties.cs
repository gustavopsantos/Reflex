using System.Collections.Generic;

namespace Reflex.Resolvers
{
    public class ResolverDebugProperties
    {
        public int Resolutions;
        public List<CallSite> Callsite { get; } = new();
    }
}