using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        internal static Action<Scene, SceneScope> OnSceneLoaded;
        internal static Dictionary<Scene, Container> ContainersPerScene { get; } = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded()
        {
            ReportReflexDebuggerStatus();
            ResetStaticState();

            void InjectScene(Scene scene, SceneScope sceneScope)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
                var sceneContainer = CreateSceneContainer(scene, sceneScope);

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
                Container.ProjectContainer?.Dispose();
                Container.ProjectContainer = null;
                
                // Unsubscribe from static events ensuring that Reflex works with domain reloading set to false
                OnSceneLoaded -= InjectScene;
                SceneManager.sceneUnloaded -= DisposeScene;
                Application.quitting -= DisposeProject;
            }
            
            OnSceneLoaded += InjectScene;
            SceneManager.sceneUnloaded += DisposeScene;
            Application.quitting += DisposeProject;
        }

        private static Container CreateProjectContainer()
        {
            var reflexSettings = ReflexSettings.Instance;
            var builder = new ContainerBuilder().SetName("ProjectContainer");

            if (reflexSettings.ProjectScopes != null)
            {
                foreach (var projectScope in reflexSettings.ProjectScopes.Where(x => x != null && x.gameObject.activeSelf))
                {
                    projectScope.InstallBindings(builder);
                }
            }
            
            ProjectScope.OnRootContainerBuilding?.Invoke(builder);
            return builder.Build();
        }

        private static Container CreateSceneContainer(Scene scene, SceneScope sceneScope)
        {
            if (Container.ProjectContainer == null)
            {
                Container.ProjectContainer = CreateProjectContainer();
            }
            
            return Container.ProjectContainer.Scope(builder =>
            {
                builder.SetName($"{scene.name} ({scene.GetHashCode()})");
                sceneScope.InstallBindings(builder);
                SceneScope.OnSceneContainerBuilding?.Invoke(scene, builder);
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
            Container.ProjectContainer = null;
            SceneScope.OnSceneContainerBuilding = null;
            ProjectScope.OnRootContainerBuilding = null;
            ContainersPerScene.Clear();
            
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
