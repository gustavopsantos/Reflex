using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Reflex.Editor.DebuggingWindow
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<MyTreeElement>
	{
		private const float RowHeight = 20f;
		private const float ToggleWidth = 18f;

		private enum Column
		{
			Implementation,
			Resolutions,
			ResolutionType,
			Contracts,
		}

		public MultiColumnTreeView (TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<MyTreeElement> model) : base (state, multiColumnHeader, model)
		{
			rowHeight = RowHeight;
			columnIndexForTreeFoldouts = 0;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (RowHeight - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = ToggleWidth;
			Reload();
		}

		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<MyTreeElement>) args.item;

			for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (Column)args.GetColumn(i), ref args);
			}
		}

		private void CellGUI(Rect cellRect, TreeViewItem<MyTreeElement> item, Column column, ref RowGUIArgs args)
		{
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case Column.Implementation:
					DrawItemNameColumn(cellRect, item, ref args);
					break;
				case Column.Resolutions:
					GUI.Label(cellRect, item.Data.Resolutions.Invoke());
					break;
				case Column.ResolutionType:
					GUI.Label(cellRect, item.Data.ResolutionType);
					break;
				case Column.Contracts:
					DrawContracts(cellRect, item.Data.Contracts);
					break;
			}
		}

		private static void DrawContracts(Rect rect, string[] contracts)
		{
			if (contracts == null || contracts.Length == 0 || string.IsNullOrEmpty(contracts[0]))
			{
				return;
			}
			
			rect.y += 1;

			var style = new GUIStyle("CN CountBadge") // CN CountBadge, AssetLabel, AssetLabel Partial
			{
				wordWrap = true,
				stretchWidth = false,
				stretchHeight = false,
				fontStyle = FontStyle.Bold,
				fontSize = 11
			};

			foreach (var contract in contracts)
			{
				var content = new GUIContent($"{contract}");
				rect.width = style.CalcSize(content).x;
				GUI.Label(rect, content, style);
				rect.x += rect.width + 4;
			}
		}

		private void DrawItemIcon(Rect area, TreeViewItem<MyTreeElement> item)
		{
			area.x += GetContentIndent(item);
			area.width = 16;
			GUI.DrawTexture(area, item.Data.Icon, ScaleMode.ScaleToFit);
		}

		private void DrawItemNameColumn(Rect area, TreeViewItem<MyTreeElement> item, ref RowGUIArgs args)
		{
			DrawItemIcon(area, item);
			area.x += 4;
			args.rowRect = area;
			base.RowGUI(args);
		}

		protected override bool CanMultiSelect (TreeViewItem item)
		{
			return false;
		}
		
		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
		{
			var columns = new[] 
			{
				new MultiColumnHeaderState.Column 
				{
					canSort = false,
					headerContent = new GUIContent("Implementation"),
					headerTextAlignment = TextAlignment.Left,
					width = 280, 
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column 
				{
					canSort = false,
					headerContent = new GUIContent("Resolutions", "In sed porta ante. Nunc et nulla mi."),
					headerTextAlignment = TextAlignment.Left,
					minWidth = 74,
					maxWidth = 74,
					autoResize = true
				},
				new MultiColumnHeaderState.Column 
				{
					canSort = false,
					headerContent = new GUIContent("Type"),
					headerTextAlignment = TextAlignment.Left,
					width = 140, 
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column 
				{
					canSort = false,
					headerContent = new GUIContent("Contracts"),
					headerTextAlignment = TextAlignment.Left,
					width = 280, 
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false
				},
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Column)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");
			return new MultiColumnHeaderState(columns);;
		}
	}
}
