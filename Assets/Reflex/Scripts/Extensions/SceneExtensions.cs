using System;
using Reflex.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex
{
    internal static class SceneExtensions
    {
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

        internal static void OnUnload(this Scene scene, Action callback)
        {
            var gameObject = new GameObject("SceneUnloadHook");
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            var hook = gameObject.AddComponent<MonoBehaviourEventHook>();
            hook.OnDestroyEvent += () => callback?.Invoke();
        }
    }
}