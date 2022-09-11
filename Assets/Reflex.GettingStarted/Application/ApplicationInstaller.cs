using Reflex.Scripts;
using UnityEngine;

public class ApplicationInstaller : Installer
{
    [SerializeField] private PickupSoundEffect _pickupSoundEffect;

    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton(_pickupSoundEffect);
        container.BindSingleton<IResetGame, ResetGame>();
        container.BindSingleton<IGetPlayerInput, GetPlayerInput>();
    }
}