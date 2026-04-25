using UnityEngine;

public class EntityRunAwayState : EntityState
{
    private EnemyAI _ai;

    public EntityRunAwayState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
    }

    public override void Execute()
    {
        // Huimos del jugador usando Flee
        Vector3 targetPos = _ai.PlayerTransform.position;
        Vector3 steer = SteeringBehaviours.Flee(_ai.transform, targetPos, _ai.Agent.Velocity, _ai.Agent.MaxSpeed);

        // El esquive también se aplica al huir
        steer += _ai.Avoidance.GetAvoidanceForce();

        _ai.Agent.ApplySteering(steer);
    }
}