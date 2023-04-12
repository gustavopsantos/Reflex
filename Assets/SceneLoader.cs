using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Button]
    private void LoadScene(string scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene, mode);
    }

    [Button]
    private void UnloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}