using NUnit.Framework;
using Reflex.Configuration;
using Reflex.Core;
using UnityEngine;

namespace Reflex.EditModeTests
{
    public class ProjectSetupTests
    {
        [Test]
        public void ProjectCanHaveNoneOrSingleProjectScope()
        {
            var assets = Resources.LoadAll<ProjectScope>(string.Empty);

            if (assets.Length is 0 or 1)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"You can have none or a single {nameof(ProjectScope)}, currently you have {assets.Length}");
            }
        }
        
        [Test]
        public void ProjectCanHaveNoneOrSingleReflexSettings()
        {
            var assets = Resources.LoadAll<ReflexSettings>(string.Empty);

            if (assets.Length is 0 or 1)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"You can have none or a single ReflexSettings, currently you have {assets.Length}");
            }
        }
    }
}
