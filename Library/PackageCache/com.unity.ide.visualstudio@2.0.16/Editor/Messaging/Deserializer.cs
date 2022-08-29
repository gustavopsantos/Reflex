/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using System.Text;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class Deserializer
	{
		private readonly BinaryReader _reader;

		public Deserializer(byte[] buffer)
		{
			_reader = new BinaryReader(new MemoryStream(buffer));
		}

		public int ReadInt32()
		{
			return _reader.ReadInt32();
		}

		public string ReadString()
		{
			var length = ReadInt32();
			return length > 0
				? Encoding.UTF8.GetString(_reader.ReadBytes(length))
				: "";
		}

		public bool CanReadMore()
		{
			return _reader.BaseStream.Position < _reader.BaseStream.Length;
		}
	}
}
