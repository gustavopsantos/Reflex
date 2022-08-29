namespace EasyButtons.Example
{
    using System.Linq;
    using Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(CustomEditorButtonsExample))]
    public class CustomEditorButtonsExampleEditor : Editor
    {
        private ButtonsDrawer _buttonsDrawer;

        // Instantiate ButtonsDrawer in OnEnable if possible.
        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.HelpBox("Message from the custom editor.", MessageType.Info);

            // You can draw all buttons at once.
            _buttonsDrawer.DrawButtons(targets);

            // As well as a specific button in the wanted place.
            _buttonsDrawer.Buttons.First(button => button.DisplayName == "Custom Editor Example").Draw(targets);
        }
    }
}
