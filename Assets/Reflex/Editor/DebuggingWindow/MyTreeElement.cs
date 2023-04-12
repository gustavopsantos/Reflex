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
        public string[] Contracts { get; }
        public string ResolutionType { get; }

        public MyTreeElement(string name, int depth, int id, string icon, Func<string> resolutions, string[] contracts, string resolutionType) : base(name, depth, id)
        {
            Resolutions = resolutions;
            ResolutionType = resolutionType;
            Contracts = contracts;
            Icon = EditorGUIUtility.IconContent(icon).image;
        }
    }
}