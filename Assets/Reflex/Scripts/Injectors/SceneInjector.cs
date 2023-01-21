using UnityEngine;
using Reflex.Scripts.Core;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
	internal static class SceneInjector
	{
		internal static void Inject(Scene scene, Container projectContainer)
		{
			var sceneContainer = CreateSceneContainer(scene, projectContainer);
			
			foreach (var monoBehaviour in scene.All<MonoBehaviour>())
			{
				AttributeInjector.Inject(monoBehaviour, sceneContainer);
			}
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