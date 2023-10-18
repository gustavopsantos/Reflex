using System;
using Reflex.Configuration;
using Reflex.Utilities;
using UnityEngine;

namespace Reflex.Logging
{
    internal static class ReflexLogger
    {
        private static LogLevel _logLevel;

        static ReflexLogger()
        {
            _logLevel = ResourcesUtilities.TryLoad<ReflexSettings>(nameof(ReflexSettings), out var reflexSettings)
                ? reflexSettings.LogLevel
                : LogLevel.Info;
            
            ReportLogLevel();
        }

        public static void UpdateLogLevel(LogLevel logLevel)
        {
            if (logLevel != _logLevel)
            {
                _logLevel = logLevel;
                Log($"Reflex LogLevel set to {_logLevel}", LogLevel.Info);
            }
        }
        
        public static void Log(object message, LogLevel logLevel, UnityEngine.Object context = null)
        {
            if (logLevel < _logLevel)
            {
                return;
            }
            
            switch (logLevel)
            {
                case LogLevel.Development: Debug.Log(message, context); break;
                case LogLevel.Info: Debug.Log(message, context); break;
                case LogLevel.Warning: Debug.LogWarning(message, context); break;
                case LogLevel.Error: Debug.LogError(message, context); break;
                default: throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        private static void ReportLogLevel()
        {
            Log($"Reflex LogLevel set to {_logLevel}", LogLevel.Info);
        }
    }
}