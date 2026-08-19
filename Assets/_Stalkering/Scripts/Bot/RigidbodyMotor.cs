using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMotor : MonoBehaviour, IMovementMotor
{
    private const float ClimbSkin = 0.02f;

    private Rigidbody _rigidbody;
    private IStepClimber _stepClimber;
    private Vector3 _desiredPlanarVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints |= RigidbodyConstraints.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Construct(IStepClimber stepClimber)
    {
        _stepClimber = stepClimber;
    }

    public void Move(Vector3 worldDirection, float speed)
    {
        Vector3 planar = new Vector3(worldDirection.x, 0f, worldDirection.z);
        if (planar.sqrMagnitude > 1f)
        {
            planar.Normalize();
        }

        _desiredPlanarVelocity = planar * speed;
    }

    private void FixedUpdate()
    {
        if (_rigidbody == null)
        {
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x = _desiredPlanarVelocity.x;
        velocity.z = _desiredPlanarVelocity.z;

        if (_stepClimber != null &&
            _stepClimber.TryClimb(_desiredPlanarVelocity, out float climbHeight) &&
            climbHeight > 0f)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector3.up * (climbHeight + ClimbSkin));
            velocity.y = 0f;
        }

        _rigidbody.linearVelocity = velocity;
    }
}
