using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reflex.Configuration;
using Reflex.Core;
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
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeAwakeOfFirstSceneOnly()
        {
            ReportReflexDebuggerStatus();
            ResetStaticState();
            Container.ProjectContainer = CreateProjectContainer();

            void InjectScene(Scene scene, SceneScope sceneScope)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
                var sceneContainer = CreateSceneContainer(scene, Container.ProjectContainer, sceneScope);
                ContainersPerScene.Add(scene, sceneContainer);
                SceneInjector.Inject(scene, sceneContainer);
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

            return builder.Build();
        }

        private static Container CreateSceneContainer(Scene scene, Container projectContainer, SceneScope sceneScope)
        {
            return projectContainer.Scope(builder =>
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
            SceneScope.OnSceneContainerBuilding = null;
            Container.ProjectContainer = null;
            Container.RootContainers.Clear();
            ContainersPerScene.Clear();
        }

        [Conditional("REFLEX_DEBUG")]
        private static void ReportReflexDebuggerStatus()
        {
            ReflexLogger.Log("Symbol REFLEX_DEBUG are defined, performance impacted!", LogLevel.Warning);
        }
    }
}
