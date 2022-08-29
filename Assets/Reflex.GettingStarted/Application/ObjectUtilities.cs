using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class ObjectUtilities
{
    public static List<T> FindObjectsOfType<T>(bool includeInactive)
    {
        return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(rootGameObject => rootGameObject.GetComponentsInChildren<T>(includeInactive)).ToList();
    }

    public static T FindObjectOfType<T>(bool includeInactive)
    {
        return FindObjectsOfType<T>(includeInactive).First();
    }
}