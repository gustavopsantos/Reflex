using Reflex.Logging;
using UnityEngine;

namespace Reflex.Configuration
{
    internal class ReflexSettings : ScriptableObject
    {
        [field: SerializeField] public LogLevel LogLevel { get; private set; }
    }
}