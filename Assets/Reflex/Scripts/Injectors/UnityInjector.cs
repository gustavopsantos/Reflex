using UnityEngine;
using Reflex.Scripts.Core;
using Reflex.Scripts.Events;
using Reflex.Scripts.Utilities;

namespace Reflex.Injectors
{
	internal static class UnityInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void AfterAssembliesLoaded()
		{
			var projectContainer = CreateProjectContainer();
			//SceneManager.sceneLoaded += (scene, mode) => SceneInjector.Inject(scene, projectContainer);
			UnityStaticEvents.OnSceneEarlyAwake += scene => SceneInjector.Inject(scene, projectContainer);
		}

		private static Container CreateProjectContainer()
		{
			var container = new Container("Project");
			Application.quitting += () =>
			{
				ContainerTree.Root = null;
				container.Dispose();
			};
			ContainerTree.Root = container;

			if (ResourcesUtilities.TryLoad<ProjectContext>("ProjectContext", out var projectContext))
			{
				projectContext.InstallBindings(container);
			}

			return container;
		}
	}
}