using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Reflex.Scripts.Core;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
	internal static class SceneInjector
	{
		internal static void Inject(Scene scene, Container projectContainer)
		{
			var sceneContainer = CreateSceneContainer(scene, projectContainer);
			
			foreach (var monoBehaviour in GetEveryMonoBehaviourAtScene(scene))
			{
				MonoInjector.Inject(monoBehaviour, sceneContainer);
			}
		}
		
		private static Container CreateSceneContainer(Scene scene, Container projectContainer)
		{
			var container = projectContainer.Scope();
			scene.OnUnload(() => container.Dispose());

			if (scene.TryFindAtRootObjects<SceneContext>(out var sceneContext))
			{
				sceneContext.InstallBindings(container);
			}

			return container;
		}

		private static IEnumerable<MonoBehaviour> GetEveryMonoBehaviourAtScene(Scene scene)
		{
			return scene.GetRootGameObjects().SelectMany(gameObject => gameObject.GetComponentsInChildren<MonoBehaviour>(true));
		}
	}
}