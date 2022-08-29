using Reflex.Scripts;

public class SceneOneInstaller : MonoInstaller
{
    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton<string>("SceneOne");
    }
}