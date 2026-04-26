using UnityEngine;

public class EntityChaseState : EntityState
{
    private EnemyAI _ai;
    public EntityChaseState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm) => _ai = ai;

    public override void Execute()
    {
        // Obtenemos la velocidad del jugador (necesaria para Pursuit)
        Vector3 targetVel = Vector3.zero;
        if (_ai.PlayerTransform.TryGetComponent(out PlayerMovement pm))
            targetVel = pm.Velocity; // Asegúrate de que PlayerMovement tenga una propiedad Velocity

        Vector3 steer = SteeringBehaviours.Pursuit(
            _ai.transform,
            _ai.PlayerTransform,
            targetVel,
            _ai.Agent.Velocity,
            _ai.Agent.MaxSpeed
        );

        steer += _ai.Avoidance.GetAvoidanceForce();
        _ai.Agent.ApplySteering(steer);
    }
}