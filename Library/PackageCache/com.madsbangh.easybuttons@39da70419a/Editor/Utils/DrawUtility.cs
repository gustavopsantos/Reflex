namespace EasyButtons.Editor.Utils
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A set of methods that simplify drawing of button controls.
    /// </summary>
    internal static class DrawUtility
    {
        private static readonly GUIContent _tempContent = new GUIContent();

        public readonly struct VerticalIndent : IDisposable
        {
            private const float SpacingHeight = 10f;
            private readonly bool _bottom;

            public VerticalIndent(bool top, bool bottom)
            {
                if (top)
                    GUILayout.Space(SpacingHeight);

                _bottom = bottom;
            }

            public void Dispose()
            {
                if (_bottom)
                    GUILayout.Space(SpacingHeight);
            }
        }

        public static bool DrawInFoldout(Rect foldoutRect, bool expanded, string header, Action drawStuff)
        {
            expanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, expanded, header);

            if (expanded)
            {
                EditorGUI.indentLevel++;
                drawStuff();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return expanded;
        }

        public static (Rect foldoutRect, Rect buttonRect) GetFoldoutAndButtonRects(string header)
        {
            const float buttonWidth = 60f;

            Rect foldoutWithoutButton = GUILayoutUtility.GetRect(TempContent(header), EditorStyles.foldoutHeader);

            var foldoutRect = new Rect(
                foldoutWithoutButton.x,
                foldoutWithoutButton.y,
                foldoutWithoutButton.width - buttonWidth,
                foldoutWithoutButton.height);

            var buttonRect = new Rect(
                foldoutWithoutButton.xMax - buttonWidth,
                foldoutWithoutButton.y,
                buttonWidth,
                foldoutWithoutButton.height);

            return (foldoutRect, buttonRect);
        }

        private static GUIContent TempContent(string text)
        {
            _tempContent.text = text;
            return _tempContent;
        }
    }
}