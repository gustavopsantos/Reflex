using Reflex.Core;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
    internal static class SceneInjector
    {
        public static void Inject(Scene scene, Container container)
        {
            using var pooledObject1 = ListPool<GameObject>.Get(out var rootGameObjects);
            using var pooledObject2 = ListPool<MonoBehaviour>.Get(out var monoBehaviours);

            scene.GetRootGameObjects(rootGameObjects);

            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                rootGameObjects[i].GetComponentsInChildren(includeInactive: true, monoBehaviours); // GetComponentsInChildren clears result list, so it needs to be consumed right after

                for (int j = 0; j < monoBehaviours.Count; j++)
                {
                    var monoBehaviour = monoBehaviours[j];

                    if (monoBehaviour != null)
                    {
                        AttributeInjector.Inject(monoBehaviour, container);
                    }
                }
            }
        }
    }
}