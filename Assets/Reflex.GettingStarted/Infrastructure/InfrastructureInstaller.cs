using Reflex.Scripts;
using UnityEngine;

public class InfrastructureInstaller : Installer
{
    [SerializeField] private ScriptableObjectGameSettings _scriptableObjectGameSettings;

    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton<IGameSettings>(_scriptableObjectGameSettings);
        container.BindSingleton<ICollectableRegistry, PlayerPrefsCollectableRegistry>();
    }
}