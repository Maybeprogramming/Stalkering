using UnityEngine;

public interface IMovementMotor
{
    void SetDesiredVelocity(Vector3 planarVelocity);
}