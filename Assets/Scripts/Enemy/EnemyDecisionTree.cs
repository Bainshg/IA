using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    private EnemyAI _ai;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float safeEscapeDistance = 15f;

    void Awake() => _ai = GetComponent<EnemyAI>();

    public void OnUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, _ai.PlayerTransform.position);

        bool isAlreadyFleeing = _ai.StateMachine.CurrentState != null && 
                                _ai.StateMachine.CurrentState.GetType() == typeof(EntityRunAwayState);
        
        // Si se estaba escapando, le importa la distancia al player (no podemos hacer chequeos de vision)
        if (isAlreadyFleeing)
        {
            if (distToPlayer < safeEscapeDistance)
            {
                return; // Sigue huyendo 
            }
            else
            {
                // Ya está lo suficientemente lejos, vuelve a su rutina
                _ai.StateMachine.ChangeState(_ai.NeedsToIdle ? EntityStates.Idle : EntityStates.Patrol);
                return; 
            }
        }

        // Lógica normal de detección
        if (_ai.LoS.CanSeeTarget(_ai.PlayerTransform))
        {
            if (distToPlayer <= attackDistance) // atacar
            {
                _ai.StateMachine.ChangeState(EntityStates.Attack); 
            }
            else
            {
                if (_ai.IsAggressive)
                    _ai.StateMachine.ChangeState(EntityStates.Chase);
                else
                    _ai.StateMachine.ChangeState(EntityStates.RunAway);
            }
        }
        else 
        {
            if (_ai.NeedsToIdle)
                _ai.StateMachine.ChangeState(EntityStates.Idle);
            else
                _ai.StateMachine.ChangeState(EntityStates.Patrol);
        }
    }
}