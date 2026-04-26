using UnityEngine;

public class EntityPatrolState : EntityState
{
    private EnemyAI _ai;
    public EntityPatrolState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm) => _ai = ai;

    public override void Execute()
    {
        Vector3 target = _ai.Patrol.GetCurrentWaypoint();
        Vector3 steer = SteeringBehaviours.Seek(_ai.transform, target, _ai.Agent.Velocity, _ai.Agent.MaxSpeed);
        steer += _ai.Avoidance.GetAvoidanceForce();
        _ai.Agent.ApplySteering(steer);

        if (_ai.Patrol.HasReachedPoint())
        {
            _ai.Patrol.UpdateToNextPoint();
            _ai.AddIteration();
        }
    }
}