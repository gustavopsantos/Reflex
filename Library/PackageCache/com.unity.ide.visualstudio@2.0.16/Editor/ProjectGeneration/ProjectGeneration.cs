/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SR = System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Unity.CodeEditor;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor
{
	public enum ScriptingLanguage
	{
		None,
		CSharp
	}

	public interface IGenerator
	{
		bool SyncIfNeeded(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles);
		void Sync();
		bool HasSolutionBeenGenerated();
		bool IsSupportedFile(string path);
		string SolutionFile();
		string ProjectDirectory { get; }
		IAssemblyNameProvider AssemblyNameProvider { get; }
	}

	public class ProjectGeneration : IGenerator
	{
		public static readonly string MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";
		public IAssemblyNameProvider AssemblyNameProvider => m_AssemblyNameProvider;
		public string ProjectDirectory { get; }

		const string k_WindowsNewline = "\r\n";

		const string m_SolutionProjectEntryTemplate = @"Project(""{{{0}}}"") = ""{1}"", ""{2}"", ""{{{3}}}""{4}EndProject";

		readonly string m_SolutionProjectConfigurationTemplate = string.Join(k_WindowsNewline,
			@"        {{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
			@"        {{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU",
			@"        {{{0}}}.Release|Any CPU.ActiveCfg = Release|Any CPU",
			@"        {{{0}}}.Release|Any CPU.Build.0 = Release|Any CPU").Replace("    ", "\t");

		static readonly string[] k_ReimportSyncExtensions = { ".dll", ".asmdef" };

		HashSet<string> m_ProjectSupportedExtensions = new HashSet<string>();
		HashSet<string> m_BuiltinSupportedExtensions = new HashSet<string>();

		readonly string m_ProjectName;
		readonly IAssemblyNameProvider m_AssemblyNameProvider;
		readonly IFileIO m_FileIOProvider;
		readonly IGUIDGenerator m_GUIDGenerator;
		bool m_ShouldGenerateAll;
		IVisualStudioInstallation m_CurrentInstallation;

		public ProjectGeneration() : this(Directory.GetParent(Application.dataPath).FullName)
		{
		}

		public ProjectGeneration(string tempDirectory) : this(tempDirectory, new AssemblyNameProvider(), new FileIOProvider(), new GUIDProvider())
		{
		}

		public ProjectGeneration(string tempDirectory, IAssemblyNameProvider assemblyNameProvider, IFileIO fileIoProvider, IGUIDGenerator guidGenerator)
		{
			ProjectDirectory = FileUtility.NormalizeWindowsToUnix(tempDirectory);
			m_ProjectName = Path.GetFileName(ProjectDirectory);
			m_AssemblyNameProvider = assemblyNameProvider;
			m_FileIOProvider = fileIoProvider;
			m_GUIDGenerator = guidGenerator;

			SetupProjectSupportedExtensions();
		}

		/// <summary>
		/// Syncs the scripting solution if any affected files are relevant.
		/// </summary>
		/// <returns>
		/// Whether the solution was synced.
		/// </returns>
		/// <param name='affectedFiles'>
		/// A set of files whose status has changed
		/// </param>
		/// <param name="reimportedFiles">
		/// A set of files that got reimported
		/// </param>
		public bool SyncIfNeeded(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles)
		{
			using (solutionSyncMarker.Auto())
			{
				// We need the exact VS version/capabilities to tweak project generation (analyzers/langversion)
				RefreshCurrentInstallation();

				SetupProjectSupportedExtensions();

				// See https://devblogs.microsoft.com/setup/configure-visual-studio-across-your-organization-with-vsconfig/
				// We create a .vsconfig file to make sure our ManagedGame workload is installed
				CreateVsConfigIfNotFound();

				// Don't sync if we haven't synced before
				var affected = affectedFiles as ICollection<string> ?? affectedFiles.ToArray();
				var reimported = reimportedFiles as ICollection<string> ?? reimportedFiles.ToArray();
				if (!HasFilesBeenModified(affected, reimported))
				{
					return false;
				}

				var assemblies = m_AssemblyNameProvider.GetAssemblies(ShouldFileBePartOfSolution);
				var allProjectAssemblies = RelevantAssembliesForMode(assemblies).ToList();
				SyncSolution(allProjectAssemblies);

				var allAssetProjectParts = GenerateAllAssetProjectParts();

				var affectedNames = affected
					.Select(asset => m_AssemblyNameProvider.GetAssemblyNameFromScriptPath(asset))
					.Where(name => !string.IsNullOrWhiteSpace(name)).Select(name =>
						name.Split(new[] {".dll"}, StringSplitOptions.RemoveEmptyEntries)[0]);
				var reimportedNames = reimported
					.Select(asset => m_AssemblyNameProvider.GetAssemblyNameFromScriptPath(asset))
					.Where(name => !string.IsNullOrWhiteSpace(name)).Select(name =>
						name.Split(new[] {".dll"}, StringSplitOptions.RemoveEmptyEntries)[0]);
				var affectedAndReimported = new HashSet<string>(affectedNames.Concat(reimportedNames));

				foreach (var assembly in allProjectAssemblies)
				{
					if (!affectedAndReimported.Contains(assembly.name))
						continue;

					SyncProject(assembly,
						allAssetProjectParts,
						responseFilesData: ParseResponseFileData(assembly).ToArray());
				}

				return true;
			}
		}

		private void CreateVsConfigIfNotFound()
		{
			try
			{
				var vsConfigFile = VsConfigFile();
				if (m_FileIOProvider.Exists(vsConfigFile))
					return;

				var content = $@"{{
  ""version"": ""1.0"",
  ""components"": [
    ""{Discovery.ManagedWorkload}""
  ]
}}
";
				m_FileIOProvider.WriteAllText(vsConfigFile, content);
			}
			catch (IOException)
			{
			}
		}

		private bool HasFilesBeenModified(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles)
		{
			return affectedFiles.Any(ShouldFileBePartOfSolution) || reimportedFiles.Any(ShouldSyncOnReimportedAsset);
		}

		private static bool ShouldSyncOnReimportedAsset(string asset)
		{
			return k_ReimportSyncExtensions.Contains(new FileInfo(asset).Extension);
		}

		private void RefreshCurrentInstallation()
		{
			var editor = CodeEditor.CurrentEditor as VisualStudioEditor;
			editor?.TryGetVisualStudioInstallationForPath(CodeEditor.CurrentEditorInstallation, searchInstallations: true, out m_CurrentInstallation);
		}

		static ProfilerMarker solutionSyncMarker = new ProfilerMarker("SolutionSynchronizerSync");

		public void Sync()
		{
			// We need the exact VS version/capabilities to tweak project generation (analyzers/langversion)
			RefreshCurrentInstallation();

			SetupProjectSupportedExtensions();

			(m_AssemblyNameProvider as AssemblyNameProvider)?.ResetPackageInfoCache();

			// See https://devblogs.microsoft.com/setup/configure-visual-studio-across-your-organization-with-vsconfig/
			// We create a .vsconfig file to make sure our ManagedGame workload is installed
			CreateVsConfigIfNotFound();

			var externalCodeAlreadyGeneratedProjects = OnPreGeneratingCSProjectFiles();

			if (!externalCodeAlreadyGeneratedProjects)
			{
				GenerateAndWriteSolutionAndProjects();
			}

			OnGeneratedCSProjectFiles();
		}

		public bool HasSolutionBeenGenerated()
		{
			return m_FileIOProvider.Exists(SolutionFile());
		}

		private void SetupProjectSupportedExtensions()
		{
			m_ProjectSupportedExtensions = new HashSet<string>(m_AssemblyNameProvider.ProjectSupportedExtensions);
			m_BuiltinSupportedExtensions = new HashSet<string>(EditorSettings.projectGenerationBuiltinExtensions);
		}

		private bool ShouldFileBePartOfSolution(string file)
		{
			// Exclude files coming from packages except if they are internalized.
			if (m_AssemblyNameProvider.IsInternalizedPackagePath(file))
			{
				return false;
			}

			return IsSupportedFile(file);
		}

		private static string GetExtensionWithoutDot(string path)
		{
			// Prevent re-processing and information loss
			if (!Path.HasExtension(path))
				return path;

			return Path
				.GetExtension(path)
				.TrimStart('.')
				.ToLower();
		}

		public bool IsSupportedFile(string path)
		{
			return IsSupportedFile(path, out _);
		}

		private bool IsSupportedFile(string path, out string extensionWithoutDot)
		{
			extensionWithoutDot = GetExtensionWithoutDot(path);

			// Dll's are not scripts but still need to be included
			if (extensionWithoutDot == "dll")
				return true;

			if (extensionWithoutDot == "asmdef")
				return true;

			if (m_BuiltinSupportedExtensions.Contains(extensionWithoutDot))
				return true;

			if (m_ProjectSupportedExtensions.Contains(extensionWithoutDot))
				return true;

			return false;
		}


		private static ScriptingLanguage ScriptingLanguageFor(Assembly assembly)
		{
			var files = assembly.sourceFiles;

			if (files.Length == 0)
				return ScriptingLanguage.None;

			return ScriptingLanguageForFile(files[0]);
		}

		internal static ScriptingLanguage ScriptingLanguageForExtension(string extensionWithoutDot)
		{
			return extensionWithoutDot == "cs" ? ScriptingLanguage.CSharp : ScriptingLanguage.None;
		}

		internal static ScriptingLanguage ScriptingLanguageForFile(string path)
		{
			return ScriptingLanguageForExtension(GetExtensionWithoutDot(path));
		}

		public void GenerateAndWriteSolutionAndProjects()
		{
			// Only synchronize assemblies that have associated source files and ones that we actually want in the project.
			// This also filters out DLLs coming from .asmdef files in packages.
			var assemblies = m_AssemblyNameProvider.GetAssemblies(ShouldFileBePartOfSolution).ToList();

			var allAssetProjectParts = GenerateAllAssetProjectParts();

			SyncSolution(assemblies);

			var allProjectAssemblies = RelevantAssembliesForMode(assemblies);

			foreach (var assembly in allProjectAssemblies)
			{
				SyncProject(assembly,
					allAssetProjectParts,
					responseFilesData: ParseResponseFileData(assembly).ToArray());
			}
		}

		private IEnumerable<ResponseFileData> ParseResponseFileData(Assembly assembly)
		{
			var systemReferenceDirectories = CompilationPipeline.GetSystemAssemblyDirectories(assembly.compilerOptions.ApiCompatibilityLevel);

			Dictionary<string, ResponseFileData> responseFilesData = assembly.compilerOptions.ResponseFiles.ToDictionary(x => x, x => m_AssemblyNameProvider.ParseResponseFile(
				x,
				ProjectDirectory,
				systemReferenceDirectories
			));

			Dictionary<string, ResponseFileData> responseFilesWithErrors = responseFilesData.Where(x => x.Value.Errors.Any())
				.ToDictionary(x => x.Key, x => x.Value);

			if (responseFilesWithErrors.Any())
			{
				foreach (var error in responseFilesWithErrors)
					foreach (var valueError in error.Value.Errors)
					{
						Debug.LogError($"{error.Key} Parse Error : {valueError}");
					}
			}

			return responseFilesData.Select(x => x.Value);
		}

		private Dictionary<string, string> GenerateAllAssetProjectParts()
		{
			Dictionary<string, StringBuilder> stringBuilders = new Dictionary<string, StringBuilder>();

			foreach (string asset in m_AssemblyNameProvider.GetAllAssetPaths())
			{
				// Exclude files coming from packages except if they are internalized.
				if (m_AssemblyNameProvider.IsInternalizedPackagePath(asset))
				{
					continue;
				}

				if (IsSupportedFile(asset, out var extensionWithoutDot) && ScriptingLanguage.None == ScriptingLanguageForExtension(extensionWithoutDot))
				{
					// Find assembly the asset belongs to by adding script extension and using compilation pipeline.
					var assemblyName = m_AssemblyNameProvider.GetAssemblyNameFromScriptPath(asset);

					if (string.IsNullOrEmpty(assemblyName))
					{
						continue;
					}

					assemblyName = Path.GetFileNameWithoutExtension(assemblyName);

					if (!stringBuilders.TryGetValue(assemblyName, out var projectBuilder))
					{
						projectBuilder = new StringBuilder();
						stringBuilders[assemblyName] = projectBuilder;
					}

					IncludeAsset(projectBuilder, "None", asset);
				}
			}

			var result = new Dictionary<string, string>();

			foreach (var entry in stringBuilders)
				result[entry.Key] = entry.Value.ToString();

			return result;
		}

		private void IncludeAsset(StringBuilder builder, string tag, string asset)
		{
			var filename = EscapedRelativePathFor(asset, out var packageInfo);

			builder.Append($"    <{tag} Include=\"").Append(filename);
			if (Path.IsPathRooted(filename) && packageInfo != null)
			{
				// We are outside the Unity project and using a package context
				var linkPath = SkipPathPrefix(asset.NormalizePathSeparators(), packageInfo.assetPath.NormalizePathSeparators());

				builder.Append("\">").Append(k_WindowsNewline);
				builder.Append("      <Link>").Append(linkPath).Append("</Link>").Append(k_WindowsNewline);
				builder.Append($"    </{tag}>").Append(k_WindowsNewline);
			}
			else
			{
				builder.Append("\" />").Append(k_WindowsNewline);
			}
		}

		private void SyncProject(
			Assembly assembly,
			Dictionary<string, string> allAssetsProjectParts,
			ResponseFileData[] responseFilesData)
		{
			SyncProjectFileIfNotChanged(
				ProjectFile(assembly),
				ProjectText(assembly, allAssetsProjectParts, responseFilesData));
		}

		private void SyncProjectFileIfNotChanged(string path, string newContents)
		{
			if (Path.GetExtension(path) == ".csproj")
			{
				newContents = OnGeneratedCSProject(path, newContents);
			}

			SyncFileIfNotChanged(path, newContents);
		}

		private void SyncSolutionFileIfNotChanged(string path, string newContents)
		{
			newContents = OnGeneratedSlnSolution(path, newContents);

			SyncFileIfNotChanged(path, newContents);
		}

		private static IEnumerable<SR.MethodInfo> GetPostProcessorCallbacks(string name)
		{
			return TypeCache
				.GetTypesDerivedFrom<AssetPostprocessor>()
				.Where(t => t.Assembly.GetName().Name != KnownAssemblies.Bridge) // never call into the bridge if loaded with the package
				.Select(t => t.GetMethod(name, SR.BindingFlags.Public | SR.BindingFlags.NonPublic | SR.BindingFlags.Static))
				.Where(m => m != null);
		}

		static void OnGeneratedCSProjectFiles()
		{
			foreach (var method in GetPostProcessorCallbacks(nameof(OnGeneratedCSProjectFiles)))
			{
				method.Invoke(null, Array.Empty<object>());
			}
		}

		private static bool OnPreGeneratingCSProjectFiles()
		{
			bool result = false;

			foreach (var method in GetPostProcessorCallbacks(nameof(OnPreGeneratingCSProjectFiles)))
			{
				var retValue = method.Invoke(null, Array.Empty<object>());
				if (method.ReturnType == typeof(bool))
				{
					result |= (bool)retValue;
				}
			}

			return result;
		}

		private static string InvokeAssetPostProcessorGenerationCallbacks(string name, string path, string content)
		{
			foreach (var method in GetPostProcessorCallbacks(name))
			{
				var args = new[] { path, content };
				var returnValue = method.Invoke(null, args);
				if (method.ReturnType == typeof(string))
				{
					// We want to chain content update between invocations
					content = (string)returnValue;
				}
			}

			return content;
		}

		private static string OnGeneratedCSProject(string path, string content)
		{
			return InvokeAssetPostProcessorGenerationCallbacks(nameof(OnGeneratedCSProject), path, content);
		}

		private static string OnGeneratedSlnSolution(string path, string content)
		{
			return InvokeAssetPostProcessorGenerationCallbacks(nameof(OnGeneratedSlnSolution), path, content);
		}

		private void SyncFileIfNotChanged(string filename, string newContents)
		{
			try
			{
				if (m_FileIOProvider.Exists(filename) && newContents == m_FileIOProvider.ReadAllText(filename))
				{
					return;
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}

			m_FileIOProvider.WriteAllText(filename, newContents);
		}

		private string ProjectText(Assembly assembly,
			Dictionary<string, string> allAssetsProjectParts,
			ResponseFileData[] responseFilesData)
		{
			var projectBuilder = new StringBuilder(ProjectHeader(assembly, responseFilesData));
			var references = new List<string>();

			projectBuilder.Append(@"  <ItemGroup>").Append(k_WindowsNewline);
			foreach (string file in assembly.sourceFiles)
			{
				if (!IsSupportedFile(file, out var extensionWithoutDot))
					continue;

				if ("dll" != extensionWithoutDot)
				{
					IncludeAsset(projectBuilder, "Compile", file);
				}
				else
				{
					var fullFile = EscapedRelativePathFor(file, out _);
					references.Add(fullFile);
				}
			}
			projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);

			// Append additional non-script files that should be included in project generation.
			if (allAssetsProjectParts.TryGetValue(assembly.name, out var additionalAssetsForProject))
			{
				projectBuilder.Append(@"  <ItemGroup>").Append(k_WindowsNewline);

				projectBuilder.Append(additionalAssetsForProject);

				projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);

			}

			projectBuilder.Append(@"  <ItemGroup>").Append(k_WindowsNewline);

			var responseRefs = responseFilesData.SelectMany(x => x.FullPathReferences.Select(r => r));
			var internalAssemblyReferences = assembly.assemblyReferences
				.Where(i => !i.sourceFiles.Any(ShouldFileBePartOfSolution)).Select(i => i.outputPath);
			var allReferences =
				assembly.compiledAssemblyReferences
					.Union(responseRefs)
					.Union(references)
					.Union(internalAssemblyReferences);

			foreach (var reference in allReferences)
			{
				string fullReference = Path.IsPathRooted(reference) ? reference : Path.Combine(ProjectDirectory, reference);
				AppendReference(fullReference, projectBuilder);
			}

			projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);

			if (0 < assembly.assemblyReferences.Length)
			{
				projectBuilder.Append("  <ItemGroup>").Append(k_WindowsNewline);
				foreach (var reference in assembly.assemblyReferences.Where(i => i.sourceFiles.Any(ShouldFileBePartOfSolution)))
				{
					// If the current assembly is a Player project, we want to project-reference the corresponding Player project
					var referenceName = m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, reference.name);

					projectBuilder.Append("    <ProjectReference Include=\"").Append(referenceName).Append(GetProjectExtension()).Append("\">").Append(k_WindowsNewline);
					projectBuilder.Append("      <Project>{").Append(ProjectGuid(referenceName)).Append("}</Project>").Append(k_WindowsNewline);
					projectBuilder.Append("      <Name>").Append(referenceName).Append("</Name>").Append(k_WindowsNewline);
					projectBuilder.Append("    </ProjectReference>").Append(k_WindowsNewline);
				}

				projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);
			}

			projectBuilder.Append(GetProjectFooter());
			return projectBuilder.ToString();
		}

		private static string XmlFilename(string path)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			path = path.Replace(@"%", "%25");
			path = path.Replace(@";", "%3b");

			return XmlEscape(path);
		}

		private static string XmlEscape(string s)
		{
			return SecurityElement.Escape(s);
		}

		private void AppendReference(string fullReference, StringBuilder projectBuilder)
		{
			var escapedFullPath = EscapedRelativePathFor(fullReference, out _);
			projectBuilder.Append("    <Reference Include=\"").Append(Path.GetFileNameWithoutExtension(escapedFullPath)).Append("\">").Append(k_WindowsNewline);
			projectBuilder.Append("      <HintPath>").Append(escapedFullPath).Append("</HintPath>").Append(k_WindowsNewline);
			projectBuilder.Append("    </Reference>").Append(k_WindowsNewline);
		}

		public string ProjectFile(Assembly assembly)
		{
			return Path.Combine(ProjectDirectory, $"{m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, assembly.name)}.csproj");
		}

		private static readonly Regex InvalidCharactersRegexPattern = new Regex(@"\?|&|\*|""|<|>|\||#|%|\^|;" + (VisualStudioEditor.IsWindows ? "" : "|:"));

		public string SolutionFile()
		{
			return Path.Combine(ProjectDirectory.NormalizePathSeparators(), $"{InvalidCharactersRegexPattern.Replace(m_ProjectName, "_")}.sln");
		}

		internal string VsConfigFile()
		{
			return Path.Combine(ProjectDirectory.NormalizePathSeparators(), ".vsconfig");
		}

		internal string GetLangVersion(Assembly assembly)
		{
			var targetLanguageVersion = "latest"; // danger: latest is not the same absolute value depending on the VS version.
			if (m_CurrentInstallation != null)
			{
				var vsLanguageSupport = m_CurrentInstallation.LatestLanguageVersionSupported;
				var unityLanguageSupport = UnityInstallation.LatestLanguageVersionSupported(assembly);

				// Use the minimal supported version between VS and Unity, so that compilation will work in both
				targetLanguageVersion = (vsLanguageSupport <= unityLanguageSupport ? vsLanguageSupport : unityLanguageSupport).ToString(2); // (major, minor) only
			}

			return targetLanguageVersion;
		}

		private string ProjectHeader(
			Assembly assembly,
			ResponseFileData[] responseFilesData
		)
		{
			var projectType = ProjectTypeOf(assembly.name);
			string rulesetPath = null;
			var analyzers = Array.Empty<string>();

			if (m_CurrentInstallation != null && m_CurrentInstallation.SupportsAnalyzers)
			{
				analyzers = m_CurrentInstallation.GetAnalyzers();
#if UNITY_2020_2_OR_NEWER
				analyzers = analyzers != null ? analyzers.Concat(assembly.compilerOptions.RoslynAnalyzerDllPaths).ToArray() : assembly.compilerOptions.RoslynAnalyzerDllPaths;
				rulesetPath = assembly.compilerOptions.RoslynAnalyzerRulesetPath;
#endif
			}

			var projectProperties = new ProjectProperties()
			{
				ProjectGuid = ProjectGuid(assembly),
				LangVersion = GetLangVersion(assembly),
				AssemblyName = assembly.name,
				RootNamespace = GetRootNamespace(assembly),
				OutputPath = assembly.outputPath,
				// Analyzers
				Analyzers = analyzers,
				RulesetPath = rulesetPath,
				// RSP alterable
				Defines = assembly.defines.Concat(responseFilesData.SelectMany(x => x.Defines)).Distinct().ToArray(),
				Unsafe = assembly.compilerOptions.AllowUnsafeCode | responseFilesData.Any(x => x.Unsafe),
				// VSTU Flavoring
				FlavoringProjectType = projectType + ":" + (int)projectType,
				FlavoringBuildTarget = EditorUserBuildSettings.activeBuildTarget + ":" + (int)EditorUserBuildSettings.activeBuildTarget,
				FlavoringUnityVersion = Application.unityVersion,
				FlavoringPackageVersion = VisualStudioIntegration.PackageVersion(),
			};

			return GetProjectHeader(projectProperties);
		}

		private enum ProjectType
		{
			GamePlugins = 3,
			Game = 1,
			EditorPlugins = 7,
			Editor = 5,
		}

		private static ProjectType ProjectTypeOf(string fileName)
		{
			var plugins = fileName.Contains("firstpass");
			var editor = fileName.Contains("Editor");

			if (plugins && editor)
				return ProjectType.EditorPlugins;
			if (plugins)
				return ProjectType.GamePlugins;
			if (editor)
				return ProjectType.Editor;

			return ProjectType.Game;
		}

		private string GetProjectHeader(ProjectProperties properties)
		{
			var header = new[]
			{
				$@"<?xml version=""1.0"" encoding=""utf-8""?>",
				$@"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">",
				$@"  <PropertyGroup>",
				$@"    <LangVersion>{properties.LangVersion}</LangVersion>",
				$@"  </PropertyGroup>",
				$@"  <PropertyGroup>",
				$@"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>",
				$@"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>",
				$@"    <ProductVersion>10.0.20506</ProductVersion>",
				$@"    <SchemaVersion>2.0</SchemaVersion>",
				$@"    <RootNamespace>{properties.RootNamespace}</RootNamespace>",
				$@"    <ProjectGuid>{{{properties.ProjectGuid}}}</ProjectGuid>",
				$@"    <OutputType>Library</OutputType>",
				$@"    <AppDesignerFolder>Properties</AppDesignerFolder>",
				$@"    <AssemblyName>{properties.AssemblyName}</AssemblyName>",
				$@"    <TargetFrameworkVersion>v4.7.1</TargetFrameworkVersion>",
				$@"    <FileAlignment>512</FileAlignment>",
				$@"    <BaseDirectory>.</BaseDirectory>",
				$@"  </PropertyGroup>",
				$@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">",
				$@"    <DebugSymbols>true</DebugSymbols>",
				$@"    <DebugType>full</DebugType>",
				$@"    <Optimize>false</Optimize>",
				$@"    <OutputPath>{properties.OutputPath}</OutputPath>",
				$@"    <DefineConstants>{string.Join(";", properties.Defines)}</DefineConstants>",
				$@"    <ErrorReport>prompt</ErrorReport>",
				$@"    <WarningLevel>4</WarningLevel>",
				$@"    <NoWarn>0169</NoWarn>",
				$@"    <AllowUnsafeBlocks>{properties.Unsafe}</AllowUnsafeBlocks>",
				$@"  </PropertyGroup>",
				$@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">",
				$@"    <DebugType>pdbonly</DebugType>",
				$@"    <Optimize>true</Optimize>",
				$@"    <OutputPath>Temp\bin\Release\</OutputPath>",
				$@"    <ErrorReport>prompt</ErrorReport>",
				$@"    <WarningLevel>4</WarningLevel>",
				$@"    <NoWarn>0169</NoWarn>",
				$@"    <AllowUnsafeBlocks>{properties.Unsafe}</AllowUnsafeBlocks>",
				$@"  </PropertyGroup>"
			};

			var forceExplicitReferences = new[]
			{
				$@"  <PropertyGroup>",
				$@"    <NoConfig>true</NoConfig>",
				$@"    <NoStdLib>true</NoStdLib>",
				$@"    <AddAdditionalExplicitAssemblyReferences>false</AddAdditionalExplicitAssemblyReferences>",
				$@"    <ImplicitlyExpandNETStandardFacades>false</ImplicitlyExpandNETStandardFacades>",
				$@"    <ImplicitlyExpandDesignTimeFacades>false</ImplicitlyExpandDesignTimeFacades>",
				$@"  </PropertyGroup>"
			};

			var flavoring = new[]
			{
				$@"  <PropertyGroup>",
				$@"    <ProjectTypeGuids>{{E097FAD1-6243-4DAD-9C02-E9B9EFC3FFC1}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>",
				$@"    <UnityProjectGenerator>Package</UnityProjectGenerator>",
				$@"    <UnityProjectGeneratorVersion>{properties.FlavoringPackageVersion}</UnityProjectGeneratorVersion>",
				$@"    <UnityProjectType>{properties.FlavoringProjectType}</UnityProjectType>",
				$@"    <UnityBuildTarget>{properties.FlavoringBuildTarget}</UnityBuildTarget>",
				$@"    <UnityVersion>{properties.FlavoringUnityVersion}</UnityVersion>",
				$@"  </PropertyGroup>"
			};

			var footer = new[]
			{
				@""
			};

			var lines = header
				.Concat(forceExplicitReferences)
				.Concat(flavoring)
				.ToList();

			if (!string.IsNullOrEmpty(properties.RulesetPath))
			{
				lines.Add(@"  <PropertyGroup>");
				lines.Add($"    <CodeAnalysisRuleSet>{properties.RulesetPath.MakeAbsolutePath().NormalizePathSeparators()}</CodeAnalysisRuleSet>");
				lines.Add(@"  </PropertyGroup>");
			}

			if (properties.Analyzers.Any())
			{
				lines.Add(@"  <ItemGroup>");
				foreach (var analyzer in properties.Analyzers.Distinct())
				{
					lines.Add($@"    <Analyzer Include=""{analyzer.MakeAbsolutePath().NormalizePathSeparators()}"" />");
				}
				lines.Add(@"  </ItemGroup>");
			}

			return string.Join(k_WindowsNewline, lines.Concat(footer));
		}

		private static string GetProjectFooter()
		{
			return string.Join(k_WindowsNewline,
			@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />",
			@"  <Target Name=""GenerateTargetFrameworkMonikerAttribute"" />",
			@"  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.",
			@"       Other similar extension points exist, see Microsoft.Common.targets.",
			@"  <Target Name=""BeforeBuild"">",
			@"  </Target>",
			@"  <Target Name=""AfterBuild"">",
			@"  </Target>",
			@"  -->",
			@"</Project>",
			@"");
		}

		private static string GetSolutionText()
		{
			return string.Join(k_WindowsNewline,
			@"",
			@"Microsoft Visual Studio Solution File, Format Version {0}",
			@"# Visual Studio {1}",
			@"{2}",
			@"Global",
			@"    GlobalSection(SolutionConfigurationPlatforms) = preSolution",
			@"        Debug|Any CPU = Debug|Any CPU",
			@"        Release|Any CPU = Release|Any CPU",
			@"    EndGlobalSection",
			@"    GlobalSection(ProjectConfigurationPlatforms) = postSolution",
			@"{3}",
			@"    EndGlobalSection",
			@"{4}",
			@"EndGlobal",
			@"").Replace("    ", "\t");
		}

		private void SyncSolution(IEnumerable<Assembly> assemblies)
		{
			if (InvalidCharactersRegexPattern.IsMatch(ProjectDirectory))
				Debug.LogWarning("Project path contains special characters, which can be an issue when opening Visual Studio");

			var solutionFile = SolutionFile();
			var previousSolution = m_FileIOProvider.Exists(solutionFile) ? SolutionParser.ParseSolutionFile(solutionFile, m_FileIOProvider) : null;
			SyncSolutionFileIfNotChanged(solutionFile, SolutionText(assemblies, previousSolution));
		}

		private string SolutionText(IEnumerable<Assembly> assemblies, Solution previousSolution = null)
		{
			const string fileversion = "12.00";
			const string vsversion = "15";

			var relevantAssemblies = RelevantAssembliesForMode(assemblies);
			var generatedProjects = ToProjectEntries(relevantAssemblies).ToList();

			SolutionProperties[] properties = null;

			// First, add all projects generated by Unity to the solution
			var projects = new List<SolutionProjectEntry>();
			projects.AddRange(generatedProjects);

			if (previousSolution != null)
			{
				// Add all projects that were previously in the solution and that are not generated by Unity, nor generated in the project root directory
				var externalProjects = previousSolution.Projects
					.Where(p => p.IsSolutionFolderProjectFactory() || !FileUtility.IsFileInProjectRootDirectory(p.FileName))
					.Where(p => generatedProjects.All(gp => gp.FileName != p.FileName));

				projects.AddRange(externalProjects);
				properties = previousSolution.Properties;
			}

			string propertiesText = GetPropertiesText(properties);
			string projectEntriesText = GetProjectEntriesText(projects);

			// do not generate configurations for SolutionFolders
			var configurableProjects = projects.Where(p => !p.IsSolutionFolderProjectFactory());
			string projectConfigurationsText = string.Join(k_WindowsNewline, configurableProjects.Select(p => GetProjectActiveConfigurations(p.ProjectGuid)).ToArray());

			return string.Format(GetSolutionText(), fileversion, vsversion, projectEntriesText, projectConfigurationsText, propertiesText);
		}

		private static IEnumerable<Assembly> RelevantAssembliesForMode(IEnumerable<Assembly> assemblies)
		{
			return assemblies.Where(i => ScriptingLanguage.CSharp == ScriptingLanguageFor(i));
		}

		private static string GetPropertiesText(SolutionProperties[] array)
		{
			if (array == null || array.Length == 0)
			{
				// HideSolution by default
				array = new [] {
					new SolutionProperties() {
						Name = "SolutionProperties",
						Type = "preSolution",
						Entries = new List<KeyValuePair<string,string>>() { new KeyValuePair<string, string> ("HideSolutionNode", "FALSE") }
					}
				};
			}
			var result = new StringBuilder();

			for (var i = 0; i < array.Length; i++)
			{
				if (i > 0)
					result.Append(k_WindowsNewline);

				var properties = array[i];

				result.Append($"\tGlobalSection({properties.Name}) = {properties.Type}");
				result.Append(k_WindowsNewline);

				foreach (var entry in properties.Entries)
				{
					result.Append($"\t\t{entry.Key} = {entry.Value}");
					result.Append(k_WindowsNewline);
				}

				result.Append("\tEndGlobalSection");
			}

			return result.ToString();
		}

		/// <summary>
		/// Get a Project("{guid}") = "MyProject", "MyProject.unityproj", "{projectguid}"
		/// entry for each relevant language
		/// </summary>
		private string GetProjectEntriesText(IEnumerable<SolutionProjectEntry> entries)
		{
			var projectEntries = entries.Select(entry => string.Format(
				m_SolutionProjectEntryTemplate,
				entry.ProjectFactoryGuid, entry.Name, entry.FileName, entry.ProjectGuid, entry.Metadata
			));

			return string.Join(k_WindowsNewline, projectEntries.ToArray());
		}

		private IEnumerable<SolutionProjectEntry> ToProjectEntries(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
				yield return new SolutionProjectEntry()
				{
					ProjectFactoryGuid = SolutionGuid(assembly),
					Name = assembly.name,
					FileName = Path.GetFileName(ProjectFile(assembly)),
					ProjectGuid = ProjectGuid(assembly),
					Metadata = k_WindowsNewline
				};
		}

		/// <summary>
		/// Generate the active configuration string for a given project guid
		/// </summary>
		private string GetProjectActiveConfigurations(string projectGuid)
		{
			return string.Format(
				m_SolutionProjectConfigurationTemplate,
				projectGuid);
		}

		private string EscapedRelativePathFor(string file, out UnityEditor.PackageManager.PackageInfo packageInfo)
		{
			var projectDir = ProjectDirectory.NormalizePathSeparators();
			file = file.NormalizePathSeparators();
			var path = SkipPathPrefix(file, projectDir);

			packageInfo = m_AssemblyNameProvider.FindForAssetPath(path.NormalizeWindowsToUnix());
			if (packageInfo != null)
			{
				// We have to normalize the path, because the PackageManagerRemapper assumes
				// dir seperators will be os specific.
				var absolutePath = Path.GetFullPath(path.NormalizePathSeparators());
				path = SkipPathPrefix(absolutePath, projectDir);
			}

			return XmlFilename(path);
		}

		private static string SkipPathPrefix(string path, string prefix)
		{
			if (path.StartsWith($"{prefix}{Path.DirectorySeparatorChar}") && (path.Length > prefix.Length))
				return path.Substring(prefix.Length + 1);
			return path;
		}

		static string GetProjectExtension()
		{
			return ".csproj";
		}

		private string ProjectGuid(string assemblyName)
		{
			return m_GUIDGenerator.ProjectGuid(m_ProjectName, assemblyName);
		}

		private string ProjectGuid(Assembly assembly)
		{
			return ProjectGuid(m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, assembly.name));
		}

		private string SolutionGuid(Assembly assembly)
		{
			return m_GUIDGenerator.SolutionGuid(m_ProjectName, ScriptingLanguageFor(assembly));
		}

		private static string GetRootNamespace(Assembly assembly)
		{
#if UNITY_2020_2_OR_NEWER
			return assembly.rootNamespace;
#else
			return EditorSettings.projectGenerationRootNamespace;
#endif
		}
	}

	public static class SolutionGuidGenerator
	{
		public static string GuidForProject(string projectName)
		{
			return ComputeGuidHashFor(projectName + "salt");
		}

		public static string GuidForSolution(string projectName, ScriptingLanguage language)
		{
			if (language == ScriptingLanguage.CSharp)
			{
				// GUID for a C# class library: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
				return "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
			}

			return ComputeGuidHashFor(projectName);
		}

		private static string ComputeGuidHashFor(string input)
		{
			var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
			return HashAsGuid(HashToString(hash));
		}

		private static string HashAsGuid(string hash)
		{
			var guid = hash.Substring(0, 8) + "-" + hash.Substring(8, 4) + "-" + hash.Substring(12, 4) + "-" + hash.Substring(16, 4) + "-" + hash.Substring(20, 12);
			return guid.ToUpper();
		}

		private static string HashToString(byte[] bs)
		{
			var sb = new StringBuilder();
			foreach (byte b in bs)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}
	}
}
