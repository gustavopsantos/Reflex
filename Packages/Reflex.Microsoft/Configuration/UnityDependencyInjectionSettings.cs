using Reflex.Microsoft.Logging;
using UnityEngine;

namespace Reflex.Microsoft.Configuration
{
	public class ReflexMicrosoftSettings : ScriptableObject
	{
		[field: SerializeField] public LogLevel LogLevel { get; private set; }
	}
}