using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomPropertyDrawer(typeof(Navigation), true)]
    /// <summary>
    /// This is a PropertyDrawer for Navigation. It is implemented using the standard Unity PropertyDrawer framework.
    /// </summary>
    public class NavigationDrawer : PropertyDrawer
    {
        private class Styles
        {
            readonly public GUIContent navigationContent;

            public Styles()
            {
                navigationContent = EditorGUIUtility.TrTextContent("Navigation");
            }
        }

        private static Styles s_Styles = null;

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            Rect drawRect = pos;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            SerializedProperty wrapAround = prop.FindPropertyRelative("m_WrapAround");
            Navigation.Mode navMode = GetNavigationMode(navigation);

            EditorGUI.PropertyField(drawRect, navigation, s_Styles.navigationContent);

            ++EditorGUI.indentLevel;

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            switch (navMode)
            {
                case Navigation.Mode.Horizontal:
                case Navigation.Mode.Vertical:
                {
                    EditorGUI.PropertyField(drawRect, wrapAround);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                break;
                case Navigation.Mode.Explicit:
                {
                    SerializedProperty selectOnUp = prop.FindPropertyRelative("m_SelectOnUp");
                    SerializedProperty selectOnDown = prop.FindPropertyRelative("m_SelectOnDown");
                    SerializedProperty selectOnLeft = prop.FindPropertyRelative("m_SelectOnLeft");
                    SerializedProperty selectOnRight = prop.FindPropertyRelative("m_SelectOnRight");

                    EditorGUI.PropertyField(drawRect, selectOnUp);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(drawRect, selectOnDown);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(drawRect, selectOnLeft);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(drawRect, selectOnRight);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                break;
            }

            --EditorGUI.indentLevel;
        }

        static Navigation.Mode GetNavigationMode(SerializedProperty navigation)
        {
            return (Navigation.Mode)navigation.enumValueIndex;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            if (navigation == null)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Navigation.Mode navMode = GetNavigationMode(navigation);

            switch (navMode)
            {
                case Navigation.Mode.None:
                    return EditorGUIUtility.singleLineHeight;
                case Navigation.Mode.Horizontal:
                case Navigation.Mode.Vertical:
                    return 2 * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
                case Navigation.Mode.Explicit:
                    return 5 * EditorGUIUtility.singleLineHeight + 5 * EditorGUIUtility.standardVerticalSpacing;
                default:
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
