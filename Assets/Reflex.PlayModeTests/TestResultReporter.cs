﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class TestResultReporter : ICallbacks
    {
        private readonly TestRunnerApi _testRunnerApi;
        private readonly List<ITestResultAdaptor> _results = new();

        public TestResultReporter(TestRunnerApi testRunnerApi)
        {
            _testRunnerApi = testRunnerApi;
        }
        
        void ICallbacks.RunStarted(ITestAdaptor testsToRun)
        {
            _results.Clear();
        }

        void ICallbacks.RunFinished(ITestResultAdaptor result)
        {
            _testRunnerApi.UnregisterCallbacks(this);
            
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
            var dict = new Dictionary<TestStatus, string>
            {
                { TestStatus.Passed, "✅" },
                { TestStatus.Failed, "❌" },
                { TestStatus.Skipped, "⏭️" },
                { TestStatus.Inconclusive, "⭕" },
            };
            
            var report = _results
                .Where(r => r.TestStatus == status)
                .Select(r => r.Name)
                .OrderBy(r => r.Length)
                .ToList();

            report.Insert(0, $"{dict[status]} [{status.ToString().ToUpper()} {report.Count}/{_results.Count}]");

            Debug.Log(string.Join(Environment.NewLine, report));
        }
    }
}