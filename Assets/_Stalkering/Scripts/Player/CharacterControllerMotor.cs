using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMotor : MonoBehaviour, IMovementMotor
{
    private const float UnitSquaredMagnitude = 1f;

    private CharacterController _controller;
    private Vector3 _desiredPlanarVelocity;

    private void Awake() =>
        _controller = GetComponent<CharacterController>();

    public void Move(Vector3 worldDirection, float speed)
    {
        Vector3 planar = new Vector3(worldDirection.x, Vector3.zero.y, worldDirection.z);

        if (planar.sqrMagnitude > UnitSquaredMagnitude)
            planar.Normalize();

        _desiredPlanarVelocity = planar * speed;
    }

    private void Update()
    {
        if (_controller == null || _controller.enabled == false)
            return;

        _controller.SimpleMove(_desiredPlanarVelocity);
    }
}