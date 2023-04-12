using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class ProjectContext : MonoBehaviour
    {
        public void InstallBindings(ContainerDescriptor descriptor)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(descriptor);
            }

            ReflexLogger.Log("ProjectContext Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}