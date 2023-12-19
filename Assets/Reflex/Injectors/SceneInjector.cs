using Reflex.Core;
using Reflex.Enums;
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

            scene.GetRootGameObjects(rootGameObjects);

            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                GameObjectInjector.Inject(rootGameObjects[i], container, MonoInjectionMode.Recursive);
            }
        }
    }
}