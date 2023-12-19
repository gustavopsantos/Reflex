using System.Runtime.CompilerServices;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Injectors;
using Reflex.Logging;
using UnityEngine;

namespace Reflex.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly ConditionalWeakTable<Container, ContainerDebugProperties> _containerDebugProperties = new(); 
        
        internal static ContainerDebugProperties GetDebugProperties(this Container container)
        {
            return _containerDebugProperties.GetOrCreateValue(container);
        }
        
        public static T Instantiate<T>(this Container container, T original, MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) where T : Component
        {
            var instance = Object.Instantiate(original);
            GameObjectInjector.Inject(instance.gameObject, container, injectionMode);
            return instance;
        }
    }
}