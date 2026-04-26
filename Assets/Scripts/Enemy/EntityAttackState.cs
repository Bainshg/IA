using UnityEngine;

public class EntityAttackState : EntityState
{
    private EnemyAI _ai;

    public EntityAttackState(EnemyAI ai, StateMachine<EntityStates> sm) : base(sm) => _ai = ai;

    public override void Awake()
    {
        Debug.Log("¡EL ENEMIGO ESTÁ ATACANDO!");
    }

    public override void Execute()
    {
        // Aquí podrías llamar a una función de "GameOver" en tu LevelManager
        // O simplemente seguir pegado al player
        Vector3 steer = SteeringBehaviours.Seek(_ai.transform, _ai.PlayerTransform.position, _ai.Agent.Velocity, _ai.Agent.MaxSpeed);
        _ai.Agent.ApplySteering(steer + _ai.Avoidance.GetAvoidanceForce());
    }
}