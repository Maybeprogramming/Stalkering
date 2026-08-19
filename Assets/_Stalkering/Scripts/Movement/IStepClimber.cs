using UnityEngine;

public interface IStepClimber
{
    bool TryClimb(Vector3 planarVelocity, out float climbHeight);
}
