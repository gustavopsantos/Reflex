using NUnit.Framework;
using Reflex.PlayModeTests;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Reflex.EditModeTests
{
    [SetUpFixture]
    public class EditModeTestsSetup
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