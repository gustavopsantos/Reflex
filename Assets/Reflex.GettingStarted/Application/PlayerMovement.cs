using Reflex.Scripts;
using UnityEngine;
using Reflex.Scripts.Attributes;

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    [Inject] private readonly IContainer _container;
    [Inject] private readonly IGameSettings _gameSettings;
    [Inject] private readonly IGetPlayerInput _getPlayerInput;

    private void Start()
    {
        _container.BindSingleton<IPlayerMovement>(this);
    }

    private void Update()
    {
        var playerInput = _getPlayerInput.Get();
        var motion = Vector3.ClampMagnitude(new Vector3(playerInput.Horizontal, 0, playerInput.Vertical), 1);
        var frameMotion = motion * (_gameSettings.CharacterMovementSpeed * Time.deltaTime);
        transform.Translate(frameMotion);
    }

    public void ResetGame()
    {
        transform.position = Vector3.zero;
    }
}