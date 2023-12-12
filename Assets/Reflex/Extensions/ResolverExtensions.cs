using System.Runtime.CompilerServices;
using Reflex.Resolvers;

namespace Reflex.Extensions
{
    internal static class ResolverExtensions
    {
        private static readonly ConditionalWeakTable<IResolver, ResolverDebugProperties> _registry = new(); 
        
        public static ResolverDebugProperties GetDebugProperties(this IResolver resolver)
        {
            return _registry.GetOrCreateValue(resolver);
        }
    }
}