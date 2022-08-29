/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using Microsoft.Win32;
using Unity.CodeEditor;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal interface IVisualStudioInstallation
	{
		string Path { get; }
		bool SupportsAnalyzers { get; }
		Version LatestLanguageVersionSupported { get; }
		string[] GetAnalyzers();
		CodeEditor.Installation ToCodeEditorInstallation();
	}

	internal class VisualStudioInstallation : IVisualStudioInstallation
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public Version Version { get; set; }
		public bool IsPrerelease { get; set; }

		public bool SupportsAnalyzers
		{
			get
			{
				if (VisualStudioEditor.IsWindows)
					return Version >= new Version(16, 3);

				if (VisualStudioEditor.IsOSX)
					return Version >= new Version(8, 3);

				return false;
			}
		}

		// C# language version support for Visual Studio
		private static VersionPair[] WindowsVersionTable =
		{
			// VisualStudio 2019
			new VersionPair(16,8, /* => */ 9,0),
			new VersionPair(16,0, /* => */ 8,0),
			
			// VisualStudio 2017
			new VersionPair(15,7, /* => */ 7,3),
			new VersionPair(15,5, /* => */ 7,2),
			new VersionPair(15,3, /* => */ 7,1),
			new VersionPair(15,0, /* => */ 7,0),
		};

		// C# language version support for Visual Studio for Mac
		private static VersionPair[] OSXVersionTable =
		{
			// VisualStudio for Mac 8.x
			new VersionPair(8,8, /* => */ 9,0),
			new VersionPair(8,3, /* => */ 8,0),
			new VersionPair(8,0, /* => */ 7,3),
		};

		public Version LatestLanguageVersionSupported
		{
			get
			{
				VersionPair[] versions = null;

				if (VisualStudioEditor.IsWindows)
					versions = WindowsVersionTable;

				if (VisualStudioEditor.IsOSX)
					versions = OSXVersionTable;

				if (versions != null)
				{
					foreach (var entry in versions)
					{
						if (Version >= entry.IdeVersion)
							return entry.LanguageVersion;
					}
				}

				// default to 7.0 given we support at least VS 2017
				return new Version(7, 0);
			}
		}

		private static string ReadRegistry(RegistryKey hive, string keyName, string valueName)
		{
			try
			{
				var unitykey = hive.OpenSubKey(keyName);

				var result = (string)unitykey?.GetValue(valueName);
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private string GetWindowsBridgeFromRegistry()
		{
			var keyName = $"Software\\Microsoft\\Microsoft Visual Studio {Version.Major}.0 Tools for Unity";
			const string valueName = "UnityExtensionPath";

			var bridge = ReadRegistry(Registry.CurrentUser, keyName, valueName);
			if (string.IsNullOrEmpty(bridge))
				bridge = ReadRegistry(Registry.LocalMachine, keyName, valueName);

			return bridge;
		}

		// We only use this to find analyzers, we do not need to load this assembly anymore
		private string GetExtensionPath()
		{
			if (VisualStudioEditor.IsWindows)
			{
				const string extensionName = "Visual Studio Tools for Unity";
				const string extensionAssembly = "SyntaxTree.VisualStudio.Unity.dll";

				var vsDirectory = IOPath.GetDirectoryName(Path);
				var vstuDirectory = IOPath.Combine(vsDirectory, "Extensions", "Microsoft", extensionName);

				if (File.Exists(IOPath.Combine(vstuDirectory, extensionAssembly)))
					return vstuDirectory;
			}

			if (VisualStudioEditor.IsOSX)
			{
				const string addinName = "MonoDevelop.Unity";
				const string addinAssembly = addinName + ".dll";

				// user addins repository
				var localAddins = IOPath.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.Personal),
					$"Library/Application Support/VisualStudio/${Version.Major}.0" + "/LocalInstall/Addins");

				// In the user addins repository, the addins are suffixed by their versions, like `MonoDevelop.Unity.1.0`
				// When installing another local user addin, MD will remove files inside the folder
				// So we browse all VSTUM addins, and return the one with an addin assembly
				if (Directory.Exists(localAddins))
				{
					foreach (var folder in Directory.GetDirectories(localAddins, addinName + "*", SearchOption.TopDirectoryOnly))
					{
						if (File.Exists(IOPath.Combine(folder, addinAssembly)))
							return folder;
					}
				}

				// Check in Visual Studio.app/
				// In that case the name of the addin is used
				var addinPath = IOPath.Combine(Path, $"Contents/Resources/lib/monodevelop/AddIns/{addinName}");
				if (File.Exists(IOPath.Combine(addinPath, addinAssembly)))
					return addinPath;

				addinPath = IOPath.Combine(Path, $"Contents/MonoBundle/Addins/{addinName}");
				if (File.Exists(IOPath.Combine(addinPath, addinAssembly)))
					return addinPath;
			}

			return null;
		}

		private static string[] GetAnalyzers(string path)
		{
			var analyzersDirectory = IOPath.GetFullPath(IOPath.Combine(path, "Analyzers"));

			if (Directory.Exists(analyzersDirectory))
				return Directory.GetFiles(analyzersDirectory, "*Analyzers.dll", SearchOption.AllDirectories);

			return Array.Empty<string>();
		}

		public string[] GetAnalyzers()
		{
			var vstuPath = GetExtensionPath();
			if (string.IsNullOrEmpty(vstuPath))
				return Array.Empty<string>();

			if (VisualStudioEditor.IsOSX)
				return GetAnalyzers(vstuPath);

			if (VisualStudioEditor.IsWindows)
			{
				var analyzers = GetAnalyzers(vstuPath);
				if (analyzers?.Length > 0)
					return analyzers;

				var bridge = GetWindowsBridgeFromRegistry();
				if (File.Exists(bridge))
					return GetAnalyzers(IOPath.Combine(IOPath.GetDirectoryName(bridge), ".."));
			}

			// Local assets
			// return FileUtility.FindPackageAssetFullPath("Analyzers a:packages", ".Analyzers.dll");
			return Array.Empty<string>();
		}

		public CodeEditor.Installation ToCodeEditorInstallation()
		{
			return new CodeEditor.Installation() { Name = Name, Path = Path };
		}
	}
}
