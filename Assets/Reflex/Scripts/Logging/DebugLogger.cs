using Assets.Reflex.Scripts.Configuration;
using UnityEngine;

namespace Reflex.Scripts.Logging
{
    internal static class DebugLogger
    {
        public static LogLevel LogLevel { get; set; } = LogLevel.Default;

        public static void Log(object message)
        {
            if (ShouldLog(LogLevel.Default))
                Debug.Log(message);
        }

        public static void LogWarning(object message)
        {
            if (ShouldLog(LogLevel.Warning))
                Debug.LogWarning(message);
        }

        public static void LogError(object message)
        {
            if (ShouldLog(LogLevel.Error))
                Debug.LogError(message);
        }

        public static void Apply(ReflexConfiguration configuration)
        {
            LogLevel = configuration.LogLevel;
        }

        private static bool ShouldLog(LogLevel messageLogLevel)
        {
            return messageLogLevel >= LogLevel;
        }
    }
}
