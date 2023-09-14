using System;
using IL2CPPTest.Models;
using Microsoft.Extensions.DependencyInjection;
using Reflex.Core;
using UnityEngine;

namespace Reflex.IL2CPP.Tests
{
    internal class PlatformRuntimeTest : MonoBehaviour
    {
        private IServiceProvider _serviceProvider;

        private void Start()
        {
            _serviceProvider = new ServiceCollection()
                .AddTransient<ITestGenericStructure<int>>(serviceCollection => new TestGenericStructure<int>() { Value = 42 })
                .BuildServiceProvider();
        }

        private void OnGUI()
        {
            GUILabel(RunTest());
        }

        private static void GUILabel(string content)
        {
			Rect area = new Rect(0, 0, Screen.width, Screen.height);
			GUIStyle style = new GUIStyle("label") {fontSize = 48, alignment = TextAnchor.MiddleCenter};
            GUI.Label(area, content, style);
        }

        private string RunTest()
        {
            if (!Application.isEditor)
            {
                return IsPlatformSupported(out string error) ? "Supported Platform" : $"Unsupported Platform\n{error}";
            }

            return "This test is meant to be run on runtime platforms, not inside unity editor.";
        }

        private bool IsPlatformSupported(out string error)
        {
            try
            {
                error = null;
                return _serviceProvider.GetService<ITestGenericStructure<int>>().Value == 42;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }
}