using Reflex;
using Reflex.Scripts;
using UnityEngine;

public class InfrastructureInstaller : MonoInstaller
{
    [SerializeField] private ScriptableObjectGameSettings _scriptableObjectGameSettings;

    public override void InstallBindings(Container container)
    {
        container.BindSingleton<IGameSettings>(_scriptableObjectGameSettings);
        container.Bind<ICollectableRegistry>().To<CollectableRegistry>().AsLazySingleton();
    }
}