using UnityEngine;
using Reflex;
using Reflex.Scripts;

public class InfrastructureInstaller : Installer
{
    [SerializeField] private ScriptableObjectGameSettings _scriptableObjectGameSettings;

    public override void InstallBindings(Container container)
    {
        container.BindInstance<IGameSettings>(_scriptableObjectGameSettings);
        container.BindSingleton<ICollectableRegistry, PlayerPrefsCollectableRegistry>();
    }
}