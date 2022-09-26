using UnityEngine;
using Reflex.Scripts.Core;
using Reflex.Scripts.Events;
using Reflex.Scripts.Utilities;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Injectors
{
	internal static class UnityInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeAwakeOfFirstSceneOnly()
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