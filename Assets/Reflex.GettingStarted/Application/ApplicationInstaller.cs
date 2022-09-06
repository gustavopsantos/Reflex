using Reflex.Scripts;

public class ApplicationInstaller : MonoInstaller
{
    public PickupSoundEffect PickupSoundEffect;
    
    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton(PickupSoundEffect);
        container.BindSingleton<IResetGame, ResetGame>();
        container.BindSingleton<IGetPlayerInput, GetPlayerInput>();
    }
}