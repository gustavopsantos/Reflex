using System.Linq;
using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            var activeProjectScopes = projectScopes.Where(s => s.gameObject.activeSelf);

            foreach (var projectScope in activeProjectScopes)
            {
                projectScope.InstallBindings(builder);
            }

            return builder.Build();
        }
    }
}