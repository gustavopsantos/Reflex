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
            var reflexSettings = ReflexSettings.Instance;
            var builder = new ContainerBuilder().SetName("ProjectContainer");

            if (reflexSettings.ProjectScopes != null)
            {
                foreach (var projectScope in reflexSettings.ProjectScopes.Where(x => x != null && x.gameObject.activeSelf))
                {
                    projectScope.InstallBindings(builder);
                }
            }

            return builder.Build();
        }
    }
}
