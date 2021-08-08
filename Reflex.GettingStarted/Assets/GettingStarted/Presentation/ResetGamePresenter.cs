using Reflex.Scripts.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class ResetGamePresenter : MonoBehaviour
{
    [SerializeField] private Button _resetGameButton;
    [MonoInject] private IResetGame _resetGame;

    private void Start()
    {
        _resetGameButton.onClick.AddListener(() => _resetGame.Reset());
    }
}