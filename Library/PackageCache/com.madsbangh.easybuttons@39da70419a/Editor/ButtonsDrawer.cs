namespace EasyButtons.Editor
{
    using System.Collections.Generic;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Helper class that can be used in custom Editors to draw methods marked with the <see cref="ButtonAttribute"/> as buttons.
    /// </summary>
    public class ButtonsDrawer
    {
        /// <summary>
        /// A list of buttons that can be drawn for the class.
        /// </summary>
        [PublicAPI]
        public readonly List<Button> Buttons = new List<Button>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonsDrawer"/> class and fills <see cref="Buttons"/> with
        /// methods marked with the <see cref="ButtonAttribute"/>. Recommended to instantiate it in OnEnable to improve
        /// performance of the custom editor.
        /// </summary>
        /// <param name="target">Editor's target.</param>
        public ButtonsDrawer(object target)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = target.GetType().GetMethods(flags);

            foreach (MethodInfo method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute == null)
                    continue;

                Buttons.Add(Button.Create(method, buttonAttribute));
            }
        }

        /// <summary>
        /// Draws all the methods marked with <see cref="ButtonAttribute"/>.
        /// </summary>
        public void DrawButtons(IEnumerable<object> targets)
        {
            foreach (Button button in Buttons)
            {
                button.Draw(targets);
            }
        }
    }
}