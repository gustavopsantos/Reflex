using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class ProjectScope : MonoBehaviour
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(containerBuilder);
            }

            ReflexLogger.Log($"{nameof(ProjectScope)} Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}