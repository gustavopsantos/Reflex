/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class SolutionParser
	{
		// Compared to the bridge implementation, we are not returning "{" "}" from Guids
		private static readonly Regex ProjectDeclaration = new Regex(@"Project\(\""{(?<projectFactoryGuid>.*?)}\""\)\s+=\s+\""(?<name>.*?)\"",\s+\""(?<fileName>.*?)\"",\s+\""{(?<projectGuid>.*?)}\""(?<metadata>.*?)\bEndProject\b", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
		private static readonly Regex PropertiesDeclaration = new Regex(@"GlobalSection\((?<name>([\w]+Properties|NestedProjects))\)\s+=\s+(?<type>(?:post|pre)Solution)(?<entries>.*?)EndGlobalSection", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
		private static readonly Regex PropertiesEntryDeclaration = new Regex(@"^\s*(?<key>.*?)=(?<value>.*?)$", RegexOptions.Multiline | RegexOptions.ExplicitCapture);

		public static Solution ParseSolutionFile(string filename, IFileIO fileIO)
		{
			return ParseSolutionContent(fileIO.ReadAllText(filename));
		}

		public static Solution ParseSolutionContent(string content)
		{
			return new Solution
			{
				Projects = ParseSolutionProjects(content),
				Properties = ParseSolutionProperties(content)
			};
		}

		private static SolutionProjectEntry[] ParseSolutionProjects(string content)
		{
			var projects = new List<SolutionProjectEntry>();
			var mc = ProjectDeclaration.Matches(content);

			foreach (Match match in mc)
			{
				projects.Add(new SolutionProjectEntry
				{
					ProjectFactoryGuid = match.Groups["projectFactoryGuid"].Value,
					Name = match.Groups["name"].Value,
					FileName = match.Groups["fileName"].Value,
					ProjectGuid = match.Groups["projectGuid"].Value,
					Metadata = match.Groups["metadata"].Value
				});
			}

			return projects.ToArray();
		}

		private static SolutionProperties[] ParseSolutionProperties(string content)
		{
			var properties = new List<SolutionProperties>();
			var mc = PropertiesDeclaration.Matches(content);

			foreach (Match match in mc)
			{
				var sp = new SolutionProperties
				{
					Entries = new List<KeyValuePair<string, string>>(),
					Name = match.Groups["name"].Value,
					Type = match.Groups["type"].Value
				};

				var entries = match.Groups["entries"].Value;
				var mec = PropertiesEntryDeclaration.Matches(entries);
				foreach (Match entry in mec)
				{
					var key = entry.Groups["key"].Value.Trim();
					var value = entry.Groups["value"].Value.Trim();
					sp.Entries.Add(new KeyValuePair<string, string>(key, value));
				}

				properties.Add(sp);
			}

			return properties.ToArray();
		}
	}
}
