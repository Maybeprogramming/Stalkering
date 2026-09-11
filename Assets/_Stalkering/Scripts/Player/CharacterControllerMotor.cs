using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMotor : MonoBehaviour, IMovementMotor
{
    private CharacterController _controller;
    private Vector3 _desiredPlanarVelocity;

    private void Awake() =>
        _controller = GetComponent<CharacterController>();

    private void Update()
    {
        if (_controller == null || _controller.enabled == false)
            return;

        _controller.SimpleMove(_desiredPlanarVelocity);
    }

    public void SetDesiredVelocity(Vector3 planarVelocity) =>
        _desiredPlanarVelocity = planarVelocity;
}