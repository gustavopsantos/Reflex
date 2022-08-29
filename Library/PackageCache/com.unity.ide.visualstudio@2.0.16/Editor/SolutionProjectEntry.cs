/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal class SolutionProjectEntry
	{
		public string ProjectFactoryGuid { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; }
		public string ProjectGuid { get; set; }
		public string Metadata { get; set; }

		public bool IsSolutionFolderProjectFactory()
		{
			return ProjectFactoryGuid != null && ProjectFactoryGuid.Equals("2150E333-8FDC-42A3-9474-1A3956D46DE8", StringComparison.OrdinalIgnoreCase);
		}
	}
}
