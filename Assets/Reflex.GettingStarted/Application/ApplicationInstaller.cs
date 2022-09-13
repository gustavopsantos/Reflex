using Reflex;
using Reflex.Scripts;
using UnityEngine;

public class ApplicationInstaller : Installer
{
    [SerializeField] private PickupSoundEffect _pickupSoundEffect;

    public override void InstallBindings(Container container)
    {
        container.BindInstance(_pickupSoundEffect);
        container.BindSingleton<IResetGame, ResetGame>();
        container.BindSingleton<IGetPlayerInput, GetPlayerInput>();
    }
}