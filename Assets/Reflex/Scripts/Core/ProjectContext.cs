using Reflex.Scripts.Logging;

namespace Reflex.Scripts.Core
{
    public class ProjectContext : AContext
    {
        public override void InstallBindings(Container container)
        {
            base.InstallBindings(container);
            DebugLogger.Log($"{GetType().Name} Bindings Installed");
        }
    }
}
