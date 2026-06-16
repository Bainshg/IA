using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    private EnemyAI _ai;
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private float safeEscapeDistance = 15f;
    [SerializeField] private float detectionBuffer = 1.0f; // Margen para evitar el jitter
    [SerializeField] private float chaseMemoryDuration = 3f; // Segundos que sigue persiguiendo sin LoS (Theta*)

    void Awake() => _ai = GetComponent<EnemyAI>();

    public void OnUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, _ai.PlayerTransform.position);
        bool canSee = _ai.LoS.CanSeeTarget(_ai.PlayerTransform);
        bool isFleeing = _ai.StateMachine.CurrentState is EntityRunAwayState;

        // Logica de Huida (Prioridad si ya estaba huyendo)
        if (isFleeing)
        {
            // Solo deja de huir si está lejos (distancia + buffer)
            if (distToPlayer < safeEscapeDistance + detectionBuffer) return;

            _ai.StateMachine.ChangeState(_ai.NeedsToIdle ? EntityStates.Idle : EntityStates.Patrol);
            return;
        }

        // Lógica de Detección Normal
        if (canSee)
        {
            // refresco la ultima posicion conocida mientras lo vea
            _ai.MarkPlayerSeen(_ai.PlayerTransform.position);

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
            // sin linea de vista: si es agresivo y lo vio hace poco, sigue por pathfinding
            // (Theta* a la ultima posicion conocida).
            if (_ai.IsAggressive && _ai.SawPlayerWithin(chaseMemoryDuration))
            {
                _ai.StateMachine.ChangeState(EntityStates.Chase);
            }
            else if (_ai.NeedsToIdle) _ai.StateMachine.ChangeState(EntityStates.Idle);
            else _ai.StateMachine.ChangeState(EntityStates.Patrol);
        }
    }
}