using UnityEngine;

namespace Reflex.Scripts.Logging
{
    internal static class Logger
    {
        public static LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Default;

        public static void Log(object message)
        {
            if (ShouldLog(LoggingLevel.Default))
                Debug.Log(message);
        }

        public static void LogWarning(object message)
        {
            if (ShouldLog(LoggingLevel.Warning))
                Debug.LogWarning(message);
        }

        public static void LogError(object message)
        {
            if (ShouldLog(LoggingLevel.Error))
                Debug.LogError(message);
        }

        private static bool ShouldLog(LoggingLevel messageLoggingLevel)
        {
            return messageLoggingLevel >= LoggingLevel;
        }
    }
}
