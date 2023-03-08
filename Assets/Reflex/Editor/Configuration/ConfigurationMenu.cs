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
            string path = $"Assets/{nameof(ReflexConfiguration)}.asset";

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created configuration at '{path}'");
        }

        private static bool ConfigurationAlreadyExists()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ReflexConfiguration)}");

            if (guids.Length == 0)
                return false;

            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            if (paths.Count() > 1)
            {
                Debug.LogWarning($"Multiple configuration files found ({string.Join(", ", paths)}), "
                    + "it can lead to an unexpected behaviour. Consider removing one of them.");
            }
            else
            {
                Debug.Log($"Configuration file already exists at {paths.First()}");
            }

            return true;
        }
    }
}
