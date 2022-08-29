using System;

using UnityEditor.TestTools.TestRunner.Api;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
	[Serializable]
	internal class TestAdaptorContainer
	{
		public TestAdaptor[] TestAdaptors;
	}

	[Serializable]
	internal class TestAdaptor
	{
		public string Id;
		public string Name;
		public string FullName;

		public string Type;
		public string Method;
		public string Assembly;

		public int Parent;

		public TestAdaptor(ITestAdaptor testAdaptor, int parent)
		{
			Id = testAdaptor.Id;
			Name = testAdaptor.Name;
			FullName = testAdaptor.FullName;

			Type = testAdaptor.TypeInfo?.FullName;
			Method = testAdaptor.Method?.Name;
			Assembly = testAdaptor.TypeInfo?.Assembly?.Location;

			Parent = parent;
		}
	}
}
