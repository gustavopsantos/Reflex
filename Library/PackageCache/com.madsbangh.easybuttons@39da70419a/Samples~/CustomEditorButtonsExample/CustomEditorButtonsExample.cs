namespace EasyButtons.Example
{
    using UnityEngine;

    public class CustomEditorButtonsExample : MonoBehaviour
    {
        [Button("Custom Editor Example")]
        private void SayHello()
        {
            Debug.Log("Hello from custom editor");
        }

        [Button]
        private void SecondButton()
        {
            Debug.Log("Second button of the custom editor.");
        }
    }
}
