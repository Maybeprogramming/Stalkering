using UnityEngine;

public class PlayerMoveInput : MonoBehaviour, IMoveInput
{
    private global::InputSystem _inputs;

    private void Awake() =>
        _inputs = new global::InputSystem();

    private void OnEnable() =>
        _inputs?.Enable();

    private void OnDisable() =>
        _inputs?.Disable();

    private void OnDestroy()
    {
        _inputs?.Dispose();
        _inputs = null;
    }

    public Vector2 GetMoveDirection() =>
        _inputs != null ? _inputs.Player.Move.ReadValue<Vector2>() : Vector2.zero;
}