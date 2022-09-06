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

		private static bool TryGetProjectContext(out Context projectContext)
		{
			projectContext = Resources.Load<Context>("ProjectContext");
			ValidateProjectContext(projectContext);
			return projectContext != null;
		}

		private static void ValidateProjectContext(Context projectContext)
		{
			if (projectContext == null || projectContext.Kind != ContextKind.Project)
			{
				Debug.LogWarning($"Skipping {nameof(UnityInjector)}. A context prefab named 'ProjectContext' with kind 'Project' should exist inside a Resources folder.");
			}
		}
	}
}