using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    [CreateAssetMenu]
    public class CollectorConfigurationModel : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }
    }
}