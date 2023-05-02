using System.IO;
using UnityEditor;
using UnityEngine;
using Reflex.Core;
using Reflex.Configuration;
using Reflex.Editor.DebuggingWindow;

namespace Reflex.Editor
{
    internal static class ReflexMenuItems
    {
        [MenuItem("Reflex/Debugger %e")]
        public static void OpenReflexDebuggingWindow()
        {
            EditorWindow.GetWindow<ReflexDebuggerWindow>(false, "Reflex Debugger", true);
        }

        [MenuItem("Assets/Create/Reflex/Settings", false, priority = 0)]
        private static void CreateReflexSettings()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, "ReflexSettings.asset");
            UnityEditorUtility.CreateScriptableObject<ReflexSettings>(desiredAssetPath);
        }

        [MenuItem("Assets/Create/Reflex/ProjectScope", false, priority = 0)]
        private static void CreateReflexProjectScope()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, $"{nameof(ProjectScope)}.prefab");

            void Edit(GameObject prefab)
            {
                prefab.AddComponent<ProjectScope>();
            }

            UnityEditorUtility.CreatePrefab(desiredAssetPath, Edit);
        }
        
        [MenuItem("GameObject/Reflex/SceneScope", false, 0)]
        private static void CreateReflexSceneScope()
        {
            var sceneScope = new GameObject(nameof(SceneScope)).AddComponent<SceneScope>();
            Selection.activeObject = sceneScope.gameObject;
        }
    }
}