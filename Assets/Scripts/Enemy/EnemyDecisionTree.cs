using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    private EnemyAI _ai;
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private float safeEscapeDistance = 15f;
    [SerializeField] private float detectionBuffer = 1.0f; // Margen para evitar el jitter

    void Awake() => _ai = GetComponent<EnemyAI>();

    public void OnUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, _ai.PlayerTransform.position);
        bool canSee = _ai.LoS.CanSeeTarget(_ai.PlayerTransform);
        bool isFleeing = _ai.StateMachine.CurrentState is EntityRunAwayState;

        // 1. Lógica de Huida (Prioridad si ya estaba huyendo)
        if (isFleeing)
        {
            // Solo deja de huir si está REALMENTE lejos (distancia + buffer)
            if (distToPlayer < safeEscapeDistance + detectionBuffer) return;

            _ai.StateMachine.ChangeState(_ai.NeedsToIdle ? EntityStates.Idle : EntityStates.Patrol);
            return;
        }

        // 2. Lógica de Detección Normal
        if (canSee)
        {
            if (distToPlayer <= attackDistance)
            {
                _ai.StateMachine.ChangeState(EntityStates.Attack);
            }
            else
            {
                _ai.StateMachine.ChangeState(_ai.IsAggressive ? EntityStates.Chase : EntityStates.RunAway);
            }
        }
        else
        {
            // 3. Rutina si no ve a nadie
            if (_ai.NeedsToIdle) _ai.StateMachine.ChangeState(EntityStates.Idle);
            else _ai.StateMachine.ChangeState(EntityStates.Patrol);
        }
    }
}