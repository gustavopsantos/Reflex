using Reflex.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
	internal class UnityInjector : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void AfterAssembliesLoaded()
		{
			if (TryGetProjectContext(out var projectContext))
			{
				projectContext.InstallBindings();
				SceneManager.sceneLoaded += (scene, mode) => SceneInjector.Inject(scene, projectContext.Container);
			}
		}

		private static bool TryGetProjectContext(out ProjectContext projectContext)
		{
			projectContext = Resources.Load<ProjectContext>("ProjectContext");
			Assert.IsNotNull(projectContext, "Skipping MonoInjector. A project context prefab named 'ProjectContext' should exist inside a Resources folder.");
			return projectContext != null;
		}
	}
}