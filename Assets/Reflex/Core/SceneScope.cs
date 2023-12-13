using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class SceneScope : MonoBehaviour
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(containerBuilder);
            }

            ReflexLogger.Log($"{nameof(SceneScope)} ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}