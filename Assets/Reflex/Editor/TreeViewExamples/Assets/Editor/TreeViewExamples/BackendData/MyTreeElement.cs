using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityEditor.TreeViewExamples
{
    [Serializable]
    internal class MyTreeElement : TreeElement
    {
        public Texture Icon { get; }
        public string Kind { get; }
        public int Resolutions { get; }

        public MyTreeElement(string name, int depth, int id, string icon, string kind) : base(name, depth, id)
        {
            Resolutions = (int) (Random.value * 100);
            Icon = EditorGUIUtility.IconContent(icon).image;
            Kind = kind;
        }
    }
}