using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Reflex.Editor.DebuggingWindow
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<MyTreeElement>
	{
		private const float kRowHeights = 20f;
		private const float kToggleWidth = 18f;

		// All columns
		private enum MyColumns
		{
			Name,
			Resolutions,
		}

		private enum SortOption
		{
			Name,
			Resolutions,
		}

		private readonly SortOption[] _sortOptions = 
		{
			SortOption.Name, 
			SortOption.Resolutions, 
		};
		
		public MultiColumnTreeView (TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<MyTreeElement> model) : base (state, multiColumnHeader, model)
		{
			Assert.AreEqual(_sortOptions.Length , Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 0;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
			multiColumnHeader.sortingChanged += (_) => SortIfNeeded (rootItem, GetRows());
			Reload();
		}

		private static void TreeToList (TreeViewItem root, IList<TreeViewItem> result)
		{
			if (root == null)
				throw new NullReferenceException("root");
			if (result == null)
				throw new NullReferenceException("result");

			result.Clear();
	
			if (root.children == null)
				return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--)
				stack.Push(root.children[i]);

			while (stack.Count > 0)
			{
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null)
				{
					for (int i = current.children.Count - 1; i >= 0; i--)
					{
						stack.Push(current.children[i]);
					}
				}
			}
		}


		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows (root);
			SortIfNeeded (root, rows);
			return rows;
		}

		private void SortIfNeeded (TreeViewItem root, IList<TreeViewItem> rows)
		{
			if (rows.Count <= 1)
				return;
			
			if (multiColumnHeader.sortedColumnIndex == -1)
			{
				return; // No column to sort for (just use the order the data are in)
			}
			
			// Sort the roots of the existing tree items
			SortByMultipleColumns ();
			TreeToList(root, rows);
			Repaint();
		}

		private void SortByMultipleColumns ()
		{
			var sortedColumns = multiColumnHeader.state.sortedColumns;

			if (sortedColumns.Length == 0)
				return;

			var myTypes = rootItem.children.Cast<TreeViewItem<MyTreeElement> >();
			var orderedQuery = InitialOrder (myTypes, sortedColumns);
			for (int i=1; i<sortedColumns.Length; i++)
			{
				SortOption sortOption = _sortOptions[sortedColumns[i]];
				bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

				switch (sortOption)
				{
					case SortOption.Name:
						orderedQuery = orderedQuery.ThenBy(l => l.data.Name, ascending);
						break;
					case SortOption.Resolutions:
						orderedQuery = orderedQuery.ThenBy(l => l.data.Resolutions, ascending);
						break;
				}
			}

			rootItem.children = orderedQuery.Cast<TreeViewItem> ().ToList ();
		}

		private IOrderedEnumerable<TreeViewItem<MyTreeElement>> InitialOrder(IEnumerable<TreeViewItem<MyTreeElement>> myTypes, int[] history)
		{
			SortOption sortOption = _sortOptions[history[0]];
			bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
			switch (sortOption)
			{
				case SortOption.Name:
					return myTypes.Order(l => l.data.Name, ascending);
				case SortOption.Resolutions:
					return myTypes.Order(l => l.data.Resolutions, ascending);
				default:
					Assert.IsTrue(false, "Unhandled enum");
					break;
			}

			// default
			return myTypes.Order(l => l.data.Name, ascending);
		}

		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<MyTreeElement>) args.item;

			for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		private void CellGUI(Rect cellRect, TreeViewItem<MyTreeElement> item, MyColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case MyColumns.Name:
					DrawItemNameColumn(cellRect, item, ref args);
					break;
				case MyColumns.Resolutions:
					GUI.Label(cellRect, item.data.Resolutions.Invoke());
					break;
			}
		}
		
		private void DrawItemIcon(Rect area, TreeViewItem<MyTreeElement> item)
		{
			area.x += GetContentIndent(item);
			area.width = 16;
			GUI.DrawTexture(area, item.data.Icon, ScaleMode.ScaleToFit);
		}

		private void DrawItemNameColumn(Rect area, TreeViewItem<MyTreeElement> item, ref RowGUIArgs args)
		{
			DrawItemIcon(area, item);
			area.x += 4;
			args.rowRect = area;
			base.RowGUI(args);
		}

		// Misc
		//--------

		protected override bool CanMultiSelect (TreeViewItem item)
		{
			return true;
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
		{
			var columns = new[] 
			{
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 800, 
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Resolutions", "In sed porta ante. Nunc et nulla mi."),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					minWidth = 74,
					maxWidth = 74,
					autoResize = true
				},
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

			var state =  new MultiColumnHeaderState(columns);
			return state;
		}
	}
}
