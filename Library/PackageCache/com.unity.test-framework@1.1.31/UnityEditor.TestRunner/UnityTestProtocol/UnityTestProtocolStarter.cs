using System;
using System.Linq;
using UnityEditor.Compilation;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    [InitializeOnLoad]
    internal static class UnityTestProtocolStarter
    {
        static UnityTestProtocolStarter()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Contains("-automated") && commandLineArgs.Contains("-runTests")) // wanna have it only for utr run
            {
                var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                var listener = ScriptableObject.CreateInstance<UnityTestProtocolListener>();
                api.RegisterCallbacks(listener);
            }
        }
    }
}
