/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using System.Text;

namespace Microsoft.Unity.VisualStudio.Editor
{
	public interface IFileIO
	{
		bool Exists(string fileName);

		string ReadAllText(string fileName);
		void WriteAllText(string fileName, string content);
	}

	class FileIOProvider : IFileIO
	{
		public bool Exists(string fileName)
		{
			return File.Exists(fileName);
		}

		public string ReadAllText(string fileName)
		{
			return File.ReadAllText(fileName);
		}

		public void WriteAllText(string fileName, string content)
		{
			File.WriteAllText(fileName, content, Encoding.UTF8);
		}
	}
}
