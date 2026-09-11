using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private const int HitBufferSize = 16;

    [SerializeField] private Vector3 _detectorOffset = new Vector3(0f, 0.1f, 0f);
    [SerializeField] [Min(0f)] private float _detectionRadius = 0.2f;
    [SerializeField] private LayerMask _groundMask;
    private readonly Collider[] _overlaps = new Collider[HitBufferSize];

    public LayerMask GroundMask => _groundMask;

    public bool IsGrounded => CheckGround();

    private bool CheckGround()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position + _detectorOffset,
            _detectionRadius,
            _overlaps,
            _groundMask,
            QueryTriggerInteraction.Ignore);

        return count > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _detectorOffset, _detectionRadius);
    }
}