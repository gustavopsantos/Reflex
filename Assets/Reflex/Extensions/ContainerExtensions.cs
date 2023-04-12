using Reflex.Core;
using Reflex.Enums;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Extensions
{
    public static class ContainerExtensions
    {
        public static T Instantiate<T>(this Container container, T original,
            MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) where T : Component
        {
            var instance = Object.Instantiate(original);

            foreach (var injectable in instance.GetInjectables(injectionMode))
            {
                AttributeInjector.Inject(injectable, container);
            }

            return instance;
        }
    }
}