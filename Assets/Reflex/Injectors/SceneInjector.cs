using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
    internal static class SceneInjector
    {
        public static void Inject(Scene scene, Container container)
        {
            foreach (var monoBehaviour in scene.All<MonoBehaviour>())
            {
                AttributeInjector.Inject(monoBehaviour, container);
            }
        }
    }
}