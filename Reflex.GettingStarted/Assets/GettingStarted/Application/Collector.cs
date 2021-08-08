using UnityEngine;
using Reflex.Scripts.Attributes;

public class Collector : MonoBehaviour
{
    [MonoInject] private readonly ICollectableRegistry _collectableRegistry;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Collectable collectable))
        {
            collectable.Collect();
        }
    }
}