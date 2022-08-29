namespace EasyButtons.Editor
{
    using System.Reflection;
    using UnityEngine;
    using System.Collections.Generic;

    internal class ButtonWithoutParams : Button
    {
        public ButtonWithoutParams(MethodInfo method, ButtonAttribute buttonAttribute)
            : base(method, buttonAttribute) { }

        protected override void DrawInternal(IEnumerable<object> targets)
        {
            if ( ! GUILayout.Button(DisplayName))
                return;

            foreach (object obj in targets)
            {
                Method.Invoke(obj, null);
            }
        }
    }
}