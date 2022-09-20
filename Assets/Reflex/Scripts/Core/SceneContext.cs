using UnityEngine;
using Reflex.Scripts.Events;

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
            Debug.Log($"{GetType().Name} Bindings Installed");
        }
    }
}