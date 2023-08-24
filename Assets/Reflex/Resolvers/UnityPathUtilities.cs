using UnityEngine;

public static class UnityPathUtilities
{
    public static string GetUnityPath(string path)
    {
        return path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
    }
}