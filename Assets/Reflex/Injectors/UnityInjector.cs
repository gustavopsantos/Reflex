using System;
using System.Collections.Generic;
using System.Diagnostics;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Logging;
using Reflex.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Injectors
{
    internal static class UnityInjector
    {
        internal static Dictionary<Scene, Action<ContainerBuilder>> ScenePreInstaller { get; } = new();
        internal static Dictionary<Scene, Container> ContainersPerScene { get; } = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeAwakeOfFirstSceneOnly()
        {
            ReportReflexDebuggerStatus();
            
            var projectContainer = CreateProjectContainer();
            ContainersPerScene.Clear();

            void InjectScene(Scene scene)
            {
                ReflexLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
                var sceneContainer = CreateSceneContainer(scene, projectContainer);
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
                Tree<Container>.Root = null;
                projectContainer.Dispose();
                
                // Unsubscribe from static events ensuring that Reflex works with domain reloading set to false
                Callbacks.OnSceneLoaded -= InjectScene;
                Callbacks.OnSceneUnloaded -= DisposeScene;
                Application.quitting -= DisposeProject;
            }
            
            Callbacks.OnSceneLoaded += InjectScene;
            Callbacks.OnSceneUnloaded += DisposeScene;
            Application.quitting += DisposeProject;
        }

        private static Container CreateProjectContainer()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
            
            if (ResourcesUtilities.TryLoad<ProjectScope>(nameof(ProjectScope), out var projectScope))
            {
                projectScope.InstallBindings(builder);
            }
            
            var container = Tree<Container>.Root = builder.Build();

            return container;
        }

        private static Container CreateSceneContainer(Scene scene, Container projectContainer)
        {
            return projectContainer.Scope(builder =>
            {
                builder.SetName($"{scene.name} ({scene.GetHashCode()})");
                
                if (ScenePreInstaller.Remove(scene, out var preInstaller))
                {
                    preInstaller.Invoke(builder);
                }
                
                if (scene.TryFindAtRoot<SceneScope>(out var sceneScope))
                {
                    sceneScope.InstallBindings(builder);
                }
            });
        }

        [Conditional("REFLEX_DEBUG")]
        private static void ReportReflexDebuggerStatus()
        {
            ReflexLogger.Log("Symbol REFLEX_DEBUG are defined, performance impacted!", LogLevel.Warning);
        }
    }
}
