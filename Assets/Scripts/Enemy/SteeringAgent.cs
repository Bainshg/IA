using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float mass = 1f;
    private Vector3 _velocity;

    public void ApplySteering(Vector3 steeringForce)
    {
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        Vector3 acceleration = steeringForce / mass;
        _velocity = Vector3.ClampMagnitude(_velocity + acceleration * Time.deltaTime, maxSpeed);
        transform.position += _velocity * Time.deltaTime;

        if (_velocity.sqrMagnitude > 0.1f)
            transform.forward = _velocity.normalized;
    }

    public Vector3 Velocity => _velocity;
    public float MaxSpeed => maxSpeed;
}