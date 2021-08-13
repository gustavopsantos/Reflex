using Reflex.Scripts.Attributes;
using UnityEngine.Scripting;

public class CollectablesFeedbackPresenter : Presenter<CollectablesFeedbackView>
{
    [MonoInject, Preserve]
    private void Inject(ICollectableRegistry collectableRegistry)
    {
        Present(collectableRegistry);
        collectableRegistry.OnValueChanged += () => Present(collectableRegistry);
    }

    private void Present(ICollectableRegistry collectableRegistry)
    {
        View.FeedbackText.text = $"Collectables {collectableRegistry.CollectionCount()}/4";
    }
}