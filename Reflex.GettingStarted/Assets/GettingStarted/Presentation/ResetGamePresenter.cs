using UnityEngine.Scripting;
using Reflex.Scripts.Attributes;

public class ResetGamePresenter : Presenter<ResetGameView>
{
    [MonoInject, Preserve]
    private void Inject(IResetGame resetGame)
    {
        View.ResetButton.onClick.AddListener(resetGame.Reset);
    }
}