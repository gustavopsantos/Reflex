namespace EasyButtons.Editor
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class NoScriptFieldEditor : Editor
    {
        private static readonly MethodInfo _removeLogEntriesByMode;
        private static readonly string[] _propertiesToExclude = { "m_Script" };

        static NoScriptFieldEditor()
        {
            const string logEntryClassName = "UnityEditor.LogEntry";
            const string removeLogMethodName = "RemoveLogEntriesByMode";

            var editorAssembly = Assembly.GetAssembly(typeof(Editor));
            Type logEntryType = editorAssembly.GetType(logEntryClassName);
            _removeLogEntriesByMode = logEntryType.GetMethod(removeLogMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            if (_removeLogEntriesByMode == null)
            {
                Debug.LogError($"Could not find the {logEntryClassName}.{removeLogMethodName}() method. " +
                               "Please submit an issue and specify your Unity version: https://github.com/madsbangh/EasyButtons/issues/new");
            }
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, _propertiesToExclude);
        }

        public void ApplyModifiedProperties()
        {
            if ( ! serializedObject.hasModifiedProperties)
                return;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            RemoveNoScriptWarning();
        }

        private static void RemoveNoScriptWarning()
        {
            // The warning doesn't appear in edit mode.
            if ( ! Application.isPlaying)
                return;

            // The "No Script asset for ..." log has a unique identifier that can be used to remove the warning.
            const int noScriptAssetMode = 262144;
            _removeLogEntriesByMode?.Invoke(null, new object[] { noScriptAssetMode });
        }
    }
}