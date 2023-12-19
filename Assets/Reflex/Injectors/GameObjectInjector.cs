using Reflex.Core;
using Reflex.Enums;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Injectors
{
    internal static class GameObjectInjector
    {
        internal static void Inject(GameObject gameObject, Container container, MonoInjectionMode injectionMode)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetInjectables(injectionMode, monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    AttributeInjector.Inject(monoBehaviour, container);
                }
            }
        }
    }
}