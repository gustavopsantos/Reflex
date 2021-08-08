using UnityEngine;
using UnityEngine.UI;
using Reflex.Scripts.Attributes;

public class CollectablesFeedbackPresenter : MonoBehaviour
{
    [SerializeField] private Text _feedbackText;

    [MonoInject] private ICollectableRegistry _collectableRegistry;

    [MonoInject]
    private void Inject(ICollectableRegistry collectableRegistry)
    {
        Present(collectableRegistry);
        _collectableRegistry.OnValueChanged += () => Present(collectableRegistry);
    }

    // private void Start()
    // {
    //     Present();
    //     _collectableRegistry.OnValueChanged += Present;
    // }
    //
    // private void Present()
    // {
    //     _feedbackText.text = $"Collectables {_collectableRegistry.CollectionCount()}/4";
    // }
    
    private void Present(ICollectableRegistry collectableRegistry)
    {
        _feedbackText.text = $"Collectables {collectableRegistry.CollectionCount()}/4";
    }
}