using System;
using System.Collections.Generic;
using Reflex.Extensions;
using UnityEngine;

namespace Reflex.IL2CPP.Tests
{
    public class WeavingTest : MonoBehaviour
    {
        private class NumberManager
        {
            public IEnumerable<int> Numbers { get; }

            public NumberManager(IEnumerable<int> numbers)
            {
                Numbers = numbers;
            }
        }
        
        private void OnGUI()
        {
			string content = GetContent(); 
            GUILabel(content);
        }

        private static string GetContent()  
        {
            try
            {
                return string.Join(", ", (IEnumerable<int>) new object[] {1, 2, 3, 42}.CastDynamic(typeof(int)));
            }
            catch (Exception e)
            {
                return e.ToString(); 
            }
        }

        private static void GUILabel(string content)
        {
			Rect area = new Rect(0, 0, Screen.width, Screen.height);
			GUIStyle style = new GUIStyle("label") {fontSize = 16, alignment = TextAnchor.MiddleCenter};
            GUI.Label(area, content, style);
        }
    }
}