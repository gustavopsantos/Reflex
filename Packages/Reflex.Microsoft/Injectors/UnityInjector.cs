using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Extensions;
using Reflex.Microsoft.Generics;
using Reflex.Microsoft.Logging;
using Reflex.Microsoft.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Microsoft.Injectors
{
	internal static class UnityInjector
	{
		internal static Dictionary<Scene, Action<IServiceCollection>> Extensions { get; } = new();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeAwakeOfFirstSceneOnly()
		{
			IServiceProvider projectContainer = CreateProjectContainer();
			Dictionary<Scene, IServiceProvider> containersByScene = new Dictionary<Scene, IServiceProvider>();

			void InjectScene(Scene scene, LoadSceneMode mode = default)
			{
				ReflexMicrosoftLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) loaded", LogLevel.Development);
				IServiceProvider sceneContainer = CreateSceneContainer(scene, projectContainer);
				containersByScene.Add(scene, sceneContainer);
				SceneInjector.Inject(scene, sceneContainer);
			}

			void DisposeScene(Scene scene)
			{
				ReflexMicrosoftLogger.Log($"Scene {scene.name} ({scene.GetHashCode()}) unloaded", LogLevel.Development);
				IServiceProvider sceneContainer = containersByScene[scene];
				containersByScene.Remove(scene);
				((ServiceProvider)sceneContainer).Dispose();
			}

			void DisposeProject()
			{
				Tree<IServiceProvider>.Root = null;
				((ServiceProvider)projectContainer).Dispose();

				// Unsubscribe from static events ensuring that Reflex works with domain reloading set to false
				SceneManager.sceneLoaded -= InjectScene;
				SceneManager.sceneUnloaded -= DisposeScene;
				Application.quitting -= DisposeProject;
			}

#if UNITY_EDITOR
			if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled &&
				UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableSceneReload))
			{
				InjectScene(SceneManager.GetActiveScene());
			}
#endif

			SceneManager.sceneLoaded += InjectScene;
			SceneManager.sceneUnloaded += DisposeScene;
			Application.quitting += DisposeProject;
		}

		private static IServiceProvider CreateProjectContainer()
		{
			ServiceCollection builder = new ServiceCollection();

			if (ResourcesUtilities.TryLoad<ProjectScope>(nameof(ProjectScope), out ProjectScope projectScope))
			{
				projectScope.InstallBindings(builder);
			}

			IServiceProvider container = Tree<IServiceProvider>.Root = builder.BuildServiceProvider();

			return container;
		}

		private static IServiceProvider CreateSceneContainer(Scene scene, IServiceProvider projectServiceProvider)
		{
			IServiceCollection services = new ServiceCollection();

			if (Extensions.TryGetValue(scene, out Action<IServiceCollection> preBuilder))
			{
				Extensions.Remove(scene);
				preBuilder.Invoke(services);
			}

			if (scene.TryFindAtRoot<SceneScope>(out SceneScope sceneScope))
			{
				sceneScope.InstallBindings(services);
			}

			return services.BuildServiceProvider();
		}
	}
}