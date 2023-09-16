using Microsoft.Extensions.DependencyInjection;
using Reflex.Microsoft.Logging;
using UnityEngine;

namespace Reflex.Microsoft.Core
{
	public sealed class ProjectScope : MonoBehaviour
	{
		public void InstallBindings(IServiceCollection serviceCollection)
		{
			foreach (IInstaller nestedInstaller in GetComponentsInChildren<IInstaller>())
			{
				nestedInstaller.InstallBindings(serviceCollection);
			}

			ReflexMicrosoftLogger.Log($"{nameof(ProjectScope)} Bindings Installed", LogLevel.Info, gameObject);
		}
	}
}