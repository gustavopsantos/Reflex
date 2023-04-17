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
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(desiredAssetPath);
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            Focus(asset);
        }
        
        public static void CreatePrefab(string desiredPrefabPath, Action<GameObject> edit = null)
        {
            var prefabPath =  AssetDatabase.GenerateUniqueAssetPath(desiredPrefabPath);
            var template = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            var prefab = PrefabUtility.SaveAsPrefabAsset(template, prefabPath);
            Object.DestroyImmediate(template);
            
            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
            {
                edit?.Invoke(editingScope.prefabContentsRoot);
            }
            
            Focus(prefab);
        }
        
        public static string GetSelectedPathInProjectWindow()
        {
            var path = "Assets";

            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
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