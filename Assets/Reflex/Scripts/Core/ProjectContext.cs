using UnityEngine;

namespace Reflex.Scripts.Core
{
    public class ProjectContext : AContext
    {
        public override void InstallBindings(IContainer container)
        {
            base.InstallBindings(container);
            Debug.Log($"{GetType().Name} Bindings Installed");
        }
    }
}