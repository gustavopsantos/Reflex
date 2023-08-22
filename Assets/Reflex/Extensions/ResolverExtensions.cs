using System.Runtime.CompilerServices;
using Reflex.Resolvers;

namespace Reflex.Extensions
{
    internal static class ResolverExtensions
    {
        private static readonly ConditionalWeakTable<Resolver, ResolverDebugProperties> _registry = new(); 
        
        public static ResolverDebugProperties GetDebugProperties(this Resolver resolver)
        {
            return _registry.GetOrCreateValue(resolver);
        }
    }
}