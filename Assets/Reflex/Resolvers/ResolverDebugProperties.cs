using System.Collections.Generic;

namespace Reflex.Resolvers
{
    public sealed class ResolverDebugProperties
    {
        public int Resolutions;
        public List<(object, List<CallSite>)> Instances { get; } = new();
        public List<CallSite> BindingCallsite { get; } = new();
    }
}