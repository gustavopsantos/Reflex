using UnityEngine;

public class GetPlayerInput : IGetPlayerInput
{
    public PlayerInput Get()
    {
        return new PlayerInput
        {
            Vertical = Input.GetAxis("Vertical"),
            Horizontal = Input.GetAxis("Horizontal"),
        };
    }
}