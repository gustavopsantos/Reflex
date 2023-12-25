using Reflex.Logging;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    public sealed class ProjectScope : MonoBehaviour
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using var pooledObject = ListPool<IInstaller>.Get(out var installers);
            GetComponentsInChildren<IInstaller>(installers);
            
            for (var i = 0; i < installers.Count; i++)
            {
                installers[i].InstallBindings(containerBuilder);
            }

            ReflexLogger.Log("ProjectScope Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}