using System;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Extensions
{
    public static class SceneExtensions
    {
        public static Container GetSceneContainer(this Scene scene)
        {
            if (UnityInjector.ContainersPerScene.TryGetValue(scene, out var sceneContainer))
            {
                return sceneContainer;
            }

            throw new Exception($"Scene '{scene.name}' does not have a container, make sure it has a SceneScope component");
        }
    }
}