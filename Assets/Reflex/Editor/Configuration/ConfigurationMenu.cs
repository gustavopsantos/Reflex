using System.Linq;
using Assets.Reflex.Scripts.Configuration;
using UnityEditor;
using UnityEngine;

namespace Assets.Reflex.Editor.Configuration
{
    public class ConfigurationMenu : MonoBehaviour
    {
        [MenuItem("Reflex/Configuration/Create Configuration")]
        private static void CreateConfiguration()
        {
            if (ConfigurationAlreadyExists())
                return;

            CreateConfigurationAsset();
        }

        private static void CreateConfigurationAsset()
        {
            ReflexConfiguration asset = ScriptableObject.CreateInstance<ReflexConfiguration>();
            string path = $"Assets/Resources/{ReflexConfiguration.AssetName}.asset";

            if (!AssetDatabase.IsValidFolder($"Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created configuration at {path}");
        }

        private static bool ConfigurationAlreadyExists()
        {
            var assets = Resources.LoadAll<ReflexConfiguration>(ReflexConfiguration.AssetName);

            if (assets.Length == 0)
                return false;

            var paths = assets.Select(asset => AssetDatabase.GetAssetPath(asset));

            if (assets.Length == 1)
            {
                Debug.Log($"Configuration file already exists at {paths.First()}");
            }
            else
            {
                Debug.LogWarning($"Multiple Reflex configuration files found ({string.Join(", ", paths)}), "
                    + "it can lead to an unexpected behaviour. Consider removing one of them.");
            }

            return true;
        }
    }
}
