using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using Reflex.Resolvers;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Editor.DebuggingWindow
{
    public class ReflexDebuggerWindow : EditorWindow
    {
        private const string ContainerIcon = "PreMatCylinder"; // d_PrefabModel On Icon, "PreMatCylinder"
        private const string ResolverIcon = "d_NetworkAnimator Icon"; // "d_eyeDropper.Large", "AnimatorStateTransition Icon", "RelativeJoint2D Icon"
        private const string InstanceIcon = "d_Prefab Icon"; // "d_Prefab Icon", "d_Prefab On Icon"

        [NonSerialized] private bool _isInitialized;
        [SerializeField] private TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] private MultiColumnHeaderState _multiColumnHeaderState;

        private int _id = -1;
        private SearchField _searchField;
        private Vector2 _bindingStackTraceScrollPosition;

        private MultiColumnTreeView TreeView { get; set; }
        private Rect SearchBarRect => new Rect(20f, 10f, position.width - 40f, 20f);
        private Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 200);

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            await Task.Yield();
            Refresh();
        }
        
        private async void OnSceneUnloaded(Scene scene)
        {
            await Task.Yield();
            Refresh();
        }
        
        private async void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            await Task.Yield();
            Refresh();
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

        private static List<(IResolver, Type[])> BuildMatrix(Container container)
        {
            var resolvers = container.ResolversByContract.Values
                .SelectMany(r => r)
                .ToHashSet();

            if (ReflexEditorSettings.ShowInternalBindings == false)
            {
                var containerResolver = container.ResolversByContract[typeof(Container)].Single();
                resolvers.Remove(containerResolver);
            }

            if (ReflexEditorSettings.ShowInheritedBindings == false && container.Parent != null)
            {
                var parentResolvers = container.Parent.ResolversByContract.Values
                    .SelectMany(r => r)
                    .Distinct();

                foreach (var parentResolver in parentResolvers)
                {
                    resolvers.Remove(parentResolver);
                }
            }
            
            return resolvers.Select(resolver => (resolver, GetContracts(resolver, container))).ToList();
        }
        
        private static Type[] GetContracts(IResolver resolver, Container container)
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

            var containerTreeElement = new MyTreeElement(container.Name, parent.Depth + 1, ++_id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty, container.GetDebugProperties().BuildCallsite, kind: string.Empty);
            parent.Children.Add(containerTreeElement);
            containerTreeElement.Parent = parent;
            
            foreach (var pair in BuildMatrix(container))
            {
                var resolverTreeElement = new MyTreeElement(
                    string.Join(", ", pair.Item2.Select(x => x.GetName())), // In this case Name is not used for rendering, but for searching
                    containerTreeElement.Depth + 1,
                    ++_id,
                    ResolverIcon,
                    () => pair.Item1.GetDebugProperties().Resolutions.ToString(),
                    pair.Item2.Select(x => x.GetName()).OrderBy(x => x).ToArray(),
                    pair.Item1.Lifetime.ToString(),
                    pair.Item1.GetDebugProperties().BindingCallsite,
                    kind: pair.Item1.GetType().Name.Replace("Singleton", string.Empty).Replace("Transient", string.Empty).Replace("Scoped", string.Empty).Replace("Resolver", string.Empty)
                );

                foreach (var (instance, callsite) in pair.Item1.GetDebugProperties().Instances.Where(tuple => tuple.Item1.IsAlive).Select(tuple => (tuple.Item1.Target, tuple.Item2)))
                {
                    var instanceTreeElement = new MyTreeElement(
                        $"{instance.GetType().GetName()} <color=#3D99ED>({SHA1.ShortHash(instance.GetHashCode())})</color>",
                        resolverTreeElement.Depth + 1,
                        ++_id,
                        InstanceIcon,
                        () => string.Empty,
                        Array.Empty<string>(),
                        string.Empty,
                        callsite,
                        string.Empty
                    );
                    
                    instanceTreeElement.SetParent(resolverTreeElement);
                }
                
                resolverTreeElement.SetParent(containerTreeElement);
            }

            foreach (var scopedContainer in container.Children)
            {
                BuildDataRecursively(containerTreeElement, scopedContainer);
            }
        }
        
        private IList<MyTreeElement> GetData()
        {
            var root = new MyTreeElement("Root", -1, ++_id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty, null, string.Empty);
            BuildDataRecursively(root, UnityInjector.ProjectContainer);

            var list = new List<MyTreeElement>();
            TreeElementUtility.TreeToList(root, list);
            return list;
        }

        private void OnGUI()
        {
            Repaint();
            InitIfNeeded();
            
            if (UnityScriptingDefineSymbols.IsDefined("REFLEX_DEBUG"))
            {
                PresentDebuggerEnabled();
            }
            else
            {
                PresentDebuggerDisabled();
            }
            
            GUILayout.FlexibleSpace();
            PresentStatusBar();
        }

        private void Refresh(PlayModeStateChange _ = default)
        {
            _isInitialized = false;
            InitIfNeeded();
        }

        private void SearchBar(Rect rect)
        {
            TreeView.searchString = _searchField.OnGUI(rect, TreeView.searchString);
            GUILayoutUtility.GetRect(rect.width, rect.height);
        }

        private void DoTreeView(Rect rect)
        {
            TreeView.OnGUI(rect);
            GUILayoutUtility.GetRect(rect.width, rect.height);
        }
        
        private static void PresentDebuggerDisabled()
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("To start debugging, enable the Reflex Debug Mode in the status bar.", Styles.LabelHorizontallyCentered);
            GUILayout.Label("Keep in mind that enabling Reflex Debug Mode will impact performance.", Styles.LabelHorizontallyCentered);
        }

        private void PresentDebuggerEnabled()
        {
            SearchBar(SearchBarRect);
            DoTreeView(MultiColumnTreeViewRect);

            GUILayout.Space(16);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16);

                using (new GUILayout.VerticalScope())
                {
                    _bindingStackTraceScrollPosition = GUILayout.BeginScrollView(_bindingStackTraceScrollPosition);

                    PresentCallSite();

                    GUILayout.EndScrollView();
                    GUILayout.Space(16);
                }

                GUILayout.Space(16);
            }
        }

        private void PresentHierarchyFilters()
        {
            EditorGUI.BeginChangeCheck();
            ReflexEditorSettings.ShowInternalBindings = GUILayout.Toggle(ReflexEditorSettings.ShowInternalBindings, "Show Internal Bindings ");
            ReflexEditorSettings.ShowInheritedBindings = GUILayout.Toggle(ReflexEditorSettings.ShowInheritedBindings, "Show Inherited Bindings ");
            if (EditorGUI.EndChangeCheck())
            {
                Refresh();
            }
        }
        
        private void PresentStatusBar()
        {
            using (new EditorGUILayout.HorizontalScope(Styles.AppToolbar))
            {
                GUILayout.FlexibleSpace();
                PresentHierarchyFilters();
                
                var refreshIcon = EditorGUIUtility.IconContent("d_TreeEditor.Refresh");
                refreshIcon.tooltip = "Forces Tree View to Refresh";
                
                if (GUILayout.Button(refreshIcon, Styles.StatusBarIcon, GUILayout.Width(25)))
                {
                    Refresh();
                }
                
                var debuggerIcon = UnityScriptingDefineSymbols.IsDefined("REFLEX_DEBUG")
                    ? EditorGUIUtility.IconContent("d_DebuggerEnabled")
                    : EditorGUIUtility.IconContent("d_DebuggerDisabled");

                debuggerIcon.tooltip = UnityScriptingDefineSymbols.IsDefined("REFLEX_DEBUG")
                    ? "Reflex Debugger Enabled"
                    : "Reflex Debugger Disabled";
                
                if (GUILayout.Button(debuggerIcon, Styles.StatusBarIcon, GUILayout.Width(25)))
                {
                    UnityScriptingDefineSymbols.Toggle("REFLEX_DEBUG", EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
        }

        private void PresentCallSite()
        {
            var selection = TreeView.GetSelection();
            
            if (selection == null || selection.Count == 0)
            {
                return;
            }
            
            var item = TreeView.Find(selection.Single());

            if (item == null || item.Callsite == null)
            {
                return;
            }

            foreach (var callSite in item.Callsite)
            {
                PresentStackFrame(callSite.ClassName, callSite.FunctionName, callSite.Path, callSite.Line);
            }
        }
        
        private static void PresentStackFrame(string className, string functionName, string path, int line)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"{className}:{functionName}()  →", Styles.StackTrace);

                if (PresentLinkButton($"{path}:{line}"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, line);
                }
            }
        }

        private static bool PresentLinkButton(string label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(new GUIContent(label), Styles.Hyperlink, options);
            position.y -= 3;
            Handles.color = Styles.Hyperlink.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin + (float)EditorStyles.linkLabel.padding.left, position.yMax), new Vector3(position.xMax - (float)EditorStyles.linkLabel.padding.right, position.yMax));
            Handles.color = Color.white;
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, Styles.Hyperlink);
        }
    }
}