using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMotor : MonoBehaviour, IMovementMotor
{
    private CharacterController _controller;
    private Vector3 _desiredPlanarVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
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

    private void Update()
    {
        if (_controller == null || !_controller.enabled)
        {
            return;
        }

        _controller.SimpleMove(_desiredPlanarVelocity);
    }
}
