using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringAgent))]
[RequireComponent(typeof(PatrolBehaviour))]
public class FlockAgent : MonoBehaviour
{
    private SteeringAgent _steeringAgent;
    private PatrolBehaviour _patrol;
    private List<FlockAgent> _neighbors = new List<FlockAgent>();

    void Awake()
    {
        _steeringAgent = GetComponent<SteeringAgent>();
        _patrol = GetComponent<PatrolBehaviour>();
    }

    void Update()
    {
        GetNeighbors();

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        if (_neighbors.Count > 0)
        {
            Vector3 averageVelocity = Vector3.zero;
            Vector3 averagePosition = Vector3.zero;

            foreach (var neighbor in _neighbors)
            {
                Vector3 toMe = transform.position - neighbor.transform.position;
                if (toMe.sqrMagnitude > 0)
                {
                    separation += toMe.normalized / toMe.magnitude;
                }
                averageVelocity += neighbor._steeringAgent.Velocity;
                averagePosition += neighbor.transform.position;
            }

            averageVelocity /= _neighbors.Count;
            alignment = averageVelocity - _steeringAgent.Velocity;

            averagePosition /= _neighbors.Count;
            cohesion = SteeringBehaviours.Seek(transform, averagePosition, _steeringAgent.Velocity, _steeringAgent.MaxSpeed);
        }

        // 4. PATRULLA COLECTIVA: Seguir el waypoint actual que heredó de la escena
        Vector3 patrolTarget = _patrol.GetCurrentWaypoint();
        Vector3 patrolForce = SteeringBehaviours.Seek(transform, patrolTarget, _steeringAgent.Velocity, _steeringAgent.MaxSpeed);

        // Si llegó al punto, actualiza al siguiente (igual que tu IA vieja)
        if (_patrol.HasReachedPoint())
        {
            _patrol.UpdateToNextPoint();
        }

        // Sumamos todo
        Vector3 flockMove = (separation * FlockManager.Instance.separationWeight) +
                            (alignment * FlockManager.Instance.alignmentWeight) +
                            (cohesion * FlockManager.Instance.cohesionWeight) +
                            (patrolForce * FlockManager.Instance.patrolWeight);

        _steeringAgent.ApplySteering(flockMove);
    }

    private void GetNeighbors()
    {
        _neighbors.Clear();
        foreach (var agent in FlockManager.Instance.Agents)
        {
            if (agent == this) continue;

            float dist = Vector3.Distance(transform.position, agent.transform.position);
            if (dist <= FlockManager.Instance.neighborRadius)
            {
                _neighbors.Add(agent);
            }
        }
    }
}