using UnityEngine;

namespace Reflex.Scripts.Core
{
    public class SceneContext : AContext
    {
        public override void InstallBindings(Container container)
        {
            base.InstallBindings(container);
            Debug.Log($"{GetType().Name} Bindings Installed");
        }
    }
}