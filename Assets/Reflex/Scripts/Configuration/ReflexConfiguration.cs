using Reflex.Scripts.Logging;
using UnityEngine;

namespace Assets.Reflex.Scripts.Configuration
{
    public class ReflexConfiguration : ScriptableObject
    {
        public static LoggingLevel LogginLevel { get; private set; }

        [SerializeField] private LoggingLevel _loggingLevel;

        private void OnValidate()
        {
            LogginLevel = _loggingLevel;
        }
    }
}
