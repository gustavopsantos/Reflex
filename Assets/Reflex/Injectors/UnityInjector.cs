using System;
using System.Collections.Generic;
using System.Diagnostics;
using Reflex.Configuration;
using Reflex.Core;
using Reflex.Exceptions;
using Reflex.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Injectors
{
    internal static class UnityInjector
    {
        internal static Action<Scene, ContainerScope> OnSceneLoaded;
        internal static Dictionary<Scene, Container> ContainersPerScene { get; } = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded()
        {
            ReportReflexDebuggerStatus();
            ResetStaticState();

            void InjectScene(Scene scene, ContainerScope containerScope)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
                var sceneContainer = CreateSceneContainer(scene, containerScope);

                if (ContainersPerScene.TryAdd(scene, sceneContainer))
                {
                    SceneInjector.Inject(scene, sceneContainer);
                }
                else
                {
                    throw new SceneHasMultipleSceneScopesException(scene);
                }
            }
            
            void DisposeScene(Scene scene)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) unloaded", LogLevel.Development);

                if (ContainersPerScene.Remove(scene, out var sceneContainer)) // Not all scenes has containers
                {
                    sceneContainer.Dispose();
                }
            }
            
            void DisposeProject()
            {
                Container.RootContainer?.Dispose();
                Container.RootContainer = null;
                
                // Unsubscribe from static events ensuring that Reflex works with domain reloading set to false
                OnSceneLoaded -= InjectScene;
                SceneManager.sceneUnloaded -= DisposeScene;
                Application.quitting -= DisposeProject;
            }
            
            OnSceneLoaded += InjectScene;
            SceneManager.sceneUnloaded += DisposeScene;
            Application.quitting += DisposeProject;
        }

        private static Container CreateRootContainer()
        {
            var reflexSettings = ReflexSettings.Instance;
            var builder = new ContainerBuilder().SetName("RootContainer");

            if (reflexSettings.RootScope != null)
            {
                reflexSettings.RootScope.InstallBindings(builder);
                ReflexLogger.Log("Root Bindings Installed", LogLevel.Info, reflexSettings.RootScope.gameObject);
            }
            
            ContainerScope.OnRootContainerBuilding?.Invoke(builder);
            return builder.Build();
        }

        private static Container CreateSceneContainer(Scene scene, ContainerScope containerScope)
        {
            if (Container.RootContainer == null)
            {
                Container.RootContainer = CreateRootContainer();
            }
            
            return Container.RootContainer.Scope(builder =>
            {
                builder.SetName($"{scene.name} ({scene.GetHashCode()})");
                containerScope.InstallBindings(builder);
                ContainerScope.OnSceneContainerBuilding?.Invoke(scene, builder);
                ReflexLogger.Log($"Scene ({scene.name}) Bindings Installed", LogLevel.Info, containerScope.gameObject);
            });
        }

        /// <summary>
        /// Ensure static state is reset.
        /// This is only required when playing from editor when ProjectSettings > Editor > Reload Domain is set to false. 
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        private static void ResetStaticState()
        {
            OnSceneLoaded = null;
            Container.RootContainer = null;
            ContainersPerScene.Clear();
            ContainerScope.OnRootContainerBuilding = null;
            ContainerScope.OnSceneContainerBuilding = null;
#if UNITY_EDITOR
            Container.RootContainers.Clear();
#endif
        }

        [Conditional("REFLEX_DEBUG")]
        private static void ReportReflexDebuggerStatus()
        {
            ReflexLogger.Log("Symbol REFLEX_DEBUG are defined, performance impacted!", LogLevel.Warning);
        }
    }
}
