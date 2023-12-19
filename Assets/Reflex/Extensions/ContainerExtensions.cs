using System.Runtime.CompilerServices;
using Reflex.Core;
using Reflex.Logging;

namespace Reflex.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly ConditionalWeakTable<Container, ContainerDebugProperties> _containerDebugProperties = new();

        internal static ContainerDebugProperties GetDebugProperties(this Container container)
        {
            return _containerDebugProperties.GetOrCreateValue(container);
        }
    }
}