using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField] private float avoidRange = 4f;
    [SerializeField] private float sideRayAngle = 30f; // Ángulo de las antenas laterales
    [SerializeField] private LayerMask obstacleMask;

    public Vector3 GetAvoidanceForce()
    {
        Vector3 finalAvoidance = Vector3.zero;

        // 1. Rayo Central
        finalAvoidance += CalculateAvoidance(transform.forward);

        // 2. Rayo Derecho
        Vector3 rightDir = Quaternion.AngleAxis(sideRayAngle, Vector3.up) * transform.forward;
        finalAvoidance += CalculateAvoidance(rightDir);

        // 3. Rayo Izquierdo
        Vector3 leftDir = Quaternion.AngleAxis(-sideRayAngle, Vector3.up) * transform.forward;
        finalAvoidance += CalculateAvoidance(leftDir);

        return finalAvoidance;
    }

    private Vector3 CalculateAvoidance(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, avoidRange, obstacleMask))
        {
            // Cuanto más cerca está el obstáculo, más fuerte es la fuerza de esquive
            float multiplier = 1.0f + (avoidRange - hit.distance) / avoidRange;
            Vector3 avoidForce = hit.normal * 10f * multiplier;
            avoidForce.y = 0;
            return avoidForce;
        }
        return Vector3.zero;
    }

    // Para ver las "antenas" en el editor (Gizmos)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * avoidRange);
        Vector3 rightDir = Quaternion.AngleAxis(sideRayAngle, Vector3.up) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * (avoidRange * 0.7f));
        Vector3 leftDir = Quaternion.AngleAxis(-sideRayAngle, Vector3.up) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDir * (avoidRange * 0.7f));
    }
}