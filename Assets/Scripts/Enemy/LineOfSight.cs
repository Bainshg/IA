using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float angle = 90f;
    [SerializeField] private LayerMask obstacleMask; // Capa de paredes/obstaculos
    [SerializeField] private LayerMask targetMask;   // Capa del Player

    public bool CanSeeTarget(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > range) return false; // Muy lejos 

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToTarget) > angle / 2) return false; // Fuera de angulo 

        // Raycast para ver si hay muros
        if (Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
            return false;

        return true; // Lo ve
    }
}