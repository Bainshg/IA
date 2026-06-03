using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float mass = 1f;
    private Vector3 _velocity;

    public void ApplySteering(Vector3 steeringForce)
    {
        steeringForce.y = 0f;
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        Vector3 acceleration = steeringForce / mass;

        _velocity += acceleration * Time.deltaTime;
        _velocity.y = 0f;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);

        Vector3 nextPosition = transform.position + _velocity * Time.deltaTime;
        Vector3 rayDirection = _velocity.normalized;
        if (rayDirection != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, 1.0f, LayerMask.GetMask("Obstacle")))
            {
                Vector3 avoidanceDirection = Vector3.Reflect(_velocity, hit.normal);
                avoidanceDirection.y = 0f;

                _velocity = avoidanceDirection.normalized * maxSpeed * 0.7f;

                nextPosition = transform.position + _velocity * Time.deltaTime;
            }
        }
        if (GridManager.Instance != null)
        {
            Node checkNode = GridManager.Instance.NodeFromWorldPoint(nextPosition);
            if (checkNode != null && checkNode.walkable)
            {
                transform.position = nextPosition;
            }
            else
            {
                _velocity = Vector3.zero;
            }
        }
        else
        {
            transform.position = nextPosition;
        }

        if (_velocity.sqrMagnitude > 0.1f)
            transform.forward = _velocity.normalized;
    }
    public void StopAgent()
    {
        _velocity = Vector3.zero;
    }
    public Vector3 Velocity => _velocity;
    public float MaxSpeed => maxSpeed;
}