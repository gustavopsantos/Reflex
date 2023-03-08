using Reflex.Scripts.Logging;
using UnityEngine;

namespace Assets.Reflex.Scripts.Configuration
{
    public class ReflexConfiguration : ScriptableObject
    {
        public static LogLevel LogLevel { get; private set; }

        [SerializeField] private LogLevel _logLevel;

        private void OnValidate()
        {
            LogLevel = _logLevel;
        }
    }
}
