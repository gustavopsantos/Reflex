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
    }
}