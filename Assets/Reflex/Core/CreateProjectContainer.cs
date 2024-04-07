using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);

            foreach (var projectScope in projectScopes)
            {
                projectScope.InstallBindings(builder);
            }

            return builder.Build();
        }
    }
}