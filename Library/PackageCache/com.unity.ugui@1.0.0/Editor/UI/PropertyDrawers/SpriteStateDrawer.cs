using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomPropertyDrawer(typeof(SpriteState), true)]
    /// <summary>
    /// This is a PropertyDrawer for SpriteState. It is implemented using the standard Unity PropertyDrawer framework.
    /// </summary>
    public class SpriteStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty highlightedSprite = prop.FindPropertyRelative("m_HighlightedSprite");
            SerializedProperty pressedSprite = prop.FindPropertyRelative("m_PressedSprite");
            SerializedProperty selectedSprite = prop.FindPropertyRelative("m_SelectedSprite");
            SerializedProperty disabledSprite = prop.FindPropertyRelative("m_DisabledSprite");

            EditorGUI.PropertyField(drawRect, highlightedSprite);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, pressedSprite);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, selectedSprite);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, disabledSprite);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight + 3 * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
