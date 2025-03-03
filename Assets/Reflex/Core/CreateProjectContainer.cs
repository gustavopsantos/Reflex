using System.IO;
using System.Linq;
using System.Collections.Generic;
using Reflex.Configuration;
using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var reflexSettings = Resources.Load<ReflexSettings>(nameof(ReflexSettings));

            var loadAllProjectScopes = reflexSettings != null
                ? reflexSettings.LoadAllProjectScopes
                : true;

            var builder = new ContainerBuilder().SetName("ProjectContainer");
            var activeProjectScopes = loadAllProjectScopes
                ? LoadAllProjectScopes()
                : LoadSingleProjectScope();

            foreach (var projectScope in activeProjectScopes)
            {
                projectScope.InstallBindings(builder);
            }

            return builder.Build();
        }

        private static IEnumerable<ProjectScope> LoadAllProjectScopes()
        {
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            var activeProjectScopes = projectScopes;
            return activeProjectScopes;
        }

        private static IEnumerable<ProjectScope> LoadSingleProjectScope()
        {
            var projectScope = Resources.Load<ProjectScope>("ProjectScope");
            if (projectScope != null && projectScope.gameObject.activeSelf)
            {
                yield return projectScope;
            }
        }
    }
}
