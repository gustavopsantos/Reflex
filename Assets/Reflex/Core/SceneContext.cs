using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class SceneContext : MonoBehaviour
    {
        public void InstallBindings(ContainerDescriptor descriptor)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(descriptor);
            }

            ReflexLogger.Log($"SceneContext ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}