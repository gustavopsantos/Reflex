/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal enum MessageType
	{
		None = 0,

		Ping,
		Pong,

		Play,
		Stop,
		Pause,
		Unpause,

		Build,
		Refresh,

		Info,
		Error,
		Warning,

		Open,
		Opened,

		Version,
		UpdatePackage,

		ProjectPath,

		// This message is a technical one for big messages, not intended to be used directly
		Tcp,

		RunStarted,
		RunFinished,
		TestStarted,
		TestFinished,
		TestListRetrieved,

		RetrieveTestList,
		ExecuteTests,

		ShowUsage
	}
}
