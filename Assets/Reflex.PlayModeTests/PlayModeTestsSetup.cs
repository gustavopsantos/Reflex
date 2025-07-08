using NUnit.Framework;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    [SetUpFixture]
    public class PlayModeTestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var testResultReporter = new TestResultReporter(testRunnerApi);
            testRunnerApi.RegisterCallbacks(testResultReporter);
        }
    }
}