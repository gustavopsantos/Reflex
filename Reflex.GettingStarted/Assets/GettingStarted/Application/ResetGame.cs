using Reflex.Scripts;

public class ResetGame : IResetGame
{
    private readonly IContainer _container;
    private readonly ICollectableRegistry _collectableRegistry;

    public ResetGame(IContainer container, ICollectableRegistry collectableRegistry)
    {
        _container = container;
        _collectableRegistry = collectableRegistry;
    }

    public void Reset()
    {
        _collectableRegistry.Clear();
        _container.Resolve<IPlayerMovement>().ResetGame();
        ObjectUtilities.FindObjectsOfType<Collectable>(true).ForEach(collectable => collectable.Enable());
    }
}