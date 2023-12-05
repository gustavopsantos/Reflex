using System;
using System.Collections.Generic;
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
        public List<CallSite> Callsite { get; }
        public string Kind { get; }

        public MyTreeElement(
            string name,
            int depth,
            int id,
            string icon,
            Func<string> resolutions,
            string[] contracts,
            string resolutionType,
            List<CallSite> callsite,
            string kind = ""
            ) : base(name, depth, id)
        {
            Resolutions = resolutions;
            ResolutionType = resolutionType;
            Contracts = contracts;
            Icon = EditorGUIUtility.IconContent(icon).image;
            Callsite = callsite;
            Kind = kind;
        }
    }
}