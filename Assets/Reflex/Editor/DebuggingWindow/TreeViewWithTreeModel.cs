using System;
using System.Collections.Generic;
using Reflex.Logging;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
    internal class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
    {
        private TreeModel<T> _treeModel;
        private readonly List<TreeViewItem> _rows = new List<TreeViewItem>(100);

        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) : base(state, multiColumnHeader)
        {
            Init(model);
        }

        private void Init(TreeModel<T> model)
        {
            _treeModel = model;
        }

        protected override TreeViewItem BuildRoot()
        {
            int depthForHiddenRoot = -1;
            return new TreeViewItem<T>(_treeModel.Root.Id, depthForHiddenRoot, _treeModel.Root.Name, _treeModel.Root);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (_treeModel.Root == null)
            {
                ReflexLogger.Log("tree model root is null. did you call SetData()?", LogLevel.Error);
            }

            _rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(_treeModel.Root, searchString, _rows);
            }
            else
            {
                if (_treeModel.Root.HasChildren)
                {
                    AddChildrenRecursive(_treeModel.Root, 0, _rows);
                }
            }

            // We still need to setup the child parent information for the rows since this 
            // information is used by the TreeView internal logic (navigation, dragging etc)
            SetupParentsAndChildrenFromDepths(root, _rows);

            return _rows;
        }

        private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.Children)
            {
                var item = new TreeViewItem<T>(child.Id, depth, child.Name, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.Id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        private static void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));
            }

            if (searchFromThis == null || searchFromThis.Children == null)
            {
                return;
            }
            
            const int itemDepth = 0; // tree is flattened when searching

            Stack<T> stack = new Stack<T>();
            foreach (var element in searchFromThis.Children)
            {
                stack.Push((T) element);
            }

            while (stack.Count > 0)
            {
                T current = stack.Pop();
                // Matches search?
                if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(new TreeViewItem<T>(current.Id, itemDepth, current.Name, current));
                }

                if (current.Children != null && current.Children.Count > 0)
                {
                    foreach (var element in current.Children)
                    {
                        stack.Push((T) element);
                    }
                }
            }

            SortSearchResult(result);
        }

        private static void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }

        protected override IList<int> GetAncestors(int id)
        {
            return _treeModel.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return _treeModel.GetDescendantsThatHaveChildren(id);
        }
    }
}