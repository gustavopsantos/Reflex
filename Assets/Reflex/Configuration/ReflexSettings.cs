using Reflex.Logging;
using UnityEngine;

namespace Reflex.Configuration
{
    internal sealed class ReflexSettings : ScriptableObject
    {
        [field: SerializeField] public LogLevel LogLevel { get; private set; }
        [field: SerializeField] public bool LoadAllProjectScopes { get; private set; }

        private void OnValidate()
        {
            ReflexLogger.UpdateLogLevel(LogLevel);
        }

        private void Reset()
        {
            LoadAllProjectScopes = true;
        }
    }
}
