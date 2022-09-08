using UnityEngine;

namespace Reflex.Scripts.Core
{
    public class SceneContext : AContext
    {
        public override void InstallBindings(IContainer container)
        {
            base.InstallBindings(container);
            Debug.Log($"{GetType().Name} Bindings Installed");
        }
    }
}