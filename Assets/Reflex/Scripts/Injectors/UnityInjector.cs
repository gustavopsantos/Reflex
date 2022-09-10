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
				Application.quitting += () => stack.Pop().Dispose();
			}
			
			SceneManager.sceneLoaded += (scene, mode) => SceneInjector.Inject(scene, stack);
		}

		private static bool TryGetProjectContext(out ProjectContext projectContext)
		{
			projectContext = Resources.Load<ProjectContext>("ProjectContext");
			return projectContext != null;
		}
	}
}