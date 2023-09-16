using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
{
    [CreateAssetMenu]
    internal class CollectorConfigurationModel : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
    }
}