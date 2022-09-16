using System;
using Reflex;
using UnityEngine;
using IL2CPPTest.Models;

namespace IL2CPPTest
{
    public class PlatformRuntimeTest : MonoBehaviour
    {
        private readonly Container _container = new Container(string.Empty);

        private void Start()
        {
            _container.BindInstance(42);
            _container.BindTransient<ITestGenericStructure<int>, TestGenericStructure<int>>();
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
                return _container.Resolve<ITestGenericStructure<int>>().Value == 42;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }
}