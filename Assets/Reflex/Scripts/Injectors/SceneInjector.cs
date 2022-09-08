using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Reflex.Scripts.Core;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
	internal static class SceneInjector
	{
		internal static void Inject(Scene scene, ContainerStack containerStack)
		{
			if (scene.TryFindAtRootObjects<AContext>(out var sceneContext))
			{
				var container = containerStack.PushNew();
				sceneContext.InstallBindings(container);

				void DisposeScopedContainer(Scene _)
				{
					containerStack.Pop().Dispose();
					SceneManager.sceneUnloaded -= DisposeScopedContainer;
				}

				SceneManager.sceneUnloaded += DisposeScopedContainer;
			}
			
			foreach (var monoBehaviour in GetEveryMonoBehaviourAtScene(scene))
			{
				MonoInjector.Inject(monoBehaviour, containerStack);
			}
		}

		private static IEnumerable<MonoBehaviour> GetEveryMonoBehaviourAtScene(Scene scene)
		{
			return scene.GetRootGameObjects().SelectMany(gameObject => gameObject.GetComponentsInChildren<MonoBehaviour>(true));
		}
	}
}