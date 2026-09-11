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

    private void FixedUpdate()
    {
        if (_rigidbody == null)
            return;

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

    public void Construct(IStepClimber stepClimber) =>
        _stepClimber = stepClimber;

    public void SetDesiredVelocity(Vector3 planarVelocity) =>
        _desiredPlanarVelocity = planarVelocity;
}