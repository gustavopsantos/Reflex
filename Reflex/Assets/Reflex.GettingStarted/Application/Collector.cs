using UnityEngine;
using Reflex.Scripts.Attributes;

public class Collector : MonoBehaviour
{
    [Inject] private readonly ICollectableRegistry _collectableRegistry;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Collectable collectable))
        {
            collectable.Collect();
        }
    }
}