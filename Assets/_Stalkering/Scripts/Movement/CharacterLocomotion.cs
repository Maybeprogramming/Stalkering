using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    private IMoveInput _moveInput;
    private IMovementMotor _motor;

    public void Construct(IMoveInput moveInput, IMovementMotor motor)
    {
        _moveInput = moveInput;
        _motor = motor;
    }

    private void Update()
    {
        if (_moveInput == null || _motor == null)
        {
            return;
        }

        Vector2 input = _moveInput.MoveDirection;
        Vector3 worldDirection = new Vector3(input.x, 0f, input.y);
        _motor.Move(worldDirection, _moveSpeed);
    }
}
