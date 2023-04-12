using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    [CreateAssetMenu]
    internal class CollectorConfigurationModel : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
    }
}