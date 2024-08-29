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
            Hierarchy,
            Kind,
            Lifetime,
            Calls,
        }

        public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<MyTreeElement> model) : base(state, multiColumnHeader, model)
        {
            rowHeight = RowHeight;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (RowHeight - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = ToggleWidth;
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<MyTreeElement>) args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (Column) args.GetColumn(i), ref args);
            }
        }

        private Texture2D _texture;

        private void TryInitTexture()
        {
            if (_texture == null)
            {
                _texture = new Texture2D(1, 1);
                _texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
                _texture.Apply();
            }
        }

        private void CellGUI(Rect cellRect, TreeViewItem<MyTreeElement> item, Column column, ref RowGUIArgs args)
        {
            TryInitTexture();
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case Column.Hierarchy:
                    DrawItemIcon(cellRect, item);
                    cellRect.xMin += 20; // Icon size (16) + 4 of padding

                    if (item.Data.Contracts != null && item.Data.Contracts.Length > 0)
                    {
                        DrawContracts(item, cellRect, item.Data.Contracts);
                    }
                    else
                    {
                        DrawName(item, cellRect, item.Data.Name);
                    }

                    break;
                case Column.Calls:
                    GUI.Label(cellRect, item.Data.Resolutions.Invoke());
                    break;
                case Column.Kind:
                    GUI.Label(cellRect, item.Data.Kind);
                    break;
                case Column.Lifetime:
                    GUI.Label(cellRect, item.Data.ResolutionType);
                    break;
            }
        }

        private void DrawName(TreeViewItem item, Rect area, string name)
        {
            area.xMin += GetContentIndent(item);
            GUI.Label(area, name, Styles.RichTextLabel);
        }

        private void DrawContracts(TreeViewItem<MyTreeElement> item, Rect rect, string[] contracts)
        {
            if (contracts == null || contracts.Length == 0 || string.IsNullOrEmpty(contracts[0]))
            {
                return;
            }

            rect.y += 1;

            var style = new GUIStyle("CN CountBadge") // CN CountBadge, AssetLabel, AssetLabel Partial
            {
                wordWrap = false,
                stretchWidth = false,
                stretchHeight = false,
                fontStyle = FontStyle.Bold,
                fontSize = 11
            };

            rect.xMin += GetContentIndent(item);

            // Clipping group
            GUI.BeginGroup(rect);
            {
                foreach (var contract in contracts)
                {
                    var content = new GUIContent($"{contract}");
                    var labelWidth = style.CalcSize(content).x;

                    // Draw the label within the bounds of the rect
                    Rect labelRect = new Rect(0, 0, labelWidth, rect.height);
                    GUI.Label(labelRect, content, style);

                    // Move the rect for the next contract
                    rect.xMin += labelWidth + 4;

                    // Stop drawing if the labels go beyond the column's width
                    if (rect.xMin > rect.width)
                        break;
                }
            }
            GUI.EndGroup();
        }

        private void DrawItemIcon(Rect area, TreeViewItem<MyTreeElement> item)
        {
            area.xMin += GetContentIndent(item);

            // Clipping group
            GUI.BeginGroup(area);
            {
                // Draw the icon within the bounds of the area
                Rect iconRect = new Rect(0, 0, 16, area.height);
                GUI.DrawTexture(iconRect, item.Data.Icon, ScaleMode.ScaleToFit);
            }
            GUI.EndGroup();
        }

        private void DrawItemNameColumn(Rect area, TreeViewItem<MyTreeElement> item, ref RowGUIArgs args)
        {
            DrawItemIcon(area, item);
            area.x += 4;
            args.rowRect = area;
            base.RowGUI(args);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
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
                    headerContent = new GUIContent(Column.Hierarchy.ToString()),
                    headerTextAlignment = TextAlignment.Left,
                    width = 280,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(Column.Kind.ToString()),
                    headerTextAlignment = TextAlignment.Left,
                    width = 64,
                    minWidth = 64,
                    maxWidth = 64,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(Column.Lifetime.ToString()),
                    headerTextAlignment = TextAlignment.Left,
                    width = 64,
                    minWidth = 64,
                    maxWidth = 64,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(Column.Calls.ToString()),
                    headerTextAlignment = TextAlignment.Left,
                    width = 38,
                    minWidth = 38,
                    autoResize = false
                },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Column)).Length, "Number of columns should match number of enum values");
            return new MultiColumnHeaderState(columns);
        }
    }
}