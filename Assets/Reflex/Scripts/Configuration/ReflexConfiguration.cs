using Reflex.Scripts.Logging;
using UnityEngine;

namespace Assets.Reflex.Scripts.Configuration
{
    public class ReflexConfiguration : ScriptableObject
    {
        public const string AssetName = nameof(ReflexConfiguration);

        [field: SerializeField] public LogLevel LogLevel { get; private set; }
    }
}
