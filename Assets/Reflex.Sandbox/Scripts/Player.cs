using UnityEngine;
using Reflex.Scripts.Attributes;

public class Player : MonoBehaviour
{
    [Inject] public int Int;
    [Inject] public string String;
}