using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Resolvers;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
    public class ReflexDebuggerWindow : EditorWindow
    {
        private const string ContainerIcon = "winbtn_win_max_h"; // d_PrefabModel On Icon, "PreMatCylinder"
        private const string ResolverIcon = "d_NetworkAnimator Icon"; // "d_eyeDropper.Large", "AnimatorStateTransition Icon", "RelativeJoint2D Icon"

        [NonSerialized] private bool _isInitialized;
        [SerializeField] private TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] private MultiColumnHeaderState _multiColumnHeaderState;

        private int _id = -1;
        private SearchField _searchField;

        private MultiColumnTreeView TreeView { get; set; }
        private Rect SearchBarRect => new Rect(20f + 32f + 2f, 10f, position.width - 40f - 32f - 2f, 20f);
        private Rect RefreshButtonRect => new Rect(20f, 8f, 32, 20f);
        private Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 50);

        private void OnFocus()
        {
            Refresh();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += Refresh;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= Refresh;
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

        private static List<(Resolver, Type[])> BuildMatrix(Container container)
        {
            var resolvers = container.ResolversByContract.Values.SelectMany(r => r).Distinct();
            return resolvers.Select(resolver => (resolver, GetContracts(resolver, container))).ToList();
        }
        
        private static Type[] GetContracts(Resolver resolver, Container container)
        {
            var result = new List<Type>();

            foreach (var pair in container.ResolversByContract)
            {
                if (pair.Value.Contains(resolver))
                {
                    result.Add(pair.Key);
                }
            }

            return result.ToArray();
        }

        private void BuildDataRecursively(MyTreeElement parent, Container container)
        {
            if (container == null)
            {
                return;
            }

            var child = new MyTreeElement(container.Name, parent.Depth + 1, ++_id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty);
            parent.Children.Add(child);
            child.Parent = parent;
            
            foreach (var pair in BuildMatrix(container))
            {
                var r = new MyTreeElement(
                    pair.Item1.Concrete != null ? pair.Item1.Concrete.Name : "-",
                    child.Depth + 1,
                    ++_id,
                    ResolverIcon,
                    () => pair.Item1.Resolutions.ToString(),
                    pair.Item2.Select(x => x.GetFullName()).OrderBy(x => x).ToArray(),
                    pair.Item1.GetType().GetName());
                
                child.Children.Add(r);
                r.Parent = child;
            }

            foreach (var scopedContainer in container.Children)
            {
                BuildDataRecursively(child, scopedContainer);
            }
        }
        
        private IList<MyTreeElement> GetData()
        {
            var root = new MyTreeElement("Root", -1, ++_id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty);
            BuildDataRecursively(root, Tree<Container>.Root);

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
                Refresh();
            }
        }

        private void Refresh(PlayModeStateChange _ = default)
        {
            _isInitialized = false;
            InitIfNeeded();
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