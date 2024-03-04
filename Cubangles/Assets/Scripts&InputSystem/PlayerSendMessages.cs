using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSendMessages : MonoBehaviour
{
    [HideInInspector] public Vector2 move;
    [HideInInspector] public Vector2 look;
    void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    void OnLookFire(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }
    void MoveInput(Vector2 newMoveValue)
    {
        move = newMoveValue;
    }
    void LookInput(Vector2 newLookValue)
    {
        look = newLookValue;
    }
}
