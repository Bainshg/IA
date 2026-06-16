using UnityEngine;

// estado normal del enjambre: las 3 reglas (separation, alignment, cohesion) + patrulla
// colectiva hacia los waypoints del manager.
public class FlockingState : FlockState
{
    public FlockingState(FlockAgent agent, StateMachine<FlockStates> sm) : base(agent, sm) { }

    public override void Execute()
    {
        SteeringAgent steering = _agent.Steering;
        FlockManager flock = FlockManager.Instance;
        if (flock == null) return;

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        var neighbors = _agent.Neighbors;
        if (neighbors.Count > 0)
        {
            Vector3 averageVelocity = Vector3.zero;
            Vector3 averagePosition = Vector3.zero;

            for (int i = 0; i < neighbors.Count; i++)
            {
                FlockAgent neighbor = neighbors[i];
                Vector3 toMe = _agent.transform.position - neighbor.transform.position;
                if (toMe.sqrMagnitude > 0f)
                {
                    separation += toMe.normalized / toMe.magnitude;
                }
                averageVelocity += neighbor.Steering.Velocity;
                averagePosition += neighbor.transform.position;
            }

            averageVelocity /= neighbors.Count;
            alignment = averageVelocity - steering.Velocity;

            averagePosition /= neighbors.Count;
            cohesion = SteeringBehaviours.Seek(_agent.transform, averagePosition, steering.Velocity, steering.MaxSpeed);
        }

        // voy al waypoint por ruta Theta* (rodea paredes). el avance de waypoint sigue
        // siendo colectivo via HasReachedPoint.
        if (_agent.Patrol.HasReachedPoint())
        {
            _agent.Patrol.UpdateToNextPoint();
        }
        Vector3 patrolForce = _agent.SteerAlongPathTo(_agent.Patrol.GetCurrentWaypoint());

        Vector3 flockMove = (separation * flock.separationWeight) +
                            (alignment * flock.alignmentWeight) +
                            (cohesion * flock.cohesionWeight) +
                            (patrolForce * flock.patrolWeight);

        steering.ApplySteering(flockMove);
    }
}
