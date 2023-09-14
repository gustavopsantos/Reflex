using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reflex.Editor
{
    internal static class UnityEditorUtility
    {
        public static void Focus(Object target)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = target;
        }
        
        public static void CreateScriptableObject<T>(string desiredAssetPath) where T : ScriptableObject
        {
			string assetPath = AssetDatabase.GenerateUniqueAssetPath(desiredAssetPath);
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            Focus(asset);
        }
        
        public static void CreatePrefab(string desiredPrefabPath, Action<GameObject> edit = null)
        {
			string prefabPath =  AssetDatabase.GenerateUniqueAssetPath(desiredPrefabPath);
			GameObject template = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
			GameObject prefab = PrefabUtility.SaveAsPrefabAsset(template, prefabPath);
            Object.DestroyImmediate(template);
            
            using (PrefabUtility.EditPrefabContentsScope editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
            {
                edit?.Invoke(editingScope.prefabContentsRoot);
            }
            
            Focus(prefab);
        }
        
        public static string GetSelectedPathInProjectWindow()
        {
			string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }
    }
}