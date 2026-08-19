using UnityEngine;

public interface IMovementMotor
{
    void Move(Vector3 worldDirection, float speed);
}
