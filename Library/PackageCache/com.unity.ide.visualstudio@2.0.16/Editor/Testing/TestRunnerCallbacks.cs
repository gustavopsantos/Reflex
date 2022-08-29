using System;
using System.Collections.Generic;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
	internal class TestRunnerCallbacks : ICallbacks
	{
		private string Serialize<TContainer, TSource, TAdaptor>(
			TSource source,
			Func<TSource, int, TAdaptor> createAdaptor,
			Func<TSource, IEnumerable<TSource>> children,
			Func<TAdaptor[], TContainer> container)
		{
			var adaptors = new List<TAdaptor>();

			void AddAdaptor(TSource item, int parentIndex)
			{
				var index = adaptors.Count;
				adaptors.Add(createAdaptor(item, parentIndex));
				foreach (var child in children(item))
					AddAdaptor(child, index);
			}

			AddAdaptor(source, -1);

			return JsonUtility.ToJson(container(adaptors.ToArray()));
		}

		private string Serialize(ITestAdaptor testAdaptor)
		{
			return Serialize(
				testAdaptor,
				(a, parentIndex) => new TestAdaptor(a, parentIndex),
				(a) => a.Children,
				(r) => new TestAdaptorContainer { TestAdaptors = r });
		}

		private string Serialize(ITestResultAdaptor testResultAdaptor)
		{
			return Serialize(
				testResultAdaptor,
				(a, parentIndex) => new TestResultAdaptor(a, parentIndex),
				(a) => a.Children,
				(r) => new TestResultAdaptorContainer { TestResultAdaptors = r });
		}

		public void RunFinished(ITestResultAdaptor testResultAdaptor)
		{
			VisualStudioIntegration.BroadcastMessage(Messaging.MessageType.RunFinished, Serialize(testResultAdaptor));
		}

		public void RunStarted(ITestAdaptor testAdaptor)
		{
			VisualStudioIntegration.BroadcastMessage(Messaging.MessageType.RunStarted, Serialize(testAdaptor));
		}

		public void TestFinished(ITestResultAdaptor testResultAdaptor)
		{
			VisualStudioIntegration.BroadcastMessage(Messaging.MessageType.TestFinished, Serialize(testResultAdaptor));
		}

		public void TestStarted(ITestAdaptor testAdaptor)
		{
			VisualStudioIntegration.BroadcastMessage(Messaging.MessageType.TestStarted, Serialize(testAdaptor));
		}

		private static string TestModeName(TestMode testMode)
		{
			switch (testMode)
			{
				case TestMode.EditMode: return "EditMode";
				case TestMode.PlayMode: return "PlayMode";
			}

			throw new ArgumentOutOfRangeException();
		}


		internal void TestListRetrieved(TestMode testMode, ITestAdaptor testAdaptor)
		{
			// TestListRetrieved format:
			// TestMode:Json

			var value = TestModeName(testMode) + ":" + Serialize(testAdaptor);
			VisualStudioIntegration.BroadcastMessage(Messaging.MessageType.TestListRetrieved, value);
		}
	}
}
