using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class ProjectScope : MonoBehaviour
    {
        public void InstallBindings(ContainerDescriptor descriptor)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(descriptor);
            }

            ReflexLogger.Log($"{nameof(ProjectScope)} Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}