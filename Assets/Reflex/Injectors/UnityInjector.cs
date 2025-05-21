using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal static Action<Scene, ContainerScope> OnSceneLoaded;
        internal static Container RootContainer { get; private set; }
        internal static Dictionary<Scene, Container> ContainersPerScene { get; } = new();
        internal static Stack<Container> ContainerParentOverride { get; } = new();
        internal static Action<ContainerBuilder> ExtraInstallers;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeAwakeOfFirstSceneOnly()
        {
            ReportReflexDebuggerStatus();
            ResetStaticState();
            RootContainer = CreateRootContainer();

            void InjectScene(Scene scene, ContainerScope containerScope)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
                var sceneContainer = CreateSceneContainer(scene, RootContainer, containerScope);
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
                ProjectContainer?.Dispose();
                ProjectContainer = null;
                
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
            
            return builder.Build();
        }

        private static Container CreateSceneContainer(Scene scene, Container rootContainer, ContainerScope containerScope)
        {
            return rootContainer.Scope(builder =>
            {
                builder.SetName($"{scene.name} ({scene.GetHashCode()})");
                containerScope.InstallBindings(builder);
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
            RootContainer = null;
            ContainersPerScene.Clear();
            ContainerParentOverride.Clear();
            ExtraInstallers = null;
        }

        [Conditional("REFLEX_DEBUG")]
        private static void ReportReflexDebuggerStatus()
        {
            ReflexLogger.Log("Symbol REFLEX_DEBUG are defined, performance impacted!", LogLevel.Warning);
        }
    }
}
