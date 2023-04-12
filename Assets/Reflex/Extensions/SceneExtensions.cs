using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Reflex.Extensions
{
    internal static class SceneExtensions
    {
        internal static bool TryFindAtRoot<T>(this Scene scene, out T finding)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                if (gameObject.TryGetComponent<T>(out finding))
                {
                    return true;
                }
            }

            finding = default;
            return false;
        }
        
        internal static IEnumerable<T> All<T>(this Scene scene)
        {
            return scene
                .GetRootGameObjects()
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<T>(true))
                .Where(m => m != null); // Skip missing scripts
        }
    }
}