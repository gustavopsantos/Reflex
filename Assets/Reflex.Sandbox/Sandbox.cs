using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Microsoft.Sandbox
{
    public class Sandbox : MonoBehaviour
    {
        [Button]
        private void LoadBootAdditive()
        {
            SceneManager.LoadScene("Boot", LoadSceneMode.Additive);
        }

        [Button]
        private void LoadBootSingle()
        {
            SceneManager.LoadScene("Boot", LoadSceneMode.Single);
        }

        [Button]
        private void UnloadBoot()
        {
            SceneManager.UnloadSceneAsync("Content");
        }

        [Button]
        private void LoadContentAdditive()
        {
            SceneManager.LoadScene("Content", LoadSceneMode.Additive);
        }

        [Button]
        private void LoadContentSingle()
        {
            SceneManager.LoadScene("Content", LoadSceneMode.Single);
        }

        [Button]
        private void UnloadContent()
        {
            SceneManager.UnloadSceneAsync("Content");
        }
    }
}