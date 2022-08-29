/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using System.Text;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class Serializer
	{
		private readonly MemoryStream _stream;
		private readonly BinaryWriter _writer;

		public Serializer()
		{
			_stream = new MemoryStream();
			_writer = new BinaryWriter(_stream);
		}

		public void WriteInt32(int i)
		{
			_writer.Write(i);
		}

		public void WriteString(string s)
		{
			var bytes = Encoding.UTF8.GetBytes(s ?? "");
			if (bytes.Length > 0)
			{
				_writer.Write(bytes.Length);
				_writer.Write(bytes);
			}
			else
				_writer.Write(0);
		}

		public byte[] Buffer()
		{
			return _stream.ToArray();
		}
	}
}
