using System;
using System.Collections.Generic;
using Reflex.Scripts.Core;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Container = Reflex.Container;

namespace UnityEditor.TreeViewExamples
{
    public class ReflexDebuggerWindow : EditorWindow
    {
        // private const string ContainerIcon = "d_PrefabModel On Icon";
        private const string ContainerIcon = "PreMatCube";
        private const string ObjectIcon = "curvekeyframeselected";
        private const string ContractIcon = "P4_Updating";
        
        [NonSerialized] private bool _isInitialized;
        [SerializeField] private TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] private MultiColumnHeaderState _multiColumnHeaderState;
        
        private SearchField _searchField;
        
        private MultiColumnTreeView TreeView { get; set; }
        private Rect SearchBarRect => new Rect(20f, 10f, position.width - 40f, 20f);
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

                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += TreeView.SetFocusAndEnsureSelectedItem;

                _isInitialized = true;
            }
        }

        private int _id = -1;

        private void BuildDataRecursively(MyTreeElement parent, Node<Container> node)
        {
            var child = new MyTreeElement("NoName", parent.Depth + 1, ++_id, ContainerIcon, "C");
            parent.Children.Add(child);
            child.Parent = parent;

            foreach (var c in node.Children)
            {
                BuildDataRecursively(child, c);
            }
        }

        private IList<MyTreeElement> GetData()
        {
            var root = new MyTreeElement("Root", -1, ++_id, ContainerIcon, "-");
            BuildDataRecursively(root, ContainerTree.Root);

            var list = new List<MyTreeElement>();
            TreeElementUtility.TreeToList(root, list);
            return list;
        }
        
        private static IList<MyTreeElement> GetMockedData()
        {
            var root = new MyTreeElement("Root", -1, 0, ContainerIcon, "-");

            var projectContainer = new MyTreeElement("ProjectContainer", 0, 1, ContainerIcon, "C");
            projectContainer.Parent = root;
            root.Children.Add(projectContainer);

            var projectContainer2 = new MyTreeElement("ProjectContainer2", 0, 5, ContainerIcon, "C");
            projectContainer2.Parent = root;
            root.Children.Add(projectContainer2);

            var sceneContainer = new MyTreeElement("SceneContainer", 1, 2, ContainerIcon, "C");
            sceneContainer.Parent = projectContainer;
            projectContainer.Children.Add(sceneContainer);

            var scopedContainer = new MyTreeElement("ScopedContainer", 2, 6, ContainerIcon, "C");
            scopedContainer.Parent = sceneContainer;
            sceneContainer.Children.Add(scopedContainer);

            var projectContainerResolver = new MyTreeElement("SingletonResolver<ITimeSource>", 1, 3, ContractIcon, "R");
            projectContainerResolver.Parent = projectContainer;
            projectContainer.Children.Add(projectContainerResolver);

            var timeSourceInstance = new MyTreeElement("LocalTimeSource", 2, 4, ObjectIcon, "O");
            timeSourceInstance.Parent = projectContainerResolver;
            projectContainerResolver.Children.Add(timeSourceInstance);

            var list = new List<MyTreeElement>()
            {
                root,
                projectContainer,
                sceneContainer,
                scopedContainer,
                projectContainerResolver,
                timeSourceInstance,
                projectContainer2,
            };

            return list;
        }

        private void OnGUI()
        {
            InitIfNeeded();
            SearchBar(SearchBarRect);
            DoTreeView(MultiColumnTreeViewRect);
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