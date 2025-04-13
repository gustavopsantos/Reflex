using UnityEngine;
using UnityEngine.Pool;
using Reflex.Injectors;

namespace Reflex.Core
{
    public class ContainerScope : MonoBehaviour
    {
        private void Awake()
        {
            UnityInjector.OnSceneLoaded.Invoke(gameObject.scene, this);
        }
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using (ListPool<IInstaller>.Get(out var installers))
            {
                GetComponentsInChildren(installers);
            
                for (var i = 0; i < installers.Count; i++)
                {
                    installers[i].InstallBindings(containerBuilder);
                }
            }
        }
    }
}