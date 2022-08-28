using Reflex.Scripts;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton<int>(200);
        container.BindSingleton<string>("Project");
    }
}