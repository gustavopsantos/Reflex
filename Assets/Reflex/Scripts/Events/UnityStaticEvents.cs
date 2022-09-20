using System;
using UnityEngine.SceneManagement;

namespace Reflex.Scripts.Events
{
    public static class UnityStaticEvents
    {
        public static Action<Scene> OnSceneEarlyAwake;
    }
}