using UnityEngine;

public class StepClimber : MonoBehaviour, IStepClimber
{
    private const int HitBufferSize = 16;

    [SerializeField]
    [Min(0f)]
    private float _maxStepHeight = 0.35f;

    [SerializeField]
    [Min(0.05f)]
    private float _stepCheckDistance = 0.4f;

    [SerializeField]
    [Min(0.05f)]
    private float _probeRadius = 0.2f;

    [SerializeField]
    [Min(0f)]
    private float _skinWidth = 0.08f;

    [SerializeField]
    private LayerMask _collisionMask = ~0;

    private readonly RaycastHit[] _hits = new RaycastHit[HitBufferSize];

    public bool TryClimb(Vector3 planarVelocity, out float climbHeight)
    {
        climbHeight = 0f;

        Vector3 direction = new Vector3(planarVelocity.x, 0f, planarVelocity.z);
        if (direction.sqrMagnitude < 0.01f)
        {
            return false;
        }

        direction.Normalize();

        if (_collisionMask == 0)
        {
            _collisionMask = ~0;
        }

        if (!IsGrounded())
        {
            return false;
        }

        Vector3 origin = transform.position + Vector3.up * (_probeRadius + _skinWidth);
        if (!SphereCastIgnoringSelf(origin, _probeRadius, direction, _stepCheckDistance, out RaycastHit lowHit))
        {
            return false;
        }

        if (Vector3.Angle(lowHit.normal, Vector3.up) < 50f)
        {
            return false;
        }

        float upperRadius = _probeRadius * 0.85f;
        Vector3 upperOrigin = transform.position + Vector3.up * (_maxStepHeight + upperRadius + _skinWidth);
        if (SphereCastIgnoringSelf(upperOrigin, upperRadius, direction, _stepCheckDistance, out RaycastHit highHit) &&
            highHit.distance <= lowHit.distance + 0.05f)
        {
            return false;
        }

        Vector3 landingOrigin = transform.position
            + direction * (lowHit.distance + _probeRadius + 0.05f)
            + Vector3.up * (_maxStepHeight + _skinWidth);

        if (!RaycastIgnoringSelf(landingOrigin, Vector3.down, _maxStepHeight + _skinWidth + 0.1f, out RaycastHit groundHit))
        {
            return false;
        }

        if (Vector3.Angle(groundHit.normal, Vector3.up) > 50f)
        {
            return false;
        }

        float stepHeight = groundHit.point.y - transform.position.y;
        if (stepHeight <= 0.02f || stepHeight > _maxStepHeight + 0.02f)
        {
            return false;
        }

        climbHeight = Mathf.Min(stepHeight, _maxStepHeight);
        return true;
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * (_probeRadius + _skinWidth);
        return SphereCastIgnoringSelf(origin, _probeRadius, Vector3.down, _probeRadius + 0.2f, out _);
    }

    private bool SphereCastIgnoringSelf(
        Vector3 origin,
        float radius,
        Vector3 direction,
        float distance,
        out RaycastHit hit)
    {
        int count = Physics.SphereCastNonAlloc(
            origin,
            radius,
            direction,
            _hits,
            distance,
            _collisionMask,
            QueryTriggerInteraction.Ignore);

        return SelectNearestWorldHit(count, out hit);
    }

    private bool RaycastIgnoringSelf(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        int count = Physics.RaycastNonAlloc(
            origin,
            direction,
            _hits,
            distance,
            _collisionMask,
            QueryTriggerInteraction.Ignore);

        return SelectNearestWorldHit(count, out hit);
    }

    private bool SelectNearestWorldHit(int count, out RaycastHit hit)
    {
        hit = default;
        float bestDistance = float.MaxValue;
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            RaycastHit candidate = _hits[i];
            if (candidate.collider == null)
            {
                continue;
            }

            Transform root = candidate.collider.transform.root;
            if (root == transform.root)
            {
                continue;
            }

            if (candidate.collider.GetComponentInParent<CharacterLocomotion>() != null)
            {
                continue;
            }

            if (candidate.distance < bestDistance)
            {
                bestDistance = candidate.distance;
                hit = candidate;
                found = true;
            }
        }

        return found;
    }
}
