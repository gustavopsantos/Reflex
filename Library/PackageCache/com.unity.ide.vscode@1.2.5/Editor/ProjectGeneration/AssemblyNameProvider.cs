using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;

namespace VSCodeEditor
{
    public interface IAssemblyNameProvider
    {
        string[] ProjectSupportedExtensions { get; }
        ProjectGenerationFlag ProjectGenerationFlag { get; }
        string GetAssemblyNameFromScriptPath(string path);
        IEnumerable<Assembly> GetAssemblies(Func<string, bool> shouldFileBePartOfSolution);
        IEnumerable<string> GetAllAssetPaths();
        IEnumerable<string> GetRoslynAnalyzerPaths();
        UnityEditor.PackageManager.PackageInfo FindForAssetPath(string assetPath);
        ResponseFileData ParseResponseFile(string responseFilePath, string projectDirectory, string[] systemReferenceDirectories);
        bool IsInternalizedPackagePath(string path);
        void ToggleProjectGeneration(ProjectGenerationFlag preference);
    }

    internal interface IPackageInfoCache{
        void ResetPackageInfoCache();
    }

    internal class AssemblyNameProvider : IAssemblyNameProvider, IPackageInfoCache
    {
        private readonly Dictionary<string, UnityEditor.PackageManager.PackageInfo> m_PackageInfoCache = new Dictionary<string, UnityEditor.PackageManager.PackageInfo>();

        ProjectGenerationFlag m_ProjectGenerationFlag = (ProjectGenerationFlag)EditorPrefs.GetInt("unity_project_generation_flag", 0);

        public string[] ProjectSupportedExtensions => EditorSettings.projectGenerationUserExtensions;

        public ProjectGenerationFlag ProjectGenerationFlag
        {
            get => m_ProjectGenerationFlag;
            private set
            {
                EditorPrefs.SetInt("unity_project_generation_flag", (int)value);
                m_ProjectGenerationFlag = value;
            }
        }

        public string GetAssemblyNameFromScriptPath(string path)
        {
            return CompilationPipeline.GetAssemblyNameFromScriptPath(path);
        }

        public IEnumerable<Assembly> GetAssemblies(Func<string, bool> shouldFileBePartOfSolution)
        {
            return CompilationPipeline.GetAssemblies()
                .Where(i => 0 < i.sourceFiles.Length && i.sourceFiles.Any(shouldFileBePartOfSolution));
        }

        public IEnumerable<string> GetAllAssetPaths()
        {
            return AssetDatabase.GetAllAssetPaths();
        }

        private static string ResolvePotentialParentPackageAssetPath(string assetPath)
		{
            const string packagesPrefix = "packages/";
            if (!assetPath.StartsWith(packagesPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var followupSeparator = assetPath.IndexOf('/', packagesPrefix.Length);
            if (followupSeparator == -1)
            {
                return assetPath.ToLowerInvariant();
            }

            return assetPath.Substring(0, followupSeparator).ToLowerInvariant();
		}

        public void ResetPackageInfoCache()
		{
			m_PackageInfoCache.Clear();
		}

        public UnityEditor.PackageManager.PackageInfo FindForAssetPath(string assetPath)
        {
            var parentPackageAssetPath = ResolvePotentialParentPackageAssetPath(assetPath);
			if (parentPackageAssetPath == null)
			{
				return null;
			}

			if (m_PackageInfoCache.TryGetValue(parentPackageAssetPath, out var cachedPackageInfo))
			{
				return cachedPackageInfo;
			}

			var result = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(parentPackageAssetPath);
			m_PackageInfoCache[parentPackageAssetPath] = result;
			return result;
        }

        public ResponseFileData ParseResponseFile(string responseFilePath, string projectDirectory, string[] systemReferenceDirectories)
        {
            return CompilationPipeline.ParseResponseFile(
                responseFilePath,
                projectDirectory,
                systemReferenceDirectories
            );
        }

        public bool IsInternalizedPackagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            var packageInfo = FindForAssetPath(path);
            if (packageInfo == null)
            {
                return false;
            }
            var packageSource = packageInfo.source;
            switch (packageSource)
            {
                case PackageSource.Embedded:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.Embedded);
                case PackageSource.Registry:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.Registry);
                case PackageSource.BuiltIn:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.BuiltIn);
                case PackageSource.Unknown:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.Unknown);
                case PackageSource.Local:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.Local);
                case PackageSource.Git:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.Git);
#if UNITY_2019_3_OR_NEWER
                case PackageSource.LocalTarball:
                    return !ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.LocalTarBall);
#endif
            }

            return false;
        }

        public void ToggleProjectGeneration(ProjectGenerationFlag preference)
        {
            if (ProjectGenerationFlag.HasFlag(preference))
            {
                ProjectGenerationFlag ^= preference;
            }
            else
            {
                ProjectGenerationFlag |= preference;
            }
        }

        public IEnumerable<string> GetRoslynAnalyzerPaths()
        {
            return PluginImporter.GetAllImporters()
                .Where(i => !i.isNativePlugin && AssetDatabase.GetLabels(i).SingleOrDefault(l => l == "RoslynAnalyzer") != null)
                .Select(i => i.assetPath);
        }
    }
}
