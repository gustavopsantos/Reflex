using System.Linq;
using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
#if REFLEX_SINGLEPROJECTSCOPE
            var projectScope = Resources.Load<ProjectScope>("ProjectScope");
            projectScope.InstallBindings(builder);
#else
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            var activeProjectScopes = projectScopes.Where(s => s.gameObject.activeSelf);

            foreach (var projectScope in activeProjectScopes)
            {
                projectScope.InstallBindings(builder);
            }
#endif

            return builder.Build();
        }
    }
}
