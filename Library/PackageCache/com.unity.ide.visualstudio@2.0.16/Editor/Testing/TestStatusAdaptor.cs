using System;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
	[Serializable]
	internal enum TestStatusAdaptor
	{
		Passed,
		Skipped,
		Inconclusive,
		Failed,
	}
}
