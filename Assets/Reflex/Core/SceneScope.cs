using Microsoft.Extensions.DependencyInjection;
using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class SceneScope : MonoBehaviour
    {
        public void InstallBindings(IServiceCollection serviceCollection)
        {
            foreach (IInstaller nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(serviceCollection);
            }

            ReflexLogger.Log($"{nameof(SceneScope)} ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}