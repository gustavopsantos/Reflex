using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Reflex.Tests
{
    [SetUpFixture]
    public class TestRunnerExtensions : ICallbacks
    {
        private readonly List<ITestResultAdaptor> _results = new();

        [OneTimeSetUp]
        public void Setup()
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(this);
        }

        void ICallbacks.RunStarted(ITestAdaptor testsToRun)
        {
            _results.Clear();
        }

        void ICallbacks.RunFinished(ITestResultAdaptor result)
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.UnregisterCallbacks(this);
            
            using (new ApplicationStackTraceLogTypeScope(LogType.Log, StackTraceLogType.None))
            {
                ReportStatus(TestStatus.Passed);
                ReportStatus(TestStatus.Failed);
                ReportStatus(TestStatus.Skipped);
                ReportStatus(TestStatus.Inconclusive);
            }
        }

        void ICallbacks.TestStarted(ITestAdaptor test)
        {
        }

        void ICallbacks.TestFinished(ITestResultAdaptor result)
        {
            if (!result.Test.IsSuite && !result.Test.IsTestAssembly)
            {
                _results.Add(result);
            }
        }

        private void ReportStatus(TestStatus status)
        {
            var report = _results
                .Where(r => r.TestStatus == status)
                .Select(r => r.Name)
                .OrderBy(r => r.Length)
                .ToList();

            report.Insert(0, $"[{status.ToString().ToUpper()} {report.Count}/{_results.Count}]");

            Debug.Log(string.Join(Environment.NewLine, report));
        }
    }
}