using Reflex;
using Reflex.Scripts;

public class ApplicationInstaller : MonoInstaller
{
    public PickupSoundEffect PickupSoundEffect;
    
    public override void InstallBindings(IContainer container)
    {
        container.Bind<IResetGame>().To<ResetGame>().AsSingletonNonLazy();
        container.Bind<IGetPlayerInput>().To<GetPlayerInput>().AsSingletonNonLazy();
        container.BindSingleton(PickupSoundEffect);
    }
}