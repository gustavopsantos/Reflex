using Reflex.Core;
using Reflex.Logging;
using UnityEngine;

namespace Reflex.Core
{
    public sealed class PrefabScope : MonoBehaviour
    {
        public void InstallBindings(ContainerDescriptor descriptor)
        {
            foreach (var nestedInstaller in GetComponentsInChildren<IInstaller>())
            {
                nestedInstaller.InstallBindings(descriptor);
            }

            ReflexLogger.Log($"{nameof(PrefabScope)} ({gameObject.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}