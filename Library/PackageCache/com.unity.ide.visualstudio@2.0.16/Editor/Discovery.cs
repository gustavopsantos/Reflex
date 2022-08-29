/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class Discovery
	{
		internal const string ManagedWorkload = "Microsoft.VisualStudio.Workload.ManagedGame";

		internal static string _vsWherePath;

		public static void FindVSWhere()
		{
			_vsWherePath = FileUtility.GetPackageAssetFullPath("Editor", "VSWhere", "vswhere.exe");
		}

		public static IEnumerable<IVisualStudioInstallation> GetVisualStudioInstallations()
		{
			if (VisualStudioEditor.IsWindows)
			{
				foreach (var installation in QueryVsWhere())
					yield return installation;
			}

			if (VisualStudioEditor.IsOSX)
			{
				var candidates = Directory.EnumerateDirectories("/Applications", "*.app");
				foreach (var candidate in candidates)
				{
					if (TryDiscoverInstallation(candidate, out var installation))
						yield return installation;
				}
			}
		}

		private static bool IsCandidateForDiscovery(string path)
		{
			if (File.Exists(path) && VisualStudioEditor.IsWindows && Regex.IsMatch(path, "devenv.exe$", RegexOptions.IgnoreCase))
				return true;

			if (Directory.Exists(path) && VisualStudioEditor.IsOSX && Regex.IsMatch(path, "Visual\\s?Studio(?!.*Code.*).*.app$", RegexOptions.IgnoreCase))
				return true;

			return false;
		}

		public static bool TryDiscoverInstallation(string editorPath, out IVisualStudioInstallation installation)
		{
			installation = null;

			if (string.IsNullOrEmpty(editorPath))
				return false;

			if (!IsCandidateForDiscovery(editorPath))
				return false;

			// On windows we use the executable directly, so we can query extra information
			var fvi = editorPath;

			// On Mac we use the .app folder, so we need to access to main assembly
			if (VisualStudioEditor.IsOSX)
			{
				fvi = Path.Combine(editorPath, "Contents/Resources/lib/monodevelop/bin/VisualStudio.exe");

				if (!File.Exists(fvi))
					fvi = Path.Combine(editorPath, "Contents/MonoBundle/VisualStudio.exe");

				if (!File.Exists(fvi))
					fvi = Path.Combine(editorPath, "Contents/MonoBundle/VisualStudio.dll");				
			}

			if (!File.Exists(fvi))
				return false;

			// VS preview are not using the isPrerelease flag so far
			// On Windows FileDescription contains "Preview", but not on Mac
			var vi = FileVersionInfo.GetVersionInfo(fvi);
			var version = new Version(vi.ProductVersion);
			var isPrerelease = vi.IsPreRelease || string.Concat(editorPath, "/" + vi.FileDescription).ToLower().Contains("preview");

			installation = new VisualStudioInstallation()
			{
				IsPrerelease = isPrerelease,
				Name = $"{vi.FileDescription}{(isPrerelease && VisualStudioEditor.IsOSX ? " Preview" : string.Empty)} [{version.ToString(3)}]",
				Path = editorPath,
				Version = version
			};
			return true;
		}

		#region VsWhere Json Schema
#pragma warning disable CS0649
		[Serializable]
		internal class VsWhereResult
		{
			public VsWhereEntry[] entries;

			public static VsWhereResult FromJson(string json)
			{
				return JsonUtility.FromJson<VsWhereResult>("{ \"" + nameof(VsWhereResult.entries) + "\": " + json + " }");
			}

			public IEnumerable<VisualStudioInstallation> ToVisualStudioInstallations()
			{
				foreach (var entry in entries)
				{
					yield return new VisualStudioInstallation()
					{
						Name = $"{entry.displayName} [{entry.catalog.productDisplayVersion}]",
						Path = entry.productPath,
						IsPrerelease = entry.isPrerelease,
						Version = Version.Parse(entry.catalog.buildVersion)
					};
				}
			}
		}

		[Serializable]
		internal class VsWhereEntry
		{
			public string displayName;
			public bool isPrerelease;
			public string productPath;
			public VsWhereCatalog catalog;
		}

		[Serializable]
		internal class VsWhereCatalog
		{
			public string productDisplayVersion; // non parseable like "16.3.0 Preview 3.0"
			public string buildVersion;
		}
#pragma warning restore CS3021
		#endregion

		private static IEnumerable<VisualStudioInstallation> QueryVsWhere()
		{
			var progpath = _vsWherePath;

			if (string.IsNullOrWhiteSpace(progpath))
				return Enumerable.Empty<VisualStudioInstallation>();

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = progpath,
					Arguments = "-prerelease -format json -utf8",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				}
			};

			using (process)
			{
				var json = string.Empty;

				process.OutputDataReceived += (o, e) => json += e.Data;
				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();

				var result = VsWhereResult.FromJson(json);
				return result.ToVisualStudioInstallations();
			}
		}
	}
}
