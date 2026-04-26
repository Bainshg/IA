using UnityEngine;

public class EntityRunAwayState : EntityState
{
    private EnemyAI _ai;
    private PlayerMovement _playerMovement;

    public EntityRunAwayState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm)
    {
        _ai = ai;
        _playerMovement = _ai.PlayerTransform.GetComponent<PlayerMovement>();
    }

    public override void Execute()
    {
        Vector3 playerVelocity = _playerMovement != null ? _playerMovement.Velocity : Vector3.zero;
        
        Vector3 steer = SteeringBehaviours.Evade(
            _ai.transform, 
            _ai.PlayerTransform, 
            playerVelocity, 
            _ai.Agent.Velocity, 
            _ai.Agent.MaxSpeed
        );

        // El esquive tambi�n se aplica al huir
        steer += _ai.Avoidance.GetAvoidanceForce();
        _ai.Agent.ApplySteering(steer);
    }
}