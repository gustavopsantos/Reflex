using UnityEngine;

public class Presenter<TView> : MonoBehaviour where TView : MonoBehaviour
{
    public TView View { get; private set; }

    private void Awake()
    {
        View = ObjectUtilities.FindObjectOfType<TView>(true);
    }
}