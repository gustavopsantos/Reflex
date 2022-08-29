using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.CommandLineTest
{
    internal class ExitCallbacksDataHolder : ScriptableSingleton<ExitCallbacksDataHolder>
    {
        [SerializeField] 
        public bool AnyTestsExecuted;
        [SerializeField]
        public bool RunFailed;
    }
}