using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Reflex.Extensions
{
    public static class SceneExtensions
    {
        public static Container GetSceneContainer(this Scene scene)
        {
            return UnityInjector.ContainersPerScene[scene];
        }

        internal static bool TryFindAtRoot<T>(this Scene scene, out T finding)
        {
            using var pooledObject = ListPool<GameObject>.Get(out var rootGameObjects);
            scene.GetRootGameObjects(rootGameObjects);

            for (var i = 0; i < rootGameObjects.Count; i++)
            {
                if (rootGameObjects[i].TryGetComponent<T>(out finding))
                {
                    return true;
                }
            }

            finding = default;
            return false;
        }
    }
}