using System;
using System.Collections.Generic;
using Reflex.Scripts.Core;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
    public class ReflexDebuggerWindow : EditorWindow
    {
        // private const string ContainerIcon = "d_PrefabModel On Icon";
        private const string ContainerIcon = "PreMatCube";
        private const string ObjectIcon = "curvekeyframeselected";
        private const string ResolverIcon = "P4_Updating";

        [NonSerialized] private bool _isInitialized;

        [SerializeField]
        private TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading

        [SerializeField] private MultiColumnHeaderState _multiColumnHeaderState;

        private SearchField _searchField;

        private MultiColumnTreeView TreeView { get; set; }
        private Rect SearchBarRect => new Rect(20f + 32f, 10f, position.width - 40f, 20f);
        private Rect RefreshButtonRect => new Rect(20f, 10f, 32, 20f);
        private Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 50);

        [MenuItem("Reflex/Debugger %e")]
        public static void GetWindow()
        {
            var window = GetWindow<ReflexDebuggerWindow>();
            window.titleContent = new GUIContent("Reflex Debugger");
            window.Focus();
            window.Repaint();
        }

        private void InitIfNeeded()
        {
            if (!_isInitialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (_treeViewState == null)
                    _treeViewState = new TreeViewState();

                bool firstInit = _multiColumnHeaderState == null;
                var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(_multiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(_multiColumnHeaderState, headerState);
                _multiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState)
                {
                    canSort = false,
                    height = MultiColumnHeader.DefaultGUI.minimumHeight
                };

                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<MyTreeElement>(GetData());

                TreeView = new MultiColumnTreeView(_treeViewState, multiColumnHeader, treeModel);
                TreeView.ExpandAll();

                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += TreeView.SetFocusAndEnsureSelectedItem;

                _isInitialized = true;
            }
        }

        private int _id = -1;

        private void BuildDataRecursively(MyTreeElement parent, Container container)
        {
            if (container == null)
            {
                return;
            }

            var child = new MyTreeElement(container.Name, parent.Depth + 1, ++_id, ContainerIcon, () => string.Empty);
            parent.Children.Add(child);
            child.Parent = parent;

            foreach (var pair in container._resolvers)
            {
                var t = pair.Value.Concrete != null ? pair.Value.Concrete.Name : "-";
                var r = new MyTreeElement($"{pair.Value.GetType().Name}<{pair.Key}> → {t}", child.Depth + 1, ++_id,
                    ResolverIcon, () => pair.Value.Resolutions.ToString());
                child.Children.Add(r);
                r.Parent = child;
            }

            foreach (var c in container.Children)
            {
                BuildDataRecursively(child, c);
            }
        }

        private IList<MyTreeElement> GetData()
        {
            var root = new MyTreeElement("Root", -1, ++_id, ContainerIcon, () => string.Empty);
            BuildDataRecursively(root, ContainerTree.Root);

            var list = new List<MyTreeElement>();
            TreeElementUtility.TreeToList(root, list);
            return list;
        }

        private void OnGUI()
        {
            Repaint();
            InitIfNeeded();
            SearchBar(SearchBarRect);
            DoTreeView(MultiColumnTreeViewRect);

            if (GUI.Button(RefreshButtonRect, EditorGUIUtility.IconContent("Refresh")))
            {
                _isInitialized = false;
                InitIfNeeded();
            }
        }

        private void SearchBar(Rect rect)
        {
            TreeView.searchString = _searchField.OnGUI(rect, TreeView.searchString);
        }

        private void DoTreeView(Rect rect)
        {
            TreeView.OnGUI(rect);
        }
    }
}