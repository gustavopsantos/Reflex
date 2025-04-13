using JetBrains.Annotations;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    public static class ReflexSceneManager
    {
        [PublicAPI]
        public static void OverrideSceneParentContainer(Scene scene, Container parent)
        {
            UnityInjector.SceneContainerParentOverride.Add(scene, parent);
        }
    }
}