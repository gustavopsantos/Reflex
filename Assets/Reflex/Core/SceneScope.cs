using System;
using Reflex.Injectors;
using Reflex.Logging;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class SceneScope : MonoBehaviour
    {
        public const int ExecutionOrder = -1_000_000_000;
        
        public static Action<Scene, ContainerBuilder> OnSceneContainerBuilding;
        
        private void Awake()
        {
            UnityInjector.OnSceneLoaded.Invoke(gameObject.scene, this);
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using var pooledObject = ListPool<IInstaller>.Get(out var installers);
            GetComponentsInChildren<IInstaller>(installers);
            
            for (var i = 0; i < installers.Count; i++)
            {
                installers[i].InstallBindings(containerBuilder);
            }

            ReflexLogger.Log($"SceneScope ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}