using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Utilities
{
    public static class Callbacks
    {
        public static event Action<Scene> OnSceneLoaded; 
        public static event Action<Scene> OnSceneUnloaded;

        public static void RaiseOnSceneLoaded(Scene scene)
        {
            OnSceneLoaded?.Invoke(scene); 
        }
    
        public static void RaiseOnSceneUnloaded(Scene scene)
        {
            OnSceneUnloaded?.Invoke(scene);  
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded -= RaiseOnSceneUnloaded;
            SceneManager.sceneUnloaded += RaiseOnSceneUnloaded;
        }
    }
}
