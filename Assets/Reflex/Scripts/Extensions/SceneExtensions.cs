using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex
{
    internal static class SceneExtensions
    {
        internal static IEnumerable<T> All<T>(this Scene scene)
        {
            return scene
                .GetRootGameObjects()
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<T>(true))
                .Where(m => m != null); // Skip missing scripts
        }
        
        internal static bool TryFindAtRootObjects<T>(this Scene scene, out T finding)
        {
            var roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                if (root.TryGetComponent<T>(out finding))
                {
                    return true;
                }
            }

            finding = default;
            return false;
        }
        
        internal static IDisposable OnUnload(this Scene scene, Action callback)
        {
            var gameObject = new GameObject("SceneUnloadHook");
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            var hook = gameObject.AddComponent<MonoBehaviourEventHook>();
            return hook.OnDestroyEvent.Subscribe(() => callback?.Invoke());
        }
    }
}