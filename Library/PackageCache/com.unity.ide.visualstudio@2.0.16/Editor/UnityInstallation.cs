/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using UnityEditor;
using UnityEditor.Compilation;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class UnityInstallation
	{
		public static bool IsMainUnityEditorProcess
		{
			get
			{
#if UNITY_2020_2_OR_NEWER
				if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
					return false;
#elif UNITY_2019_3_OR_NEWER
				if (UnityEditor.Experimental.AssetDatabaseExperimental.IsAssetImportWorkerProcess())
					return false;
#endif

#if UNITY_2021_1_OR_NEWER
				if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Secondary)
					return false;
#elif UNITY_2020_2_OR_NEWER
				if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Slave)
					return false;
#elif UNITY_2020_1_OR_NEWER
				if (global::Unity.MPE.ProcessService.level == global::Unity.MPE.ProcessLevel.UMP_SLAVE)
					return false;
#endif

				return true;
			}
		}

		private static readonly Lazy<bool> _lazyIsInSafeMode = new Lazy<bool>(() =>
		{
			// internal static extern bool isInSafeMode { get {} }
			var ieu = typeof(EditorUtility);
			var pinfo = ieu.GetProperty("isInSafeMode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			if (pinfo == null)
				return false;

			return Convert.ToBoolean(pinfo.GetValue(null));
		});
		public static bool IsInSafeMode => _lazyIsInSafeMode.Value;
		public static Version LatestLanguageVersionSupported(Assembly assembly)
		{
#if UNITY_2020_2_OR_NEWER
			if (assembly?.compilerOptions != null && Version.TryParse(assembly.compilerOptions.LanguageVersion, out var result))
				return result;

			// if parsing fails, we know at least we have support for 8.0
			return new Version(8, 0);
#else
			return new Version(7, 3);
#endif
		}

	}
}
