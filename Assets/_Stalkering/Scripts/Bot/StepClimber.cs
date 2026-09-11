using UnityEngine;

[RequireComponent(typeof(GroundChecker))]
public class StepClimber : MonoBehaviour, IStepClimber
{
    private const int HitBufferSize = 16;
    private const float MinMoveSqrMagnitude = 0.01f;
    private const float MaxWalkableSlopeAngle = 50f;
    private const float UpperProbeRadiusFactor = 0.85f;
    private const float HitDistanceTolerance = 0.05f;
    private const float LandingAdvanceMargin = 0.05f;
    private const float LandingRayExtraDistance = 0.1f;
    private const float MinStepHeight = 0.02f;
    private const float StepHeightTolerance = 0.02f;

    [SerializeField] [Min(0f)] private float _maxStepHeight = 0.35f;
    [SerializeField] [Min(0.05f)] private float _stepCheckDistance = 0.4f;
    [SerializeField] [Min(0.05f)] private float _probeRadius = 0.2f;
    [SerializeField] [Min(0f)] private float _skinWidth = 0.08f;
    private GroundChecker _groundChecker;
    private readonly RaycastHit[] _hits = new RaycastHit[HitBufferSize];

    public void Construct(GroundChecker groundChecker) =>
        _groundChecker = groundChecker;

    public bool TryClimb(Vector3 planarVelocity, out float climbHeight)
    {
        climbHeight = 0f;

        if (TryGetMovementDirection(planarVelocity, out Vector3 direction) == false)
            return false;

        if (_groundChecker == null || _groundChecker.IsGrounded == false)
            return false;

        if (TryFindStepEdge(direction, out RaycastHit stepHit) == false)
            return false;

        if (IsBlockedAbove(direction, stepHit))
            return false;

        if (TryFindTraversableGround(direction, stepHit, out RaycastHit groundHit) == false)
            return false;

        return TryEvaluateStepHeight(groundHit, out climbHeight);
    }

    private bool TryGetMovementDirection(Vector3 planarVelocity, out Vector3 direction)
    {
        direction = planarVelocity.normalized;

        if (direction.sqrMagnitude < MinMoveSqrMagnitude)
            return false;

        return true;
    }

    private bool TryFindStepEdge(Vector3 direction, out RaycastHit stepHit)
    {
        Vector3 origin = transform.position + Vector3.up * (_probeRadius + _skinWidth);

        if (SphereCastNearest(origin, _probeRadius, direction, _stepCheckDistance, out stepHit) == false)
            return false;

        if (Vector3.Angle(stepHit.normal, Vector3.up) < MaxWalkableSlopeAngle)
            return false;

        return true;
    }

    private bool IsBlockedAbove(Vector3 direction, RaycastHit stepHit)
    {
        float upperRadius = _probeRadius * UpperProbeRadiusFactor;
        Vector3 upperOrigin = transform.position + Vector3.up * (_maxStepHeight + upperRadius + _skinWidth);

        if (SphereCastNearest(upperOrigin, upperRadius, direction, _stepCheckDistance, out RaycastHit highHit) == false)
            return false;

        return highHit.distance <= stepHit.distance + HitDistanceTolerance;
    }

    private bool TryFindTraversableGround(Vector3 direction, RaycastHit stepHit, out RaycastHit groundHit)
    {
        Vector3 landingOrigin = transform.position
            + direction * (stepHit.distance + _probeRadius + LandingAdvanceMargin)
            + Vector3.up * (_maxStepHeight + _skinWidth);

        if (RaycastNearest(landingOrigin, Vector3.down, _maxStepHeight + _skinWidth + LandingRayExtraDistance, out groundHit) == false)
            return false;

        if (Vector3.Angle(groundHit.normal, Vector3.up) > MaxWalkableSlopeAngle)
            return false;

        return true;
    }

    private bool TryEvaluateStepHeight(RaycastHit groundHit, out float climbHeight)
    {
        climbHeight = 0f;
        float stepHeight = groundHit.point.y - transform.position.y;

        if (stepHeight <= MinStepHeight || stepHeight > _maxStepHeight + StepHeightTolerance)
            return false;

        climbHeight = Mathf.Min(stepHeight, _maxStepHeight);

        return true;
    }

    private bool SphereCastNearest(Vector3 origin, float radius, Vector3 direction, float distance, out RaycastHit hit)
    {
        int count = Physics.SphereCastNonAlloc(
            origin,
            radius,
            direction,
            _hits,
            distance,
            _groundChecker.GroundMask,
            QueryTriggerInteraction.Ignore);

        return SelectNearestHit(count, out hit);
    }

    private bool RaycastNearest(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        int count = Physics.RaycastNonAlloc(
            origin,
            direction,
            _hits,
            distance,
            _groundChecker.GroundMask,
            QueryTriggerInteraction.Ignore);

        return SelectNearestHit(count, out hit);
    }

    private bool SelectNearestHit(int count, out RaycastHit hit)
    {
        hit = default;
        float bestDistance = float.MaxValue;
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            RaycastHit candidate = _hits[i];

            if (candidate.collider == null)
                continue;

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