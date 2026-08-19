using UnityEngine;

public class PursuitMoveInput : MonoBehaviour, IMoveInput
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    [Min(0f)]
    private float _stoppingDistance = 2.5f;

    public Vector2 MoveDirection
    {
        get
        {
            if (_target == null)
            {
                return Vector2.zero;
            }

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;
            float stoppingSqr = _stoppingDistance * _stoppingDistance;
            if (toTarget.sqrMagnitude <= stoppingSqr)
            {
                return Vector2.zero;
            }

            Vector3 direction = toTarget.normalized;
            return new Vector2(direction.x, direction.z);
        }
    }

    public void Construct(Transform target)
    {
        _target = target;
    }
}
