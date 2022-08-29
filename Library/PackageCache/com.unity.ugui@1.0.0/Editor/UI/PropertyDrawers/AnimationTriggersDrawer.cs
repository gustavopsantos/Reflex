using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomPropertyDrawer(typeof(AnimationTriggers), true)]
    /// <summary>
    /// This is a PropertyDrawer for AnimationTriggers. It is implemented using the standard Unity PropertyDrawer framework.
    /// </summary>
    public class AnimationTriggersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty normalTrigger = prop.FindPropertyRelative("m_NormalTrigger");
            SerializedProperty higlightedTrigger = prop.FindPropertyRelative("m_HighlightedTrigger");
            SerializedProperty pressedTrigger = prop.FindPropertyRelative("m_PressedTrigger");
            SerializedProperty selectedTrigger = prop.FindPropertyRelative("m_SelectedTrigger");
            SerializedProperty disabledTrigger = prop.FindPropertyRelative("m_DisabledTrigger");

            EditorGUI.PropertyField(drawRect, normalTrigger);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, higlightedTrigger);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, pressedTrigger);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, selectedTrigger);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, disabledTrigger);
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 5 * EditorGUIUtility.singleLineHeight + 4 * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
