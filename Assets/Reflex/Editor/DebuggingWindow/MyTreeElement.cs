using System;
using UnityEditor;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
    [Serializable]
    internal class MyTreeElement : TreeElement
    {
        public readonly Func<string> Resolutions;
        public Texture Icon { get; }

        public MyTreeElement(string name, int depth, int id, string icon, Func<string> resolutions) : base(name, depth, id)
        {
            Resolutions = resolutions;
            Icon = EditorGUIUtility.IconContent(icon).image;
        }
    }
}