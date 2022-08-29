using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
public class ScriptableObjectGameSettings : ScriptableObject, IGameSettings
{
    [field: SerializeField] public float CharacterMovementSpeed { get; private set; }
}