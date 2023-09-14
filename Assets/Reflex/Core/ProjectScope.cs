using Microsoft.Extensions.DependencyInjection;
using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class ProjectScope : MonoBehaviour
    {
        public void InstallBindings(IServiceCollection serviceCollection)
        {
            foreach (IInstaller nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(serviceCollection);
            }

            ReflexLogger.Log($"{nameof(ProjectScope)} Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}