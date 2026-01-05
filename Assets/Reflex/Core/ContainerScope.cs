using System;
using UnityEngine;
using UnityEngine.Pool;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    [DefaultExecutionOrder(SceneContainerScopeExecutionOrder)]
    public class ContainerScope : MonoBehaviour
    {
        public const int SceneContainerScopeExecutionOrder = -1_000_000_000;
        
        public static Action<ContainerBuilder> OnRootContainerBuilding;
        public static Action<Scene, ContainerBuilder> OnSceneContainerBuilding;
        
        private void Awake() // Note that Awake will only be called for ContainerScopes used inside scenes, not for RootScope as they are never instantiated
        {
            UnityInjector.OnSceneLoaded.Invoke(gameObject.scene, this); // This will only be executed 
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