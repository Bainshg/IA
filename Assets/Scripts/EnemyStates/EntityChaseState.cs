using UnityEngine;

public class EntityChaseState : EntityState
{
    private EnemyAI _ai;
    private PlayerMovement _playerMovement;

    public EntityChaseState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        _playerMovement = _ai.PlayerTransform.GetComponent<PlayerMovement>();
    }

    public override void Execute()
    {
        Vector3 playerVelocity = _playerMovement != null ? _playerMovement.Velocity : Vector3.zero;
        // Obtenemos la velocidad del jugador (para Pursuit)
        Vector3 steer = SteeringBehaviours.Pursuit(
            _ai.transform, 
            _ai.PlayerTransform, 
            playerVelocity, 
            _ai.Agent.Velocity, 
            _ai.Agent.MaxSpeed
        );
    
        steer += _ai.Avoidance.GetAvoidanceForce();
        _ai.Agent.ApplySteering(steer);
    }
}