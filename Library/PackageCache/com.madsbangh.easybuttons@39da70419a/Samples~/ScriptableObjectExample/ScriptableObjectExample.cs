namespace EasyButtons.Example
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ScriptableObjectExample.asset", menuName = "EasyButtons/ScriptableObjectExample")]
    public class ScriptableObjectExample : ScriptableObject
    {
        [Button]
        public void SayHello()
        {
            Debug.Log("Hello");
        }

        [Button(Mode = ButtonMode.DisabledInPlayMode)]
        public void SayHelloEditor()
        {
            Debug.Log("Hello from edit mode");
        }

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void SayHelloPlayMode()
        {
            Debug.Log("Hello from play mode");
        }
    }
}
