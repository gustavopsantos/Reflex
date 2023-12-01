using System;
using IL2CPPTest.Models;
using Reflex.Core;
using UnityEngine;

namespace Reflex.IL2CPP.Tests
{
    internal class PlatformRuntimeTest : MonoBehaviour
    {
        private Container _container;

        private void Start()
        {
            _container = new ContainerDescriptor("")
                .AddSingleton(42, typeof(int))
                .AddTransient(typeof(TestGenericStructure<int>), typeof(ITestGenericStructure<int>))
                .Build();
        }

        private void OnGUI()
        {
            GUILabel(RunTest());
        }

        private static void GUILabel(string content)
        {
            var area = new Rect(0, 0, Screen.width, Screen.height);
            var style = new GUIStyle("label") {fontSize = 48, alignment = TextAnchor.MiddleCenter};
            GUI.Label(area, content, style);
        }

        private string RunTest()
        {
            if (!Application.isEditor)
            {
                return IsPlatformSupported(out var error) ? "Supported Platform" : $"Unsupported Platform\n{error}";
            }

            return "This test is meant to be run on runtime platforms, not inside unity editor.";
        }

        private bool IsPlatformSupported(out string error)
        {
            try
            {
                error = null;
                return _container.Single<ITestGenericStructure<int>>().Value == 42;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }
}