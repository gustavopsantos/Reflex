using System.IO;
using UnityEditor;
using UnityEngine;
using Reflex.Core;
using Reflex.Configuration;
using Reflex.Editor.DebuggingWindow;
using UnityEditor.SceneManagement;

namespace Reflex.Editor
{
    internal static class ReflexMenuItems
    {
        [MenuItem("Window/Analysis/Reflex Debugger %e")]
        private static void OpenReflexDebuggingWindow()
        {
            EditorWindow.GetWindow<ReflexDebuggerWindow>(false, "Reflex Debugger", true);
        }

        [MenuItem("Assets/Create/Reflex/Settings")]
        private static void CreateReflexSettings()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, "ReflexSettings.asset");
            UnityEditorUtility.CreateScriptableObject<ReflexSettings>(desiredAssetPath);
        }

        [MenuItem("Assets/Create/Reflex/RootScope")]
        private static void CreateReflexRootScope()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, "RootScope.prefab");

            void Edit(GameObject prefab)
            {
                prefab.AddComponent<ContainerScope>();
            }

            UnityEditorUtility.CreatePrefab(desiredAssetPath, Edit);
        }
        
        [MenuItem("GameObject/Reflex/ContainerScope")]
        private static void CreateReflexContainerScope()
        {
            var containerScope = new GameObject("ContainerScope").AddComponent<ContainerScope>();
            Selection.activeObject = containerScope.gameObject;
            EditorSceneManager.MarkSceneDirty(containerScope.gameObject.scene);
        }
    }
}