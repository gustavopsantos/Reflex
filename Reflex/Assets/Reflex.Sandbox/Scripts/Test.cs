using EasyButtons;
using Reflex.Scripts;
using Reflex.Scripts.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    [MonoInject] private IContainer _container;

    [Button] private void Log() => Debug.Log(_container.GetType().Name);
    [Button] private void ResolveInt() => Debug.Log(_container.Resolve<int>());
    [Button] private void ResolveString() => Debug.Log(_container.Resolve<string>());
    [Button] private void Bind() => _container.BindSingleton<string>("Override");
    [Button] private void OpenSceneOne() => SceneManager.LoadScene("SceneOne");
    [Button] private void OpenSceneTwo() => SceneManager.LoadScene("SceneTwo");
    [Button] private void SpawnPlayer()
    {
        var prefab = Resources.Load<Player>("Player");
        _container.Instantiate(prefab);
    }
}