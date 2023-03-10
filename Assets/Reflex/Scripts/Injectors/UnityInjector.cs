using UnityEngine;
using Reflex.Scripts.Core;
using Reflex.Scripts.Events;
using Reflex.Scripts.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Assets.Reflex.Scripts.Configuration;
using Reflex.Scripts.Logging;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Injectors
{
	internal static class UnityInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeAwakeOfFirstSceneOnly()
		{
			var projectContainer = CreateProjectContainer();
			LoadConfiguration();
			UnityStaticEvents.OnSceneEarlyAwake += scene =>
			{
				var sceneContainer = CreateSceneContainer(scene, projectContainer);
				SceneInjector.Inject(scene, sceneContainer);
			};
		}

		private static void LoadConfiguration()
		{
			if (ResourcesUtilities.TryLoad<ReflexConfiguration>(ReflexConfiguration.AssetName, out var configuration))
			{
				DebugLogger.Apply(configuration);
			}
		}

		private static Container CreateProjectContainer()
		{
			var container = ContainerTree.Root = new Container("ProjectContainer");

			Application.quitting += () =>
			{
				ContainerTree.Root = null;
				container.Dispose();
			};

			if (ResourcesUtilities.TryLoad<ProjectContext>("ProjectContext", out var projectContext))
			{
				projectContext.InstallBindings(container);
			}

			return container;
		}

		private static Container CreateSceneContainer(Scene scene, Container projectContainer)
		{
			var container = projectContainer.Scope(scene.name);

			var subscription = scene.OnUnload(() =>
			{
				container.Dispose();
			});

			// If app is being closed, all containers will be disposed by depth first search starting from project container root, see UnityInjector.cs
			Application.quitting += () =>
			{
				subscription.Dispose();
			};

			if (scene.TryFindAtRootObjects<SceneContext>(out var sceneContext))
			{
				sceneContext.InstallBindings(container);
			}

			return container;
		}
	}
}
