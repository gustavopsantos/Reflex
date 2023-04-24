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

            var prefabScope = instance.GetComponent<PrefabScope>();

            if (prefabScope != null)
            {
                var prefabContainer = container.Scope($"{instance.name} ({instance.GetInstanceID()})", builder =>
                {
                    prefabScope.InstallBindings(builder);
                });

                foreach (var injectable in instance.GetInjectables(MonoInjectionMode.Recursive))
                {
                    AttributeInjector.Inject(injectable, prefabContainer);
                }
            }
            else
            {
                foreach (var injectable in instance.GetInjectables(injectionMode))
                {
                    AttributeInjector.Inject(injectable, container);
                }
            }

            return instance;
        }
    }
}