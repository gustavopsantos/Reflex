using Reflex.Utilities;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
            
            if (ResourcesUtilities.TryLoad<ProjectScope>(nameof(ProjectScope), out var projectScope))
            {
                projectScope.InstallBindings(builder);
            }
            
            return builder.Build();
        }
    }
}