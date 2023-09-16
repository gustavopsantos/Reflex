using Microsoft.Extensions.DependencyInjection;
using Reflex.Microsoft.Logging;
using UnityEngine;

namespace Reflex.Microsoft.Core
{
    public sealed class SceneScope : MonoBehaviour
    {
        public void InstallBindings(IServiceCollection serviceCollection)
        {
            foreach (IInstaller nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(serviceCollection);
            }

            ReflexMicrosoftLogger.Log($"{nameof(SceneScope)} ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}