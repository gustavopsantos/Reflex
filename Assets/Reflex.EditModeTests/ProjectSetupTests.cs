using NUnit.Framework;
using Reflex.Configuration;
using UnityEngine;

namespace Reflex.EditModeTests
{
    public class ProjectSetupTests
    {
        [Test]
        public void ProjectShouldHaveReflexSettings()
        {
            if (ReflexSettings.Instance != null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"Project is missing ReflexSettings scriptable object at Resources folder.");
            }
        }
    }
}