using System.Collections.Generic;
using Reflex.Core;
using Reflex.Logging;
using UnityEngine;
using UnityEngine.Assertions;

namespace Reflex.Configuration
{
    internal sealed class ReflexSettings : ScriptableObject
    {
        private static ReflexSettings _instance;
        private static ResourceRequest _settingsRequest;

        public static ReflexSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _settingsRequest ??= Resources.LoadAsync<ReflexSettings>("ReflexSettings");

                    // This stalls execution until the request fully resolves.
                    // This *should* be faster than non-async loading when project installers have a lot of references.
                    _instance = (ReflexSettings)_settingsRequest.asset;
                }
                
                Assert.IsNotNull(_instance, "ReflexSettings not found in Resources folder.\n" +
                                            "Please create ReflexSettings using right mouse button over Resources folder, Create > Reflex > Settings.");
                return _instance;
            }
        }
        
        [field: SerializeField] public LogLevel LogLevel { get; private set; }
        [field: SerializeField] public List<ProjectScope> ProjectScopes { get; private set; }

        private void OnValidate()
        {
            _instance = this;
            ReflexLogger.UpdateLogLevel(LogLevel);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeReflex()
        {
            _settingsRequest = Resources.LoadAsync<ReflexSettings>("ReflexSettings");
        }
    }
}
