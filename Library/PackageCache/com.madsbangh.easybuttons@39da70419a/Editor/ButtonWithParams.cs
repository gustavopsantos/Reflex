namespace EasyButtons.Editor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;
    using System.Collections.Generic;

    internal class ButtonWithParams : Button
    {
        private readonly Parameter[] _parameters;
        private bool _expanded;

        public ButtonWithParams(MethodInfo method, ButtonAttribute buttonAttribute, ParameterInfo[] parameters)
            : base(method, buttonAttribute)
        {
            _parameters = parameters.Select(paramInfo => new Parameter(paramInfo)).ToArray();
            _expanded = buttonAttribute.Expanded;
        }

        protected override void DrawInternal(IEnumerable<object> targets)
        {
            (Rect foldoutRect, Rect buttonRect) = DrawUtility.GetFoldoutAndButtonRects(DisplayName);

            _expanded = DrawUtility.DrawInFoldout(foldoutRect, _expanded, DisplayName, () =>
            {
                foreach (Parameter param in _parameters)
                {
                    param.Draw();
                }
            });

            if ( ! GUI.Button(buttonRect, "Invoke"))
                return;

            var paramValues = _parameters.Select(param => param.Value).ToArray();

            foreach (object obj in targets)
            {
                Method.Invoke(obj, paramValues);
            }
        }

        private readonly struct Parameter
        {
            private readonly FieldInfo _fieldInfo;
            private readonly ScriptableObject _scriptableObj;
            private readonly NoScriptFieldEditor _editor;

            public Parameter(ParameterInfo paramInfo)
            {
                Type generatedType = ScriptableObjectCache.GetClass(paramInfo.Name, paramInfo.ParameterType);
                _scriptableObj = ScriptableObject.CreateInstance(generatedType);
                _fieldInfo = generatedType.GetField(paramInfo.Name);
                _editor = CreateEditor<NoScriptFieldEditor>(_scriptableObj);
            }

            public object Value
            {
                get
                {
                    // Every time modified properties are applied, the "No script asset for ..." warning appears.
                    // Saving only once before invoking the button minimizes those warnings.
                    _editor.ApplyModifiedProperties();
                    return _fieldInfo.GetValue(_scriptableObj);
                }
            }

            public void Draw()
            {
                _editor.OnInspectorGUI();
            }

            private static TEditor CreateEditor<TEditor>(Object obj)
                where TEditor : Editor
            {
                return (TEditor) Editor.CreateEditor(obj, typeof(TEditor));
            }
        }
    }
}