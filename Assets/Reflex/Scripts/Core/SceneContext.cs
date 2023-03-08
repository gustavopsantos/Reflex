using UnityEngine;
using Reflex.Scripts.Events;
using Reflex.Scripts.Logging;

namespace Reflex.Scripts.Core
{
    [DefaultExecutionOrder(-10000)]
    public class SceneContext : AContext
    {
        private void Awake()
        {
            UnityStaticEvents.OnSceneEarlyAwake.Invoke(gameObject.scene);
        }

        public override void InstallBindings(Container container)
        {
            base.InstallBindings(container);
            DebugLogger.Log($"{GetType().Name} Bindings Installed");
        }
    }
}
