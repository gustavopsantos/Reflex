using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    public static class ReflexSceneManager
    {
        [PublicAPI]
        public static void LoadScene(string sceneName, LoadSceneMode mode, Action<IServiceCollection> builder = null)
        {
			Scene scene = SceneManager.LoadScene(sceneName, new LoadSceneParameters(mode));
            UnityInjector.Extensions.Add(scene, builder);
        }
    }
}