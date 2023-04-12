using Reflex.Configuration;
using Reflex.Core;
using NUnit.Framework;
using UnityEngine;

namespace Reflex.Tests
{
    public class ProjectSetupTests
    {
        [Test]
        public void ProjectCanHaveNoneOrSingleProjectContext()
        {
            var assets = Resources.LoadAll<ProjectContext>(string.Empty);

            if (assets.Length is 0 or 1)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"You can have none or a single ProjectContext, currently you have {assets.Length}");
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
