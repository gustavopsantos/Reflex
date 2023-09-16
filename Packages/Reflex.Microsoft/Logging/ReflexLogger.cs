using System;
using Reflex.Microsoft.Configuration;
using Reflex.Microsoft.Utilities;
using UnityEngine;

namespace Reflex.Microsoft.Logging
{
	public static class ReflexMicrosoftLogger
	{
		private static readonly LogLevel _logLevel;

		static ReflexMicrosoftLogger()
		{
			_logLevel = ResourcesUtilities.TryLoad(nameof(ReflexMicrosoftSettings), out ReflexMicrosoftSettings reflexSettings)
				? reflexSettings.LogLevel
				: LogLevel.Info;

			Log($"Reflex LogLevel set to {_logLevel}", LogLevel.Info);
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
	}
}