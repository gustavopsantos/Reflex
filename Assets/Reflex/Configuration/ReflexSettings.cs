using Reflex.Logging;
using UnityEngine;
using UnityEngine.Assertions;

namespace Reflex.Configuration
{
    internal sealed class ReflexSettings : ScriptableObject
    {
        private static ReflexSettings _instance;
        
        public static ReflexSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ReflexSettings>("ReflexSettings");
                }
                
                Assert.IsNotNull(_instance, "ReflexSettings not found in Resources folder. Please create one.");
                return _instance;
            }
        }
        
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
