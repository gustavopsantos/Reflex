/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class MessageEventArgs
	{
		public Message Message
		{
			get;
		}

		public MessageEventArgs(Message message)
		{
			Message = message;
		}
	}
}
