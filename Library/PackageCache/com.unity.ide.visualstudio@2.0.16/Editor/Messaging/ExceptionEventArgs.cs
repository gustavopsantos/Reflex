/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class ExceptionEventArgs
	{
		public Exception Exception { get; }

		public ExceptionEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}
}
