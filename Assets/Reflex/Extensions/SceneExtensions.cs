using Reflex.Core;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Extensions
{
    public static class SceneExtensions
    {
        public static Container GetSceneContainer(this Scene scene)
        {
            return UnityInjector.ContainersPerScene[scene];
        }
    }
}