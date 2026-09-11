using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private const float UnitSquaredMagnitude = 1f;

    [SerializeField] private float _moveSpeed = 5f;
    private IMoveInput _moveInput;
    private IMovementMotor _motor;

    private void Update()
    {
        if (_moveInput == null || _motor == null)
            return;

        Vector2 input = _moveInput.GetMoveDirection();
        Vector3 planarDirection = new Vector3(input.x, 0f, input.y);

        if (planarDirection.sqrMagnitude > UnitSquaredMagnitude)
            planarDirection.Normalize();

        _motor.SetDesiredVelocity(planarDirection * _moveSpeed);
    }

    public void Construct(IMoveInput moveInput, IMovementMotor motor)
    {
        _moveInput = moveInput;
        _motor = motor;
    }
}