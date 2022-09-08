using UnityEngine;
using Reflex.Scripts.Core;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
	internal static class UnityInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void AfterAssembliesLoaded()
		{
			var stack = new ContainerStack();
			
			if (TryGetProjectContext(out var projectContext))
			{
				var container = stack.PushNew();
				projectContext.InstallBindings(container);
				SceneManager.sceneLoaded += (scene, mode) => SceneInjector.Inject(scene, stack);
				Application.quitting += () => stack.Pop().Dispose();
			}
		}

		private static bool TryGetProjectContext(out ProjectContext projectContext)
		{
			projectContext = Resources.Load<ProjectContext>("ProjectContext");
			ValidateProjectContext(projectContext);
			return projectContext != null;
		}

		private static void ValidateProjectContext(ProjectContext projectContext)
		{
			if (projectContext == null)
			{
				Debug.LogWarning($"Skipping {nameof(UnityInjector)}. A context prefab named 'ProjectContext' with component 'ProjectContext' attached to it should exist inside a Resources folder.");
			}
		}
	}
}