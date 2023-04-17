using System;
using JetBrains.Annotations;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    public static class ReflexSceneManager
    {
        [PublicAPI]
        public static void LoadScene(string sceneName, LoadSceneMode mode, Action<ContainerDescriptor> builder = null)
        {
            var scene = SceneManager.LoadScene(sceneName, new LoadSceneParameters(mode));
            UnityInjector.Extensions.Add(scene, builder);
        }
    }
}