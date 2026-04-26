using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    private EnemyAI _ai;
    [SerializeField] private float attackDistance = 2.5f;

    void Awake() => _ai = GetComponent<EnemyAI>();

    public void OnUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, _ai.PlayerTransform.position);

        // 1. ¿Ve al jugador?
        if (_ai.LoS.CanSeeTarget(_ai.PlayerTransform))
        {
            // 2. Si está muy cerca, ATACA 
            if (distToPlayer <= attackDistance)
            {
                _ai.StateMachine.ChangeState(EntityStates.Attack);
            }
            // 3. Si no está cerca pero lo ve, elige según su grupo 
            else
            {
                if (_ai.IsAggressive)
                    _ai.StateMachine.ChangeState(EntityStates.Chase); // Usará Pursuit
                else
                    _ai.StateMachine.ChangeState(EntityStates.RunAway); // Usará Evade
            }
        }
        else
        {
            // 4. Si no hay nadie, patrulla o descansa 
            if (_ai.NeedsToIdle)
                _ai.StateMachine.ChangeState(EntityStates.Idle);
            else
                _ai.StateMachine.ChangeState(EntityStates.Patrol);
        }
    }
}